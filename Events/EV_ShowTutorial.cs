using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_ShowTutorial : EventBase
    {
        public Tutorial tutorial;

        public override IEnumerator Dispatch()
        {
            tutorial.gameObject.SetActive(true);

            FindObjectOfType<PlayerComponentManager>(true).isInTutorial = true;
            FindObjectOfType<PlayerComponentManager>(true).DisableComponents();
            FindObjectOfType<ThirdPersonController>().LockCameraPosition = true;

            yield return new WaitUntil(() => tutorial.gameObject.activeSelf == false);

            FindObjectOfType<ThirdPersonController>().LockCameraPosition = false;
            FindObjectOfType<PlayerComponentManager>(true).EnableComponents();
            FindObjectOfType<PlayerComponentManager>(true).isInTutorial = false;

        }
    }

}
