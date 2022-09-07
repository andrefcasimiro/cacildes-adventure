using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_ConditionHasEnoughAttribute : EV_Condition
    {
        public Attribute attribute;

        public int requiredLevel;

        public override IEnumerator Dispatch()
        {
            var currentAttributeLevel = 0;

            if (attribute == Attribute.Vitality)
            {
                currentAttributeLevel = PlayerStatsManager.instance.vitality;
            }
            else if (attribute == Attribute.Endurance)
            {
                currentAttributeLevel = PlayerStatsManager.instance.endurance;
            }
            else if (attribute == Attribute.Intelligence)
            {
                currentAttributeLevel = PlayerStatsManager.instance.intelligence;
            }
            else if (attribute == Attribute.Strength)
            {
                currentAttributeLevel = PlayerStatsManager.instance.strength;
            }
            else if (attribute == Attribute.Dexterity)
            {
                currentAttributeLevel = PlayerStatsManager.instance.dexterity;
            }
            else if (attribute == Attribute.Arcane)
            {
                currentAttributeLevel = PlayerStatsManager.instance.arcane;
            }
            else if (attribute == Attribute.Charisma)
            {
                currentAttributeLevel = PlayerStatsManager.instance.charisma;
            }
            else if (attribute == Attribute.Luck)
            {
                currentAttributeLevel = PlayerStatsManager.instance.luck;
            }

            yield return DispatchConditionResults(currentAttributeLevel >= requiredLevel);
        }
    }
}