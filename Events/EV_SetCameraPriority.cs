using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_SetCameraPriority : EventBase
    {
        public Cinemachine.CinemachineVirtualCamera desiredCamera;
        public int priority = 0;

        Vector3 originalPosition;
        Quaternion originalRotation;

        private void Awake()
        {
            if (desiredCamera == null)
            {
                return;
            }

            this.originalPosition = desiredCamera.transform.position;
            this.originalRotation = desiredCamera.transform.rotation;
        }

        public override IEnumerator Dispatch()
        {
            if (desiredCamera != null)
            {
                desiredCamera.Priority = priority;
                desiredCamera.transform.position = originalPosition;
                desiredCamera.transform.rotation = originalRotation;
            }

            yield return null;
        }
    }

}
