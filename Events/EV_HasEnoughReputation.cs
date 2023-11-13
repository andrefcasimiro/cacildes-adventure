using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class EV_HasEnoughReputation : EV_Condition
    {
        public bool equal = false;
        public bool greaterOrEqualThan = false;
        public bool greaterThan = false;
        public bool lessThan = false;
        public bool lessOrEqualThan = false;

        public int value = 0;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;

        public override IEnumerator Dispatch()
        {
            bool finalValue = false;
            if (equal)
            {
                finalValue = playerStatsDatabase.GetCurrentReputation() == value;
            }
            else if (greaterOrEqualThan)
            {
                finalValue = playerStatsDatabase.GetCurrentReputation() >= value;
            }
            else if (greaterThan)
            {
                finalValue = playerStatsDatabase.GetCurrentReputation() > value;
            }
            else if (lessThan)
            {
                finalValue = playerStatsDatabase.GetCurrentReputation() < value;
            }
            else if (lessOrEqualThan)
            {
                finalValue = playerStatsDatabase.GetCurrentReputation() <= value;
            }

            yield return DispatchConditionResults(finalValue);
        }
    }

}
