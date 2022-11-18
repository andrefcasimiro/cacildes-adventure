using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class EV_RandomDialogues : EventBase
    {
        [TextArea]
        public string comment = "Put some dialogue events in the children of this gameobject";


        public override IEnumerator Dispatch()
        {
            var childLength = transform.childCount;

            yield return null;

            var chance = Random.Range(0, childLength);

            var events = transform.GetChild(chance).GetComponents<EventBase>();

            if (events.Length > 0)
            {
                foreach (EventBase subEvent in events)
                {
                    yield return subEvent.Dispatch();
                }
            }
        }

    }

}
