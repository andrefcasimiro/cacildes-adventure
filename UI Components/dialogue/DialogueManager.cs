using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class DialogueManager : InputListener
    {
        public IEnumerator ShowDialogue(string actorName, string message)
        {
            UIDocumentDialogue uIDocumentDialogue = FindObjectOfType<UIDocumentDialogue>();

            yield return new WaitUntil(() => hasPressedConfirmButton == false);

            uIDocumentDialogue.DisplayDialogue(actorName, message);

            yield return new WaitUntil(() => hasPressedConfirmButton == true);
            yield return new WaitUntil(() => hasPressedConfirmButton == false);

            uIDocumentDialogue.CloseDialogue();
        }

        public IEnumerator ShowDialogueWithChoices(string actorName, string message, Choice[] choices)
        {
            UIDocumentDialogue uIDocumentDialogue = FindObjectOfType<UIDocumentDialogue>();

            yield return uIDocumentDialogue.DisplayDialogueWithChoices(actorName, message, choices);
        }
    }
}