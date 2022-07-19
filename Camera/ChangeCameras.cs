using UnityEngine;

namespace AF
{
    public class ChangeCameras : MonoBehaviour
    {
        public GameObject cameraToTransitionTo;

        CameraManager cameraManager;

        private void Awake()
        {
            cameraManager = FindObjectOfType<CameraManager>(true);
        }

        private void Start()
        {
            GetComponent<MeshRenderer>().enabled = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                cameraManager.SwitchCamera(cameraToTransitionTo);
            }
        }
    }
}
