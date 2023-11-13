using System.Collections;
using System.Collections.Generic;
using AF.Stats;
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

        [Header("Components")]
        public StatsBonusController playerStatsBonusController;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;

        public override IEnumerator Dispatch()
        {
            bool finalValue = false;

            int currentValue = 0;
            if (isVitality)
            {
                currentValue = playerStatsDatabase.vitality + playerStatsBonusController.vitalityBonus;
            }
            else if (isEndurance)
            {
                currentValue = playerStatsDatabase.endurance + playerStatsBonusController.enduranceBonus;
            }
            else if (isStrength)
            {
                currentValue = playerStatsDatabase.strength + playerStatsBonusController.strengthBonus;
            }
            else if (isDexterity)
            {
                currentValue = playerStatsDatabase.dexterity + playerStatsBonusController.dexterityBonus;
            }
            else if (isIntelligence)
            {
                currentValue = playerStatsDatabase.intelligence + playerStatsBonusController.intelligenceBonus;
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
