using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_ChangeCamera : EventBase
    {
        public GameObject desiredCamera;
        public float transitionTime = 1f;

        CameraManager cameraManager;

        private void Awake()
        {
            cameraManager = FindObjectOfType<CameraManager>(true);
        }

        private void Start()
        {
        }

        public override IEnumerator Dispatch()
        {
            yield return StartCoroutine(ChangeCamera());
        }

        IEnumerator ChangeCamera()
        {
            cameraManager.SetTransitionTime(transitionTime);
            cameraManager.SwitchCamera(desiredCamera);

            yield return null;
        }
    }
}
