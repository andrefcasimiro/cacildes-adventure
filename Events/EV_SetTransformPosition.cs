using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_SetTransformPosition : EventBase
    {
        public Transform target;
        public Transform desiredTransformPosition;

        [Header("Optional Vector3 Setter")]
        public bool useDesiredPositionVector = false;
        public Vector3 desiredPosition;

        Vector3 originalPosition;

        [Header("Options")]
        public bool resetToOrigin = false;
        public float forwardOffset = 0f;
        public float rightOffset = 0f;
        public float leftOffset = 0f;

        private void Awake()
        {
            if (target != null)
            {
                originalPosition = target.transform.position;
            }
        }

        public override IEnumerator Dispatch()
        {
            SetTransformPosition();
            yield return null;
        }

        public void SetTransformPosition()
        {
            if (resetToOrigin)
            {
                target.transform.position = originalPosition;
                return;
            }

            if (useDesiredPositionVector)
            {
                target.transform.localPosition = desiredPosition;
                return;
            }

            Vector3 targetPos = desiredTransformPosition.transform.position;

            if (forwardOffset > 0)
            {
                targetPos += transform.forward * forwardOffset;
            }
            if (leftOffset > 0)
            {
                targetPos += transform.right * leftOffset * -1f;
            }
            if (rightOffset > 0)
            {
                targetPos += transform.right * rightOffset;
            }

            target.transform.position = targetPos;
        }
    }
}
