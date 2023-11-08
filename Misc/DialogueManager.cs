using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Video;

namespace AF
{
    [System.Serializable]
    public class DialogueChoice
    {
        public LocalizedText choiceText;

        [Header("Use Sub Events")]
        public GameObject subEventPage;

        [Header("Use Quick Response")]
        public Character replier;
        public LocalizedText response;
        public UnityEvent onResponseFinished;

        [Header("Depends on Switch?")]
        public SwitchEntry switchEntry;
        public bool switchValue = false;

        public SwitchEntry switchEntry2;
        public bool switchValue2;


        [Header("Reputation")]
        public int reputationAmountToIncrease = 0;
        public int reputationAmountToDecrease = 0;
        public SwitchEntry reputationSwitchEntry;
    }

    public class DialogueManager : MonoBehaviour
    {
        UIDocumentDialogueWindow uIDocumentDialogueWindow;
        StarterAssetsInputs inputs;

        private void Awake()
        {
            uIDocumentDialogueWindow = FindObjectOfType<UIDocumentDialogueWindow>(true);
            inputs = FindObjectOfType<StarterAssetsInputs>(true);
        }

        public IEnumerator ShowDialogueWithChoices(Character character, LocalizedText message, List<DialogueChoice> dialogueChoices, float textDelay, bool showHint, Texture2D image, VideoPlayer video)
        {
            uIDocumentDialogueWindow.gameObject.SetActive(false);

            uIDocumentDialogueWindow.showHint = showHint;

            uIDocumentDialogueWindow.textDelay = textDelay;
            if (character != null)
            {
                uIDocumentDialogueWindow.actorName = character.name;
                uIDocumentDialogueWindow.actorTitle = character.title.GetText();
                uIDocumentDialogueWindow.actorAvatar = character.avatar;
            }
            else
            {
                uIDocumentDialogueWindow.actorName = "";
                uIDocumentDialogueWindow.actorTitle = "";
                uIDocumentDialogueWindow.actorAvatar = null;
            }

            if (image != null)
            {
                uIDocumentDialogueWindow.image = image;
            }
            else
            {
                uIDocumentDialogueWindow.image = null;
            }

            if (video != null)
            {
                uIDocumentDialogueWindow.video = video;
            }
            else
            {
                uIDocumentDialogueWindow.video = null;
            }

            uIDocumentDialogueWindow.dialogueText = message.GetText();
            uIDocumentDialogueWindow.dialogueChoices.Clear();

            if (dialogueChoices != null && dialogueChoices.Count > 0)
            {
                // We need to copy clean the dialogue choices so we make sure we dont mutate the event's choice, very funny bug
                foreach (var clonedChoice in dialogueChoices)
                {
                    if (clonedChoice.switchEntry != null)
                    {
                        bool shouldAdd = false;
                        // Evaluate switch
                        if (SwitchManager.instance.GetSwitchCurrentValue(clonedChoice.switchEntry) == clonedChoice.switchValue)
                        {
                            shouldAdd = true;
                        }

                        if (shouldAdd && clonedChoice.switchEntry2 != null)
                        {
                            shouldAdd = SwitchManager.instance.GetSwitchCurrentValue(clonedChoice.switchEntry2) == clonedChoice.switchValue2;
                        }

                        if (shouldAdd)
                        {
                            uIDocumentDialogueWindow.dialogueChoices.Add(clonedChoice);
                        }
                    }
                    else
                    {
                        uIDocumentDialogueWindow.dialogueChoices.Add(clonedChoice);
                    }
                }
            }

            uIDocumentDialogueWindow.gameObject.SetActive(true);

            yield return new WaitUntil(() => inputs.interact == false);

            while (uIDocumentDialogueWindow.hasFinishedTypewriter == false)
            {
                if (inputs.interact || Gamepad.current != null && Gamepad.current.buttonWest.isPressed)
                {
                    uIDocumentDialogueWindow.ShowAllTextAtOnce();
                }

                yield return null;
            }

            yield return new WaitUntil(() => inputs.interact == false);

            if (Gamepad.current != null)
            {
                yield return new WaitUntil(() => Gamepad.current.buttonWest.IsActuated() == false);
            }

            yield return new WaitUntil(
                () => inputs.interact == true
                || Gamepad.current != null && (Gamepad.current.buttonWest.isPressed || Gamepad.current.buttonSouth.isPressed));

            if (dialogueChoices != null && dialogueChoices.Count > 0)
            {
                yield return uIDocumentDialogueWindow.ShowChoices();
            }

            yield return new WaitUntil(() => inputs.interact == false);

            uIDocumentDialogueWindow.gameObject.SetActive(false);
        }
    }
}
