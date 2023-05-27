using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace AF
{
    public class EV_DisplayMessage : EventBase
    {
        [Header("Actor")]
        public Character character;
        public bool isCompanion = false;

        [Header("Message")]
        public LocalizedText message;
        public LocalizedText[] randomMessages;
        public LocalizedText[] negativeReputationMessages;

        [Header("Choices")]
        public List<DialogueChoice> choices = new List<DialogueChoice>();

        [Header("Settings")]
        public float textDelay = 0.05f;
        public bool showHint = false;
        public bool facePlayer = false;
        public Transform characterTransform;

        [Header("Conditions - Switches")]
        public SwitchEntry switchEntry;
        public bool switchValue;

        [Header("Conditions - Variables")]
        public VariableEntry variableEntry;
        public bool mustEqualVariableValue = true;
        public float requiredVariableValue = -1;

        [Header("Animations")]
        public Animator animator;
        public string animationClip;

        DialogueManager dialogueManager;

        private void Awake()
        {
             dialogueManager = FindObjectOfType<DialogueManager>(true);

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

            if (variableEntry != null)
            {
                // If depends on variable, evaluate value:
                var variableValue = VariableManager.instance.GetVariableValue(variableEntry);
                
                if (
                    mustEqualVariableValue && variableValue == requiredVariableValue
                    || mustEqualVariableValue == false && variableValue != requiredVariableValue)
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

                var displayMessage = message;
                if (randomMessages.Length > 0)
                {
                    var idx = Random.Range(0, randomMessages.Length);
                    displayMessage = randomMessages[idx];
                }

                if (negativeReputationMessages!= null && negativeReputationMessages.Length > 0)
                {
                    var idx = Random.Range(0, negativeReputationMessages.Length);
                    displayMessage = negativeReputationMessages[idx];
                }

                yield return dialogueManager.ShowDialogueWithChoices(
                    character, displayMessage, choices, textDelay, showHint);


                if (!string.IsNullOrEmpty(animationClip))
                {
                    animator.CrossFade("NoGesture", 0.2f);
                }
            }
        }

        void FacePlayer()
        {
            if (characterTransform == null)
            {
                characterTransform = this.transform;
            }

            Transform player = GameObject.FindWithTag("Player").transform;

            var npcLookPos = player.transform.position - characterTransform.transform.position;
            npcLookPos.y = 0;
                
            characterTransform.transform.rotation = Quaternion.LookRotation(npcLookPos);
        }
    }
}
