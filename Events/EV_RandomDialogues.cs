using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_RandomDialogues : EventBase
    {
        public GameObject[] dialogueEvents;

        public override IEnumerator Dispatch()
        {
            var chance = Random.Range(0, dialogueEvents.Length);

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
