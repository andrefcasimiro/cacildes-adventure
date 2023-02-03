using StarterAssets;
using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_SetCameraPriority : EventBase
    {
        public Cinemachine.CinemachineVirtualCamera desiredCamera;
        public int priority = 0;

        public override IEnumerator Dispatch()
        {
            desiredCamera.Priority = priority;
            yield return null;
        }
    }

}
