using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_SubEvent : EventBase
    {
        public GameObject subEvents;

        public bool filterInactiveSubEvents = false;

        public override IEnumerator Dispatch()
        {
            var evs = subEvents.GetComponents<EventBase>();

            if (evs.Length <= 0)
            {
                evs = subEvents.GetComponentsInChildren<EventBase>();
            }

            if (evs.Length > 0)
            {
                foreach (EventBase subEvent in evs)
                {
                    if (filterInactiveSubEvents && !subEvent.gameObject.activeSelf)
                    {
                        continue;
                    }

                    yield return subEvent.Dispatch();
                }
            }

        }
    }
}
