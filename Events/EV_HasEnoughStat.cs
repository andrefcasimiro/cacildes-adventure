using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class EV_HasEnoughStat : EV_Condition
    {

        [Header("Stat")]
        public bool isVitality;
        public bool isEndurance;
        public bool isStrength;
        public bool isDexterity;
        public bool isIntelligence;

        public bool equal = false;
        public bool greaterOrEqualThan = false;
        public bool greaterThan = false;
        public bool lessThan = false;
        public bool lessOrEqualThan = false;

        public int value = 0;

        EquipmentGraphicsHandler equipmentGraphicsHandler;

        public override IEnumerator Dispatch()
        {
            if (equipmentGraphicsHandler == null)
            {
                equipmentGraphicsHandler = FindAnyObjectByType<EquipmentGraphicsHandler>(FindObjectsInactive.Include);
            }

            bool finalValue = false;

            int currentValue = 0;
            if (isVitality)
            {
                currentValue = Player.instance.vitality + equipmentGraphicsHandler.vitalityBonus;
            }
            else if (isEndurance)
            {
                currentValue = Player.instance.endurance + equipmentGraphicsHandler.enduranceBonus;
            }
            else if (isStrength)
            {
                currentValue = Player.instance.strength + equipmentGraphicsHandler.strengthBonus;
            }
            else if (isDexterity)
            {
                currentValue = Player.instance.dexterity + equipmentGraphicsHandler.dexterityBonus;
            }
            else if (isIntelligence)
            {
                currentValue = Player.instance.intelligence + equipmentGraphicsHandler.intelligenceBonus;
            }

            if (equal)
            {
                finalValue = currentValue == value;
            }
            else if (greaterOrEqualThan)
            {
                finalValue = currentValue >= value;
            }
            else if (greaterThan)
            {
                finalValue = currentValue > value;
            }
            else if (lessThan)
            {
                finalValue = currentValue < value;
            }
            else if (lessOrEqualThan)
            {
                finalValue = currentValue <= value;
            }

            yield return DispatchConditionResults(finalValue);
        }
    }

}
