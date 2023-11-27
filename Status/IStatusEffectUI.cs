using UnityEngine;

namespace AF.StatusEffects
{
    public interface IStatusEffectUI
    {
        public void AddEntry(AppliedStatusEffect statusEffect, float currentMaximumResistanceToStatusEffect);
        public void UpdateEntry(AppliedStatusEffect statusEffect, float currentMaximumResistanceToStatusEffect);
        public void RemoveEntry(AppliedStatusEffect statusEffect);
    }
}
