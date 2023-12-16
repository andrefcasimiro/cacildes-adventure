using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class EV_InvokeEvent : EventBase
    {
        public UnityEvent eventToInvoke;
        public float waitBeforeInvoke = 0f;

        public override IEnumerator Dispatch()
        {
            yield return new WaitForSeconds(waitBeforeInvoke);
            eventToInvoke.Invoke();
        }
    }

}
