using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_UpdateSwitch : EventBase
    {
        public SwitchEntry targetSwitch;
        public bool newValue;

        [Header("Options")]
        [Tooltip("If true, will only update switch when the event has stopped running")] public bool queueSwitch = false;
        public bool shouldRefreshEvents = true;

        public override IEnumerator Dispatch()
        {
            if (queueSwitch)
            {
                SwitchManager.instance.AddQueueSwitch(targetSwitch, newValue);
            }
            else if (shouldRefreshEvents)
            {
                SwitchManager.instance.UpdateSwitch(targetSwitch, newValue);
            }
            else
            {
                SwitchManager.instance.UpdateSwitchWithoutRefreshingEvents(targetSwitch, newValue);
            }

            yield return null;
        }
    }
}
