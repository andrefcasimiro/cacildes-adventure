using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_DisplayMessage : EventBase
    {
        public string actorName;

        [TextArea] public string message;

        [Header("Camera Settings")]
        public GameObject customCamera;
        public float transitionTime = 0f;

        [Header("Graphics Settings")]
        public GameObject graphicsToHide;

        [Header("Delays")]
        public float startDelay = 0f;
        public float endDelay = 0f;

        [Header("Animations")]
        public Animator animator;
        public string animationClip;

        DialogueManager dialogueManager;

        private void Awake()
        {
            dialogueManager = FindObjectOfType<DialogueManager>(true);
        }

        public override IEnumerator Dispatch()
        {   
            if (customCamera != null)
            {
                CameraManager cameraManager = FindObjectOfType<CameraManager>(true);

                cameraManager.SetTransitionTime(transitionTime);
                cameraManager.SwitchCamera(customCamera);
            }

            if (graphicsToHide != null)
            {
                graphicsToHide.gameObject.SetActive(false);
            }

            yield return new WaitForSeconds(startDelay);

            if (!System.String.IsNullOrEmpty(animationClip))
            {
                animator.CrossFade(animationClip, 0.2f);
            }

            var actorText = actorName;
            var dialogueText = message;

            yield return StartCoroutine(dialogueManager.ShowDialogue(actorText, dialogueText));

            yield return new WaitForSeconds(endDelay);

            if (!System.String.IsNullOrEmpty(animationClip))
            {
                animator.CrossFade("NoGesture", 0.2f);
            }

            if (graphicsToHide != null)
            {
                graphicsToHide.gameObject.SetActive(true);
            }
        }
    }

}
