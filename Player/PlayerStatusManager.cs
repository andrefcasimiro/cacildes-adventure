using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class PlayerStatusManager : MonoBehaviour
    {
        DefenseStatManager defenseStatManager => GetComponent<DefenseStatManager>();
        HealthStatManager healthStatManager => GetComponent<HealthStatManager>();
        StaminaStatManager staminaStatManager => GetComponent<StaminaStatManager>();
        PlayerHealthbox playerHealthbox => GetComponentInChildren<PlayerHealthbox>();

        private void Update()
        {
            if (Player.instance.appliedStatus.Count > 0)
            {
                HandleStatusEffects();
            }
        }

        public void InflictStatusEffect(StatusEffect statusEffect, float amount, bool hasReachedFullAmount)
        {
            var idx = Player.instance.appliedStatus.FindIndex(x => x.statusEffect == statusEffect);
            if (idx != -1)
            {
                if (Player.instance.appliedStatus[idx].hasReachedTotalAmount)
                {
                    return;
                }

                Player.instance.appliedStatus[idx].currentAmount += amount;

                float maxAmountBeforeSuffering = defenseStatManager.GetMaximumStatusResistanceBeforeSufferingStatusEffect(statusEffect);
                if (Player.instance.appliedStatus[idx].currentAmount >= maxAmountBeforeSuffering)
                {
                    Player.instance.appliedStatus[idx].currentAmount = maxAmountBeforeSuffering;
                    Player.instance.appliedStatus[idx].hasReachedTotalAmount = true;



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

            Player.instance.appliedStatus.Add(appliedStatus);
            FindObjectOfType<UIDocumentStatusEffectV2>(true).AddNegativeStatusEntry(statusEffect, defenseStatManager.GetMaximumStatusResistanceBeforeSufferingStatusEffect(statusEffect));
        }

        private void HandleStatusEffects()
        {
            List<AppliedStatus> statusToDelete = new List<AppliedStatus>();

            foreach (var entry in Player.instance.appliedStatus)
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
            Player.instance.appliedStatus.Remove(appliedStatus);
            FindObjectOfType<UIDocumentStatusEffectV2>(true).RemoveNegativeStatusEntry(appliedStatus.statusEffect);

            if (appliedStatus.statusEffect.disablePlayerMovement)
            {
                FindObjectOfType<ThirdPersonController>(true).canMove = true;
            }
        }

        public void RemoveAllStatus()
        {
            var playerNegativeStatus = Player.instance.appliedStatus.ToArray();

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
                        var newHealth = Player.instance.currentHealth - percentageOfHealthToTake;
                        Player.instance.currentHealth = Mathf.Clamp(newHealth, 0, healthStatManager.GetMaxHealth());
                    }
                    else
                    {

                    }
                }
                else
                {
                    var newHealth =
                        Player.instance.currentHealth - (entry.statusEffect.damagePerSecond * Time.deltaTime)
                    ;

                    Player.instance.currentHealth = Mathf.Clamp(newHealth, 0, healthStatManager.GetMaxHealth());
                }

                if (Player.instance.currentHealth <= 0)
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
                FindObjectOfType<ThirdPersonController>(true).canMove = false;
            }
        }
    }
}

