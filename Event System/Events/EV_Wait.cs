using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_Wait : EventBase
    {
        public float timeToWait;

        public override IEnumerator Dispatch()
        {
            yield return StartCoroutine(Wait());
        }

        private IEnumerator Wait()
        {
            yield return new WaitForSeconds(timeToWait);
        }
    }

}
