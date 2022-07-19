using System.Collections;

namespace AF
{

    public class EV_UpdateLocalSwitch : EventBase
    {
        public LocalSwitch localSwitchToUpdate;
        public LocalSwitchName nextLocalSwitch;

        public override IEnumerator Dispatch()
        {
            yield return StartCoroutine(UpdateLocalSwitch());
        }

        private IEnumerator UpdateLocalSwitch()
        {
            localSwitchToUpdate.UpdateLocalSwitchValue(nextLocalSwitch);

            yield return null;
        }
    }

}