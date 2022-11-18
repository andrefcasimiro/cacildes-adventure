using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_ConditionHasEnoughAttribute : EV_Condition
    {
        // public Attribute attribute;

        public int requiredLevel;

        public override IEnumerator Dispatch()
        {
            var currentAttributeLevel = 0;

            /*if (attribute == Attribute.Vitality)
            {
                currentAttributeLevel = Player.instance.vitality;
            }
            else if (attribute == Attribute.Endurance)
            {
                currentAttributeLevel = Player.instance.endurance;
            }
            else if (attribute == Attribute.Strength)
            {
                currentAttributeLevel = Player.instance.strength;
            }
            else if (attribute == Attribute.Dexterity)
            {
                currentAttributeLevel = Player.instance.dexterity;
            }*/

            yield return DispatchConditionResults(currentAttributeLevel >= requiredLevel);
        }
    }
}
