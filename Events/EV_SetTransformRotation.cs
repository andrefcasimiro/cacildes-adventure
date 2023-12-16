using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_SetTransformRotation : EventBase
    {
        public Transform transformTarget;
        public Quaternion targetRotation;

        public override IEnumerator Dispatch()
        {
            SetTransformRotation();
            yield return null;
        }

        public void SetTransformRotation()
        {
            transformTarget.transform.rotation = targetRotation;
        }
    }
}
