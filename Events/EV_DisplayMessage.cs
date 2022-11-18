using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;

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

        [Header("Conditions")]
        public string switchUuid;
        public bool switchValue;

        public string variableUuid;
        public bool equalsToVariableValue = true;
        public float variableValue = -1;

        private void Awake()
        {
            dialogueManager = FindObjectOfType<DialogueManager>(true);
        }

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

            if (System.String.IsNullOrEmpty(variableUuid) == false)
            {
                // If depends on variable, evaluate value:
                var val = VariableManager.instance.GetVariableValue(variableUuid);
                
                if (equalsToVariableValue && val == variableValue || equalsToVariableValue == false && val != variableValue)
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
                if (facePlayer)
                {
                    FacePlayer();
                }

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
