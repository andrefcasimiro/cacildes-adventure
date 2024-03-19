using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class EV_InvokeEvent : EventBase
    {
        public UnityEvent eventToInvoke;
        public float waitBeforeInvoke = 0f;
        [TextArea]
        public string comment;

        public override IEnumerator Dispatch()
        {
            yield return new WaitForSeconds(waitBeforeInvoke);
            eventToInvoke.Invoke();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ExecuteImmediately()
        {
            eventToInvoke.Invoke();
        }
    }

}
