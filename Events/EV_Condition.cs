using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_Condition : EventBase
    {
        public GameObject IF_THEN;

        public GameObject ELSE;

        public override IEnumerator Dispatch()
        {
            return base.Dispatch();
        }

        public IEnumerator DispatchConditionResults(bool isTrue)
        {
            if (isTrue)
            {
                var IF_EVENTS = IF_THEN.GetComponents<EventBase>();

                if (IF_EVENTS.Length > 0)
                {
                    foreach (EventBase subEvent in IF_EVENTS)
                    {
                        yield return subEvent.Dispatch();
                    }
                }
            }
            else
            {
                var ELSE_EVENTS = ELSE.GetComponents<EventBase>();

                if (ELSE_EVENTS.Length > 0)
                {
                    foreach (EventBase subEvent in ELSE_EVENTS)
                    {
                        yield return subEvent.Dispatch();
                    }
                }
            }

        }
    }
}
