using UnityEngine;
using UnityEngine.UI;
using AF.StatusEffects;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;

namespace AF
{
    public class CharacterStatusEffectUIManager : MonoBehaviour, IStatusEffectUI
    {
        public CharacterStatusEffectIndicator characterStatusEffectIndicatorPrefab;

        [Header("References")]
        public Transform indicatorInstancesParent;

        [SerializedDictionary("Status Effect", "UI Indicator")]
        public Dictionary<StatusEffect, CharacterStatusEffectIndicator> appliedStatusUIIndicatorInstances = new();

        public void AddEntry(AppliedStatusEffect statusEffect, float currentMaximumResistanceToStatusEffect)
        {
            CharacterStatusEffectIndicator characterStatusEffectIndicator = Instantiate(
                characterStatusEffectIndicatorPrefab, indicatorInstancesParent);

            characterStatusEffectIndicator.GetComponent<Image>().sprite = statusEffect.statusEffect.icon;
        }

        public void UpdateEntry(AppliedStatusEffect appliedStatusEffect, float currentMaximumResistanceToStatusEffect)
        {
            if (appliedStatusUIIndicatorInstances.ContainsKey(appliedStatusEffect.statusEffect))
            {
                appliedStatusUIIndicatorInstances[appliedStatusEffect.statusEffect].UpdateUI(
                    appliedStatusEffect, currentMaximumResistanceToStatusEffect);
            }
        }

        public void RemoveEntry(AppliedStatusEffect appliedStatusEffect)
        {
            if (appliedStatusUIIndicatorInstances.ContainsKey(appliedStatusEffect.statusEffect))
            {
                GameObject tmp = appliedStatusUIIndicatorInstances[appliedStatusEffect.statusEffect].gameObject;
                appliedStatusUIIndicatorInstances.Remove(appliedStatusEffect.statusEffect);
                Destroy(tmp);
            }
        }
    }
}
