using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF
{
    public class EnemyNegativeStatusController : MonoBehaviour
    {
        [System.Serializable]
        public class EnemyAppliedStatus
        {
            public AppliedStatus appliedStatus;

            public EnemyStatusEffectIndicator fullAmountUIIndicator;

            public float maxAmountBeforeDamage;

            public float decreaseRateWithoutDamage = 5;

            public float decreaseRateWithDamage = 1;
        }

        [Header("Negative Status")]
        [SerializeField] List<EnemyAppliedStatus> appliedStatus = new List<EnemyAppliedStatus>();
        public EnemyStatusEffectIndicator uiStatusPrefab;
        public GameObject statusEffectContainer;

        // Components
        EnemyManager enemyManager => GetComponent<EnemyManager>();
        EnemyHealthController enemyHealthController => GetComponent<EnemyHealthController>();
        CombatNotificationsController combatNotificationsController => GetComponent<CombatNotificationsController>();

        private void Update()
        {
            UpdateNegativeStatus();
        }

        void UpdateNegativeStatus()
        {
            if (appliedStatus.Count > 0)
            {
                HandleStatusEffects();
            }
        }

        public void ShowHUD()
        {
            statusEffectContainer.SetActive(true);
        }
        public void HideHUD()
        {
            statusEffectContainer.SetActive(false);
        }

        public float InflictStatusEffect(StatusEffect statusEffect, float amount)
        {
            float valueToReturn = 0f;

            var idx = this.appliedStatus.FindIndex(x => x.appliedStatus.statusEffect == statusEffect);

            if (idx != -1)
            {
                if (this.appliedStatus[idx].appliedStatus.hasReachedTotalAmount)
                {
                    return valueToReturn;
                }

                this.appliedStatus[idx].appliedStatus.currentAmount += amount;

                if (this.appliedStatus[idx].appliedStatus.currentAmount >= this.appliedStatus[idx].maxAmountBeforeDamage)
                {
                    Instantiate(statusEffect.particleOnDamage, this.transform);

                    this.appliedStatus[idx].appliedStatus.hasReachedTotalAmount = true;


                    if (statusEffect.usePercentuagelDamage && statusEffect.damagePercentualValue > 0)
                    {
                        var percentageOfHealthToTake = statusEffect.damagePercentualValue * enemyHealthController.GetMaxHealth() / 100;
                        valueToReturn = percentageOfHealthToTake;

                        combatNotificationsController.ShowStatusEffectAmount(statusEffect.name.GetText(), valueToReturn, statusEffect.barColor);
                    }
                    else
                    {
                        combatNotificationsController.ShowStatusFullAmountEffect(statusEffect.appliedStatusDisplayName.GetText(), statusEffect.barColor);
                    }

                    return valueToReturn;
                }

                combatNotificationsController.ShowStatusEffectAmount(statusEffect.name.GetText(), valueToReturn, statusEffect.barColor);

                return valueToReturn;
            }

            var negativeStatusResistance = enemyManager.enemy.negativeStatusResistances.FirstOrDefault(x => x.statusEffect == statusEffect);
            var maxAmountBeforeDamage = 100f;
            if (negativeStatusResistance != null)
            {
                maxAmountBeforeDamage = negativeStatusResistance.resistance;
            }

            AppliedStatus appliedStatus = new AppliedStatus();
            appliedStatus.statusEffect = statusEffect;
            appliedStatus.currentAmount = amount;
            appliedStatus.hasReachedTotalAmount = amount >= maxAmountBeforeDamage;

            if (appliedStatus.hasReachedTotalAmount)
            {

                combatNotificationsController.ShowStatusFullAmountEffect(statusEffect.appliedStatusDisplayName.GetText(), statusEffect.barColor);
                Instantiate(statusEffect.particleOnDamage, this.transform);

                if (appliedStatus.statusEffect.damagePercentualValue > 0)
                {
                    var percentageOfHealthToTake = appliedStatus.statusEffect.damagePercentualValue * enemyHealthController.GetMaxHealth() / 100;
                    valueToReturn = percentageOfHealthToTake;
                    combatNotificationsController.ShowStatusEffectAmount(statusEffect.name.GetText(), valueToReturn, statusEffect.barColor);
                }
            }

            EnemyAppliedStatus enemyAppliedStatus = new EnemyAppliedStatus();
            enemyAppliedStatus.appliedStatus = appliedStatus;
            enemyAppliedStatus.maxAmountBeforeDamage = maxAmountBeforeDamage;
            enemyAppliedStatus.decreaseRateWithDamage = negativeStatusResistance.decreaseRateWithDamage;
            enemyAppliedStatus.decreaseRateWithoutDamage = negativeStatusResistance.decreaseRateWithoutDamage;

            var uiIndicatorInstance = Instantiate(uiStatusPrefab, statusEffectContainer.transform);
            enemyAppliedStatus.fullAmountUIIndicator = uiIndicatorInstance;
            uiIndicatorInstance.background.sprite = enemyAppliedStatus.appliedStatus.statusEffect.spriteIndicator;
            uiIndicatorInstance.fill.sprite = enemyAppliedStatus.appliedStatus.statusEffect.spriteIndicator;

            this.appliedStatus.Add(enemyAppliedStatus);

            return valueToReturn;
        }

        private void HandleStatusEffects()
        {
            List<EnemyAppliedStatus> statusToDelete = new List<EnemyAppliedStatus>();

            foreach (var entry in this.appliedStatus)
            {
                entry.appliedStatus.currentAmount -= (entry.appliedStatus.hasReachedTotalAmount
                    ? entry.decreaseRateWithDamage
                    : entry.decreaseRateWithoutDamage) * Time.deltaTime;

                float uiValue = entry.appliedStatus.currentAmount / entry.maxAmountBeforeDamage;
                entry.fullAmountUIIndicator.UpdateUI(uiValue, entry.appliedStatus.hasReachedTotalAmount);

                if (entry.appliedStatus.hasReachedTotalAmount)
                {
                    EvaluateEffect(entry, statusToDelete);
                }

                if (entry.appliedStatus.currentAmount <= 0 || entry.appliedStatus.hasReachedTotalAmount && entry.appliedStatus.statusEffect.effectIsImmediate)
                {
                    statusToDelete.Add(entry);
                }
            }

            foreach (var status in statusToDelete)
            {
                RemoveAppliedStatus(status);
            }

        }

        public void RemoveAppliedStatus(EnemyAppliedStatus appliedStatus)
        {
            Destroy(appliedStatus.fullAmountUIIndicator.gameObject);

            this.appliedStatus.Remove(appliedStatus);

            HandleNegativeStatusDeletion(appliedStatus.appliedStatus);
        }

        public void ClearAllNegativeStatus()
        {
            EnemyAppliedStatus[] negativeStatusArray = new EnemyAppliedStatus[this.appliedStatus.Count];
            this.appliedStatus.CopyTo(negativeStatusArray);

            foreach (var _negativeStatus in negativeStatusArray)
            {
                RemoveAppliedStatus(_negativeStatus);
            }
        }

        void EvaluateEffect(EnemyAppliedStatus enemyAppliedStatus, List<EnemyAppliedStatus> statusToDelete)
        {
            var entry = enemyAppliedStatus.appliedStatus;

            if (entry.statusEffect.statAffected == Stat.Health)
            {
                if (entry.statusEffect.effectIsImmediate)
                {
                    if (entry.statusEffect.usePercentuagelDamage)
                    {
                        var percentageOfHealthToTake = entry.statusEffect.damagePercentualValue * enemyHealthController.GetMaxHealth() / 100;
                        var newHealth = enemyHealthController.currentHealth - percentageOfHealthToTake;
                        enemyHealthController.currentHealth = Mathf.Clamp(newHealth, 0, enemyHealthController.GetMaxHealth());
                    }
                }
                else
                {
                    var newHealth = enemyHealthController.currentHealth - (entry.statusEffect.damagePerSecond * Time.deltaTime);

                    enemyHealthController.currentHealth = Mathf.Clamp(newHealth, 0, enemyHealthController.GetMaxHealth());
                }

                if (enemyHealthController.currentHealth <= 0)
                {
                    enemyHealthController.Die();

                    // Remove this status
                    statusToDelete.Add(enemyAppliedStatus);
                }

                return;
            }


            if (entry.statusEffect.damageSelf)
            {
                enemyManager.enemyCombatController.isDamagingHimself = true;
            }

            if (entry.statusEffect.ignoreDefense)
            {
                enemyManager.enemyHealthController.ignoreDefense = true;
            }

            if (entry.statusEffect.instantDeath)
            {
                enemyManager.enemyHealthController.Die();
            }

            if (entry.statusEffect.slowDownAnimatorSpeedValue > 0f)
            {
                enemyManager.animator.speed -= entry.statusEffect.slowDownAnimatorSpeedValue;
            }

        }

        void HandleNegativeStatusDeletion(AppliedStatus appliedStatus)
        {
            if (appliedStatus.statusEffect.damageSelf)
            {
                enemyManager.enemyCombatController.isDamagingHimself = false;
            }

            if (appliedStatus.statusEffect.ignoreDefense)
            {
                enemyManager.enemyHealthController.ignoreDefense = false;
            }

            if (appliedStatus.statusEffect.slowDownAnimatorSpeedValue > 0f)
            {
                enemyManager.animator.speed += appliedStatus.statusEffect.slowDownAnimatorSpeedValue;
            }
        }

    }
}
