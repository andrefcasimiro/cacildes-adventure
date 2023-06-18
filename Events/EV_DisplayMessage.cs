using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace AF
{
    [System.Serializable]
    public class SwitchRandomMessages {

        public LocalizedText message;
        public SwitchEntry switchDependant;
        public bool requiredSwitchValueToDisplayMessage;
    }

    public class EV_DisplayMessage : EventBase
    {
        [Header("Actor")]
        public Character character;
        public bool isCompanion = false;

        [Header("Message")]
        public LocalizedText message;
        public LocalizedText[] randomMessages;
        public LocalizedText[] negativeReputationMessages;

        [Header("Conditional Random Messages")]
        public SwitchRandomMessages[] switchRandomMessages;


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
                // In the case where we have enemies like Petra that may have very nested events in them, sometimes dialogue manager is null on Awake
                if (dialogueManager == null)
                {
                    dialogueManager = FindObjectOfType<DialogueManager>(true);
                }

                if (facePlayer)
                {
                    FacePlayer();
                }

                if (!string.IsNullOrEmpty(animationClip))
                {
                    animator.CrossFade(animationClip, 0.2f);
                }

                List<LocalizedText> randomMessagesTable = randomMessages.ToList();
                if (switchRandomMessages != null && switchRandomMessages.Length > 0)
                {
                    foreach (var switchRandomMessage in switchRandomMessages)
                    {
                        if (SwitchManager.instance.GetSwitchCurrentValue(switchRandomMessage.switchDependant) == switchRandomMessage.requiredSwitchValueToDisplayMessage) {
                            randomMessagesTable.Add(switchRandomMessage.message);
                        }
                    }
                }

                var displayMessage = message;
                
                if (randomMessagesTable.Count > 0)
                {
                    var idx = Random.Range(0, randomMessagesTable.Count);
                    displayMessage = randomMessagesTable[idx];
                }
                else if (negativeReputationMessages!= null && negativeReputationMessages.Length > 0)
                {
                    if (Player.instance.GetCurrentReputation() < 0)
                    {
                        var idx = Random.Range(0, negativeReputationMessages.Length);
                        displayMessage = negativeReputationMessages[idx];
                    }

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
