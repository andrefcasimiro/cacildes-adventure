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

        public override IEnumerator Dispatch()
        {
            bool finalValue = false;
            if (equal)
            {
                finalValue = Player.instance.GetCurrentReputation() == value;
            }
            else if (greaterOrEqualThan)
            {
                finalValue = Player.instance.GetCurrentReputation() >= value;
            }
            else if (greaterThan)
            {
                finalValue = Player.instance.GetCurrentReputation() > value;
            }
            else if (lessThan)
            {
                finalValue = Player.instance.GetCurrentReputation() < value;
            }
            else if (lessOrEqualThan)
            {
                finalValue = Player.instance.GetCurrentReputation() <= value;
            }

            yield return DispatchConditionResults(finalValue);
        }
    }

}
