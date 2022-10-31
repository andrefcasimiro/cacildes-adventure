using System.Collections;

namespace AF
{

    public class EV_UpdateSwitch : EventBase
    {
        public Switch targetSwitch;
        public bool newValue;

        public override IEnumerator Dispatch()
        {
            SwitchManager.instance.UpdateSwitch(targetSwitch.ID, newValue);

            yield return null;
        }

    }

}