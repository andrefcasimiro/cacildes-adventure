using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_Teleport : EventBase
    {
        public string spawnGameObjectName;
        public string sceneName;

        [Header("Components")]
        public TeleportManager teleportManager;

        [Header("Conditions")]
        public SwitchEntry switchEntry;
        public bool switchValue;

        [Header("Feature Flag - Check if is demo!!")]
        public bool deactivateIfIsDemo = false;

        [Header("Demo Edge Case")]
        //DialogueManager dialogueManager;
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

            if (skip == false)
            {
                yield return StartCoroutine(Teleport());
            }
        }

        private IEnumerator Teleport()
        {
            if (audioClip != null && audioSource != null)
            {
                audioSource.PlayOneShot(audioClip);
            }

            teleportManager.Teleport(sceneName, spawnGameObjectName);
            yield return null;
        }
    }
}
