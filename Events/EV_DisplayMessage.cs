using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace AF
{

    public class EV_DisplayMessage : EventBase
    {
        public Character character;

        [TextArea] public string message;

        public List<DialogueChoice> choices = new List<DialogueChoice>();

        public float textDelay = 0.05f;

        [Header("Animations")]
        public Animator animator;
        public string animationClip;

        public bool showHint = false;

        DialogueManager dialogueManager;

        public bool facePlayer = false;

        private void Awake()
        {
            dialogueManager = FindObjectOfType<DialogueManager>(true);
        }

        public override IEnumerator Dispatch()
        {   
            if (facePlayer) { FacePlayer();  }

            if (!System.String.IsNullOrEmpty(animationClip))
            {
                animator.CrossFade(animationClip, 0.2f);
            }


            yield return dialogueManager.ShowDialogueWithChoices(
                character, message, choices, textDelay, showHint);


            if (!System.String.IsNullOrEmpty(animationClip))
            {
                animator.CrossFade("NoGesture", 0.2f);
            }
        }

        void FacePlayer()
        {
            Transform player = GameObject.FindWithTag("Player").transform;

            var npcLookPos = player.transform.position - this.transform.position;
            npcLookPos.y = 0;

            this.transform.rotation = Quaternion.LookRotation(npcLookPos);

        }
    }

}
