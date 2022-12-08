using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
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

    public class EnemyStatusManager : MonoBehaviour
    {
        public List<NegativeStatusResistance> negativeStatusResistances = new List<NegativeStatusResistance>();

        EnemyHealthController enemyHealthController => GetComponent<EnemyHealthController>();

        [SerializeField] List<EnemyAppliedStatus> appliedStatus = new List<EnemyAppliedStatus>();

        [Header("UI")]
        public EnemyStatusEffectIndicator uiStatusPrefab;
        public GameObject statusEffectContainer;
        public FloatingText floatingText;

        private void Update()
        {
            if (appliedStatus.Count > 0)
            {
                HandleStatusEffects();
            }
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
                    ShowTextAndParticleOnReachingFullAmount(this.appliedStatus[idx].appliedStatus.statusEffect);

                    this.appliedStatus[idx].appliedStatus.hasReachedTotalAmount = true;


                    if (statusEffect.damagePercentualValue > 0)
                    {
                        var percentageOfHealthToTake = statusEffect.damagePercentualValue * enemyHealthController.maxHealth / 100;
                        valueToReturn = percentageOfHealthToTake;
                    }

                    return valueToReturn;
                }

                return valueToReturn;
            }

            var negativeStatusResistance = this.negativeStatusResistances.Find(x => x.statusEffect == statusEffect);
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
                ShowTextAndParticleOnReachingFullAmount(appliedStatus.statusEffect);

                if (appliedStatus.statusEffect.damagePercentualValue > 0)
                {
                    var percentageOfHealthToTake = appliedStatus.statusEffect.damagePercentualValue * enemyHealthController.maxHealth / 100;
                    valueToReturn = percentageOfHealthToTake;
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

        void ShowTextAndParticleOnReachingFullAmount(StatusEffect statusEffect)
        {
            if (floatingText != null)
            {
                floatingText.gameObject.SetActive(true);
                floatingText.GetComponent<TMPro.TextMeshPro>().color = statusEffect.barColor;

                floatingText.ShowText(statusEffect.appliedStatusDisplayName);
            }

            Instantiate(statusEffect.particleOnDamage, this.transform);
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
                        var percentageOfHealthToTake = entry.statusEffect.damagePercentualValue * enemyHealthController.maxHealth / 100;
                        var newHealth = enemyHealthController.currentHealth - percentageOfHealthToTake;
                        enemyHealthController.currentHealth = Mathf.Clamp(newHealth, 0, enemyHealthController.maxHealth);
                    }
                    else
                    {

                    }
                }
                else
                {
                    var newHealth =
                        enemyHealthController.currentHealth - (entry.statusEffect.damagePerSecond * Time.deltaTime)
                    ;

                    enemyHealthController.currentHealth = Mathf.Clamp(newHealth, 0, enemyHealthController.maxHealth);
                }

                if (enemyHealthController.currentHealth <= 0)
                {
                    enemyHealthController.Die();

                    // Remove this status
                    statusToDelete.Add(enemyAppliedStatus);
                }

                return;
            }


            if (entry.statusEffect.statAffected == Stat.Stamina)
            {
                return;
            }
        }

    }
}
