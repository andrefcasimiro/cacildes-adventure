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
        CharacterManager characterManager => GetComponent<CharacterManager>();
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

            var maxAmountBeforeDamage = 100f;

            AppliedStatus appliedStatus = new AppliedStatus();
            appliedStatus.statusEffect = statusEffect;
            appliedStatus.currentAmount = amount;
            appliedStatus.hasReachedTotalAmount = amount >= maxAmountBeforeDamage;

            if (appliedStatus.hasReachedTotalAmount)
            {

                combatNotificationsController.ShowStatusFullAmountEffect(statusEffect.appliedStatusDisplayName.GetText(), statusEffect.barColor);
                Instantiate(statusEffect.particleOnDamage, this.transform);

            }

            EnemyAppliedStatus enemyAppliedStatus = new EnemyAppliedStatus();
            enemyAppliedStatus.appliedStatus = appliedStatus;
            enemyAppliedStatus.maxAmountBeforeDamage = maxAmountBeforeDamage;

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

                return;
            }


            if (entry.statusEffect.slowDownAnimatorSpeedValue > 0f)
            {
                characterManager.animator.speed -= entry.statusEffect.slowDownAnimatorSpeedValue;
            }

        }

        void HandleNegativeStatusDeletion(AppliedStatus appliedStatus)
        {
            if (appliedStatus.statusEffect.slowDownAnimatorSpeedValue > 0f)
            {
                characterManager.animator.speed += appliedStatus.statusEffect.slowDownAnimatorSpeedValue;
            }
        }

    }
}
