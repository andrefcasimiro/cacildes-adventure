using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_SubEvent : EventBase
    {
        public GameObject subEvents;

        public override IEnumerator Dispatch()
        {
            var evs = subEvents.GetComponents<EventBase>();

            if (evs.Length > 0)
            {
                foreach (EventBase subEvent in evs)
                {
                    yield return subEvent.Dispatch();
                }
            }
        }
    }
}
