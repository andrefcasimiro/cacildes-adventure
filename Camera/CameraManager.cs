using UnityEngine;

namespace AF
{
    public class CameraManager : MonoBehaviour
    {
        public GameObject defaultCamera;

        public GameObject currentCamera;
        public GameObject previousCamera;

        public const float DEFAULT_TRANSITION_TIME = 1f;

        private void Update()
        {
            if (currentCamera == null)
            {
                UpdateCamera(defaultCamera);
                return;
            }

            // TODO: Clean this up
            Cinemachine.CinemachineVirtualCamera virtualCamera = currentCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>();
            if (virtualCamera.m_LookAt == null || virtualCamera.m_Follow == null)
            {
                GameObject player = FindObjectOfType<Player>(true).gameObject;

                virtualCamera.m_Follow = player.transform;
                virtualCamera.m_LookAt = player.transform;
            }
        }

        // Todo: Clean this up
        public void UpdateCamera(GameObject camera)
        {
            Cinemachine.CinemachineVirtualCamera virtualCamera = camera.GetComponent<Cinemachine.CinemachineVirtualCamera>();
            if (virtualCamera != null)
            {
                GameObject player = FindObjectOfType<Player>(true).gameObject;

                virtualCamera.m_Follow = player.transform;
                virtualCamera.m_LookAt = player.transform;

                SwitchCamera(camera);
            }
        }

        public void SetTransitionTime(float transitionTime)
        {
            var brain = Camera.main.GetComponent<Cinemachine.CinemachineBrain>();
            if (brain != null)
            {
                brain.m_DefaultBlend.m_Time = transitionTime;
            }
        }

        public void SwitchCamera(GameObject newCamera)
        {
            if (this.currentCamera != null)
            {
                this.previousCamera = this.currentCamera;
            }

            this.currentCamera = newCamera;

            if (this.previousCamera != null)
            {
                this.previousCamera.SetActive(false);
            }

            this.currentCamera.SetActive(true);
        }

        public void SwitchToPreviousCamera()
        {
            GameObject targetCamera = this.previousCamera;
            this.previousCamera = this.currentCamera;
            this.currentCamera = targetCamera;

            this.previousCamera.SetActive(false);
            this.currentCamera.SetActive(true);
        }
    }
}
