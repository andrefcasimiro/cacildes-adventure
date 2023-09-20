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

        [Header("For custom invoked events like Roberto questline")]
        public bool flushQueuedSwitchesRightAfter = false;

        public string analyticsMessage;

        public override IEnumerator Dispatch()
        {
            if (queueSwitch)
            {
                SwitchManager.instance.AddQueueSwitch(targetSwitch, newValue);
            }
            else if (shouldRefreshEvents)
            {
                SwitchManager.instance.AddQueueSwitch(targetSwitch, newValue);
                //SwitchManager.instance.UpdateSwitch(targetSwitch, newValue, null);
            }
            else
            {
                SwitchManager.instance.UpdateSwitchWithoutRefreshingEvents(targetSwitch, newValue);
            }

            if (!string.IsNullOrEmpty(analyticsMessage))
            {
                FindObjectOfType<Analytics>(true).TrackAnalyticsEvent(analyticsMessage);
            }

            if (flushQueuedSwitchesRightAfter)
            {
                SwitchManager.instance.UpdateQueuedSwitches();
            }

            yield return null;
        }
    }
}
