using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AF
{
    [System.Serializable]
    public class DialogueChoice
    {
        public LocalizedText choiceText;

        public GameObject subEventPage;

        [Header("Depends on Switch?")]
        public SwitchEntry switchEntry;
        public bool switchValue = false;
    }

    public class DialogueManager : MonoBehaviour
    {
        UIDocumentDialogueWindow uIDocumentDialogueWindow;
        StarterAssets.StarterAssetsInputs inputs;

        private void Awake()
        {
            uIDocumentDialogueWindow = FindObjectOfType<UIDocumentDialogueWindow>(true);
            inputs = FindObjectOfType<StarterAssets.StarterAssetsInputs>(true);
        }

        public IEnumerator ShowDialogueWithChoices(Character character, LocalizedText message, List<DialogueChoice> dialogueChoices, float textDelay, bool showHint)
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

            uIDocumentDialogueWindow.dialogueText = message.GetText();
            uIDocumentDialogueWindow.dialogueChoices.Clear();

            if (dialogueChoices != null && dialogueChoices.Count > 0)
            {
                // We need to copy clean the dialogue choices so we make sure we dont mutate the event's choice, very funny bug
                foreach (var clonedChoice in dialogueChoices)
                {
                    if (clonedChoice.switchEntry != null)
                    {
                        // Evaluate switch
                        if (SwitchManager.instance.GetSwitchCurrentValue(clonedChoice.switchEntry) == clonedChoice.switchValue)
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

            yield return new WaitUntil(() => inputs.interact == true || Gamepad.current != null && Gamepad.current.buttonWest.isPressed);

            if (dialogueChoices.Count > 0)
            {
                yield return uIDocumentDialogueWindow.ShowChoices();
            }

            yield return new WaitUntil(() => inputs.interact == false);

            uIDocumentDialogueWindow.gameObject.SetActive(false);
        }
    }
}
