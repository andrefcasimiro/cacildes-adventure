using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{

    public class EV_Teleport : EventBase
    {

        public string spawnGameObjectName;
        public TeleportManager.SceneName sceneName;

        [Header("Conditions")]
        public string switchUuid;
        public bool switchValue;

        public override IEnumerator Dispatch()
        {
            bool skip = false;

            if (System.String.IsNullOrEmpty(switchUuid) == false)
            {
                // If depends on switch, evaluate value:
                ; if (SwitchManager.instance.GetSwitchValue(switchUuid) == switchValue)
                {
                    skip = false;
                }
                else
                {
                    skip = true;
                }
            }

            if (skip == false) {
                yield return StartCoroutine(Teleport());
            }
        }

        private IEnumerator Teleport()
        {
            FindObjectOfType<UIDocumentLoadingScreen>(true).gameObject.SetActive(true);
            FindObjectOfType<UIDocumentLoadingScreen>(true).UpdateLoadingBar(0f);
            yield return null;

            TeleportManager.instance.Teleport(sceneName, spawnGameObjectName);
            yield return null;
        }

    }

}