using System.Collections;

namespace AF
{

    public class EV_UpdateSwitch : EventBase
    {
        public Switch targetSwitch;
        public bool newValue;
        public bool shouldRefreshEvents = true;

        public override IEnumerator Dispatch()
        {
            if (shouldRefreshEvents)
            {
                SwitchManager.instance.UpdateSwitch(targetSwitch.ID, newValue);
            }
            else
            {
                SwitchManager.instance.UpdateSwitchWithoutRefreshingEvents(targetSwitch.ID, newValue);
            }

            yield return null;
        }

    }

}