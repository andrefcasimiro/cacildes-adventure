using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace AF
{
    public class EV_DisplayMessage : EventBase
    {
        [Header("Actor")]
        public Character character;
        public bool isCompanion = false;

        [Header("Message")]
        public LocalizedText message;

        [Header("Choices")]
        public List<DialogueChoice> choices = new List<DialogueChoice>();

        [Header("Settings")]
        public float textDelay = 0.05f;
        public bool showHint = false;
        public bool facePlayer = false;

        [Header("Conditions - Switches")]
        public SwitchEntry switchEntry;
        public bool switchValue;

        [Header("Conditions - Variables")]
        public string variableUuid;
        public bool equalsToVariableValue = true;
        public float variableValue = -1;

        [Header("Animations")]
        public Animator animator;
        public string animationClip;

        DialogueManager dialogueManager => FindObjectOfType<DialogueManager>(true);

        private void Awake()
        {
            if (isCompanion)
            {
                character = GetComponentInParent<CompanionManager>(true).companion.character;
            }
        }

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

            if (string.IsNullOrEmpty(variableUuid) == false)
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

                if (!string.IsNullOrEmpty(animationClip))
                {
                    animator.CrossFade(animationClip, 0.2f);
                }

                yield return dialogueManager.ShowDialogueWithChoices(
                    character, message, choices, textDelay, showHint);


                if (!string.IsNullOrEmpty(animationClip))
                {
                    animator.CrossFade("NoGesture", 0.2f);
                }
            }
        }

        void FacePlayer()
        {
            Transform player = GameObject.FindWithTag("Player").transform;

            var npcLookPos = player.transform.position - transform.position;
            npcLookPos.y = 0;

            transform.rotation = Quaternion.LookRotation(npcLookPos);
        }
    }
}
