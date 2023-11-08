using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace AF
{
    public class EV_Teleport : EventBase
    {
        public string spawnGameObjectName;
        public TeleportManager.SceneName sceneName;

        [Header("Conditions")]
        public SwitchEntry switchEntry;
        public bool switchValue;

        [Header("Feature Flag - Check if is demo!!")]
        public bool deactivateIfIsDemo = false;

        [Header("Demo Edge Case")]
        DialogueManager dialogueManager;
        public LocalizedText demoLocalizedText;

        [Header("Audio Clips")]
        public AudioSource audioSource;
        public AudioClip audioClip;

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

            if (deactivateIfIsDemo && Player.instance.isDemo)
            {
                if (dialogueManager == null)
                {
                    dialogueManager = FindAnyObjectByType<DialogueManager>(FindObjectsInactive.Include);
                }

                yield return dialogueManager.ShowDialogueWithChoices(
                    null, demoLocalizedText, new List<DialogueChoice>(), 0.05f, false, null, null);

                yield break;
            }

            if (skip == false)
            {
                yield return StartCoroutine(Teleport());
            }
        }

        private IEnumerator Teleport()
        {
            if (audioClip != null && audioSource  != null)
            {
                audioSource.PlayOneShot(audioClip);
            }

            FindObjectOfType<UIDocumentLoadingScreen>(true).gameObject.SetActive(true);
            FindObjectOfType<UIDocumentLoadingScreen>(true).UpdateLoadingBar(0f);
            yield return null;

            TeleportManager.instance.Teleport(sceneName, spawnGameObjectName);
            yield return null;
        }
    }
}
