using System.Collections.Generic;
using System.Linq;
using AYellowpaper;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Events;

namespace AF.StatusEffects
{
    public class StatusController : MonoBehaviour
    {
        [Header("Character")]
        public CharacterBaseManager characterBaseManager;

        // TODO: Refactor to be in database when we have cross scene teleport to test

        [Header("Resistances")]

        [SerializedDictionary("Status Resistances", "Duration (seconds)")]
        public SerializedDictionary<StatusEffect, float> statusEffectResistances = new();
        public Dictionary<StatusEffect, float> statusEffectResistanceBonuses = new();

        public List<AppliedStatusEffect> appliedStatusEffects = new();

        // END TODO

        [Header("Effect Instances")]
        [SerializedDictionary("Status Effect", "World Instance")]
        public SerializedDictionary<StatusEffect, StatusEffectInstance> statusEffectInstances;

        [Header("UI")]
        public InterfaceReference<IStatusEffectUI, MonoBehaviour> statusEffectUI;

        [Header("Unity Events")]
        public UnityEvent onAwake;

        private void Awake()
        {
            onAwake?.Invoke();
        }

        private void Update()
        {
            if (appliedStatusEffects.Count <= 0)
            {
                return;
            }

            HandleStatusEffects();
        }

        public void InflictStatusEffect(StatusEffect statusEffect, float amount, bool hasReachedFullAmount)
        {
            var existingStatusEffectIndex = appliedStatusEffects.FindIndex(x => x.statusEffect == statusEffect);

            // If inflicted amount is greater than the character's maximum resistance, make him suffer the status effect
            float maximumResistanceToStatusEffect = GetMaximumStatusResistanceBeforeSufferingStatusEffect(statusEffect);
            if (amount >= maximumResistanceToStatusEffect)
            {
                amount = maximumResistanceToStatusEffect;
                hasReachedFullAmount = true;
            }

            if (existingStatusEffectIndex == -1)
            {
                AddStatusEffect(statusEffect, amount, hasReachedFullAmount, maximumResistanceToStatusEffect);
            }
            // Don't inflict status effect on an already fully-inflicted status effect
            else if (!appliedStatusEffects[existingStatusEffectIndex].hasReachedTotalAmount)
            {
                appliedStatusEffects[existingStatusEffectIndex].currentAmount += amount;
                HandleReachedAmountCalculation(statusEffect, existingStatusEffectIndex);
            }
        }

        void HandleReachedAmountCalculation(StatusEffect statusEffect, int appliedStatusEffectIndex)
        {
            float maxAmountBeforeSuffering = GetMaximumStatusResistanceBeforeSufferingStatusEffect(statusEffect);
            if (appliedStatusEffects[appliedStatusEffectIndex].currentAmount < maxAmountBeforeSuffering)
            {
                return;
            }

            appliedStatusEffects[appliedStatusEffectIndex].currentAmount = maxAmountBeforeSuffering;
            appliedStatusEffects[appliedStatusEffectIndex].hasReachedTotalAmount = true;

            StatusEffectInstance statusEffectInstance = GetStatusEffectInstance(statusEffect);
            if (statusEffectInstance != null)
            {
                statusEffectInstance.onApplied_Start?.Invoke();
            }
        }

        StatusEffectInstance GetStatusEffectInstance(StatusEffect statusEffect)
        {
            if (statusEffectInstances.ContainsKey(statusEffect))
            {
                return statusEffectInstances[statusEffect];
            }

            return null;
        }

        public float GetMaximumStatusResistanceBeforeSufferingStatusEffect(StatusEffect statusEffect)
        {
            float resistance = 0;
            if (statusEffectResistances.ContainsKey(statusEffect))
            {
                resistance += statusEffectResistances[statusEffect];
            }

            if (statusEffectResistanceBonuses.ContainsKey(statusEffect))
            {
                resistance += statusEffectResistanceBonuses[statusEffect];
            }

            return resistance;
        }


        void AddStatusEffect(StatusEffect statusEffect, float amount, bool hasReachedFullAmount, float maximumResistanceToStatusEffect)
        {
            if (appliedStatusEffects.Exists(x => x.statusEffect == statusEffect))
            {
                return;
            }

            AppliedStatusEffect appliedStatus = new()
            {
                statusEffect = statusEffect,
                currentAmount = amount,
                hasReachedTotalAmount = hasReachedFullAmount
            };

            appliedStatusEffects.Add(appliedStatus);


            if (statusEffectUI != null && statusEffectUI.Value != null)
            {
                statusEffectUI?.Value?.AddEntry(appliedStatus, maximumResistanceToStatusEffect);
            }

            StatusEffectInstance statusEffectInstance = GetStatusEffectInstance(statusEffect);
            if (statusEffectInstance != null && appliedStatus.hasReachedTotalAmount)
            {
                statusEffectInstance.onApplied_Start?.Invoke();
            }
        }

        private void HandleStatusEffects()
        {
            List<AppliedStatusEffect> statusToDelete = new();

            foreach (var entry in appliedStatusEffects.ToList())
            {

                entry.currentAmount -= (entry.hasReachedTotalAmount
                    ? entry.statusEffect.decreaseRateWithDamage
                    : entry.statusEffect.decreaseRateWithoutDamage) * Time.deltaTime;

                if (statusEffectUI != null && statusEffectUI.Value != null)
                {
                    statusEffectUI.Value.UpdateEntry(entry, GetMaximumStatusResistanceBeforeSufferingStatusEffect(entry.statusEffect));
                }

                if (ShouldRemove(entry))
                {
                    statusToDelete.Add(entry);
                }

                StatusEffectInstance statusEffectInstance = GetStatusEffectInstance(entry.statusEffect);
                if (entry.hasReachedTotalAmount && statusEffectInstance != null)
                {
                    statusEffectInstance.onApplied_Update?.Invoke();
                }
            }

            foreach (var status in statusToDelete)
            {
                RemoveAppliedStatus(status);
            }
        }

        bool ShouldRemove(AppliedStatusEffect appliedStatusEffect)
        {
            if (characterBaseManager?.health?.GetCurrentHealth() <= 0)
            {
                return true;
            }

            if (appliedStatusEffect.hasReachedTotalAmount && appliedStatusEffect.statusEffect.isAppliedImmediately)
            {
                return true;
            }

            if (appliedStatusEffect.currentAmount > 0)
            {
                return false;
            }

            return true;
        }

        public void RemoveAppliedStatus(AppliedStatusEffect appliedStatus)
        {
            if (appliedStatus == null || appliedStatusEffects.Contains(appliedStatus) == false)
            {
                return;
            }

            StatusEffectInstance statusEffectInstance = GetStatusEffectInstance(appliedStatus.statusEffect);
            if (statusEffectInstance != null)
            {
                statusEffectInstance.onApplied_End?.Invoke();
            }

            if (statusEffectUI != null && statusEffectUI.Value != null)
            {
                statusEffectUI.Value.RemoveEntry(appliedStatus);
            }

            appliedStatusEffects.Remove(appliedStatus);
        }

        public void RemoveAllStatuses()
        {
            AppliedStatusEffect[] appliedStatusEffectsClone = appliedStatusEffects.ToArray();
            foreach (var appliedStatusEffect in appliedStatusEffectsClone)
            {
                RemoveAppliedStatus(appliedStatusEffect);
            }
        }
    }
}
