using System.Collections;

namespace AF
{
    public class EV_SetTimeOfDay : EventBase
    {
        public DayNightManager dayNightManager;

        public int hour = 9;

        public override IEnumerator Dispatch()
        {
            dayNightManager.SetTimeOfDay(hour, 0);
            yield return null;
        }
    }
}
