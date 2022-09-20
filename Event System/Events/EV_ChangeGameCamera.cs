using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_ChangeGameCamera : EventBase
    {
        public Cinemachine.CinemachineVirtualCamera desiredCamera;

        private void Start()
        {}

        public override IEnumerator Dispatch()
        {

            var sceneCameras = FindObjectsOfType<Cinemachine.CinemachineVirtualCamera>(true);

            if (sceneCameras.Length > 0)
            {       
                foreach (var sceneCamera in sceneCameras)
                {
                    if (sceneCamera.GetComponent<MonoBehaviourID>().ID == desiredCamera.GetComponent<MonoBehaviourID>().ID)
                    {
                        sceneCamera.gameObject.SetActive(true);
                    }
                    else
                    {
                        sceneCamera.gameObject.SetActive(false);
                    }
                }

                yield return null;
            }
        }
    }
}
