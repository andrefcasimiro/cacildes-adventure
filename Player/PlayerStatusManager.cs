using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF
{
    public class PlayerStatusManager : MonoBehaviour
    {
        DefenseStatManager defenseStatManager => GetComponent<DefenseStatManager>();
        HealthStatManager healthStatManager => GetComponent<HealthStatManager>();
        StaminaStatManager staminaStatManager => GetComponent<StaminaStatManager>();
        PlayerHealthbox playerHealthbox => GetComponentInChildren<PlayerHealthbox>();
        PlayerComponentManager playerComponentManager => GetComponent<PlayerComponentManager>();
        ThirdPersonController thirdPersonController => GetComponent<ThirdPersonController>();
        UIDocumentStatusEffectV2 uIDocumentStatusEffectV2;

        [Header("Status Consumable Protections")]
        public bool immuneToPoison = false;
        public bool immuneToFrostbite = false;
        public bool immuneToFear = false;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public StatusDatabase statusDatabase;

        private void Awake()
        {
            uIDocumentStatusEffectV2 = FindObjectOfType<UIDocumentStatusEffectV2>(true);
        }

        private void Update()
        {
            if (statusDatabase.appliedStatus.Count > 0)
            {
                HandleStatusEffects();
            }
        }

        public void InflictStatusEffect(StatusEffect statusEffect, float amount, bool hasReachedFullAmount)
        {
            if (immuneToPoison && statusEffect.name.GetEnglishText() == "Poison")
            {
                return;
            }

            if (immuneToFear && statusEffect.name.GetEnglishText() == "Fear")
            {
                return;
            }

            if (immuneToFrostbite && statusEffect.name.GetEnglishText() == "Frostbite")
            {
                return;
            }

            var idx = statusDatabase.appliedStatus.FindIndex(x => x.statusEffect == statusEffect);
            if (idx != -1)
            {
                if (statusDatabase.appliedStatus[idx].hasReachedTotalAmount)
                {
                    return;
                }

                statusDatabase.appliedStatus[idx].currentAmount += amount;

                float maxAmountBeforeSuffering = defenseStatManager.GetMaximumStatusResistanceBeforeSufferingStatusEffect(statusEffect);
                if (statusDatabase.appliedStatus[idx].currentAmount >= maxAmountBeforeSuffering)
                {
                    statusDatabase.appliedStatus[idx].currentAmount = maxAmountBeforeSuffering;
                    statusDatabase.appliedStatus[idx].hasReachedTotalAmount = true;



                    if (statusEffect.particleOnDamage != null)
                    {
                        Instantiate(statusEffect.particleOnDamage, transform.position, Quaternion.identity);
                    }

                    return;
                }

                return;
            }

            AppliedStatus appliedStatus = new AppliedStatus();
            appliedStatus.statusEffect = statusEffect;
            appliedStatus.currentAmount = amount;
            appliedStatus.hasReachedTotalAmount = hasReachedFullAmount;

            statusDatabase.appliedStatus.Add(appliedStatus);
            FindObjectOfType<UIDocumentStatusEffectV2>(true).AddNegativeStatusEntry(statusEffect, defenseStatManager.GetMaximumStatusResistanceBeforeSufferingStatusEffect(statusEffect));
        }

        private void HandleStatusEffects()
        {
            List<AppliedStatus> statusToDelete = new List<AppliedStatus>();

            foreach (var entry in statusDatabase.appliedStatus.ToList())
            {
                var negativeStatusEntry = defenseStatManager.negativeStatusResistances.Find(x => x.statusEffect == entry.statusEffect);
                var decreaseRateWithDamage = 1f;
                var decreaseRateWithoutDamage = 5f;

                if (negativeStatusEntry != null)
                {
                    decreaseRateWithDamage = negativeStatusEntry.decreaseRateWithDamage;
                    decreaseRateWithoutDamage = negativeStatusEntry.decreaseRateWithoutDamage;
                }

                entry.currentAmount -= (entry.hasReachedTotalAmount
                    ? decreaseRateWithDamage
                    : decreaseRateWithoutDamage) * Time.deltaTime;

                if (entry.hasReachedTotalAmount)
                {
                    EvaluateEffect(entry, statusToDelete);

                }

                if (entry.currentAmount <= 0 || entry.hasReachedTotalAmount && entry.statusEffect.effectIsImmediate)
                {
                    statusToDelete.Add(entry);
                }
            }

            foreach (var status in statusToDelete)
            {
                RemoveAppliedStatus(status);
            }

        }

        public void RemoveAppliedStatus(AppliedStatus appliedStatus)
        {
            if (appliedStatus == null)
            {
                return;
            }

            statusDatabase.appliedStatus.Remove(appliedStatus);
            uIDocumentStatusEffectV2.RemoveNegativeStatusEntry(appliedStatus.statusEffect);

            HandleNegativeStatusDeletion(appliedStatus);
        }

        void HandleNegativeStatusDeletion(AppliedStatus appliedStatus)
        {
            if (appliedStatus.statusEffect.disablePlayerMovement)
            {
                HandlePlayerComponents(true);
            }

            if (appliedStatus.statusEffect.damageSelf)
            {
                GetComponent<PlayerCombatController>().isDamagingHimself = false;
            }

            if (appliedStatus.statusEffect.ignoreDefense)
            {
                GetComponent<DefenseStatManager>().ignoreDefense = false;
            }

            if (appliedStatus.statusEffect.slowDownAnimatorSpeedValue > 0f)
            {
                GetComponent<Animator>().speed += appliedStatus.statusEffect.slowDownAnimatorSpeedValue;
            }
        }

        public void RemoveAllStatus()
        {
            var playerNegativeStatus = statusDatabase.appliedStatus.ToArray();

            foreach (var playerNegativeStat in playerNegativeStatus)
            {
                RemoveAppliedStatus(playerNegativeStat);
            }

        }

        void EvaluateEffect(AppliedStatus entry, List<AppliedStatus> statusToDelete)
        {
            if (entry.statusEffect.statAffected == Stat.Health)
            {
                if (entry.statusEffect.effectIsImmediate)
                {
                    if (entry.statusEffect.usePercentuagelDamage)
                    {
                        var percentageOfHealthToTake = entry.statusEffect.damagePercentualValue * healthStatManager.GetMaxHealth() / 100;
                        var newHealth = playerStatsDatabase.currentHealth - percentageOfHealthToTake;
                        playerStatsDatabase.currentHealth = Mathf.Clamp(newHealth, 0, healthStatManager.GetMaxHealth());
                    }
                    else
                    {

                    }
                }
                else
                {
                    var newHealth =
                        playerStatsDatabase.currentHealth - (entry.statusEffect.damagePerSecond * Time.deltaTime)
                    ;

                    playerStatsDatabase.currentHealth = (int)Mathf.Clamp(newHealth, 0, healthStatManager.GetMaxHealth());
                }

                if (playerStatsDatabase.currentHealth <= 0)
                {
                    playerHealthbox.Die();

                    // Remove this status
                    statusToDelete.Add(entry);
                }

                return;
            }


            if (entry.statusEffect.statAffected == Stat.Stamina)
            {
                staminaStatManager.DecreaseStamina(entry.statusEffect.damagePerSecond * Time.deltaTime);
                return;
            }

            if (entry.statusEffect.disablePlayerMovement)
            {
                HandlePlayerComponents(false);
            }

            if (entry.statusEffect.damageSelf)
            {
                GetComponent<PlayerCombatController>().isDamagingHimself = true;
            }

            if (entry.statusEffect.ignoreDefense)
            {
                GetComponent<DefenseStatManager>().ignoreDefense = true;
            }

            if (entry.statusEffect.instantDeath)
            {
                FindAnyObjectByType<PlayerHealthbox>(FindObjectsInactive.Include).Die();
            }

            if (entry.statusEffect.slowDownAnimatorSpeedValue > 0f)
            {
                GetComponent<Animator>().speed -= entry.statusEffect.slowDownAnimatorSpeedValue;
            }
        }

        public void HandlePlayerComponents(bool canUse)
        {
            if (canUse)
            {
                playerComponentManager.EnableComponents();
                thirdPersonController.canMove = true;

                GetComponent<Animator>().SetBool("IsCrouched", false);
            }
            else
            {
                playerComponentManager.DisableComponents();
                thirdPersonController.enabled = true;
                thirdPersonController.canMove = false;

                GetComponent<Animator>().SetBool("IsCrouched", true);
            }
        }
    }
}

