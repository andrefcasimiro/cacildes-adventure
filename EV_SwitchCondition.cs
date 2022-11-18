using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class EV_SwitchCondition : EV_Condition
    {
        public string switchUuid;
        public bool value;

        public override IEnumerator Dispatch()
        {
            yield return DispatchConditionResults(SwitchManager.instance.GetSwitchValue(switchUuid) == value);
        }
    }

}
