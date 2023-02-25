using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_Teleport : EventBase
    {
        public string spawnGameObjectName;
        public TeleportManager.SceneName sceneName;

        [Header("Conditions")]
        public SwitchEntry switchEntry;
        public bool switchValue;

        public override IEnumerator Dispatch()
        {
            bool skip = false;

            if (switchEntry != null)
            {
                // If depends on switch, evaluate value:
                ; if (SwitchManager.instance.GetSwitchCurrentValue(switchEntry) == switchValue)
                {
                    skip = false;
                }
                else
                {
                    skip = true;
                }
            }

            if (skip == false)
            {
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
