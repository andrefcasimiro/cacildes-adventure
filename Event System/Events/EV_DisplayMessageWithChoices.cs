using System.Collections;

namespace AF
{

    public class EV_DisplayMessageWithChoices : EventBase
    {
        public string actorName;
        public string message;

        public Choice[] choices;

        DialogueManager dialogueManager;

        private void Awake()
        {
            dialogueManager = FindObjectOfType<DialogueManager>(true);
        }

        public override IEnumerator Dispatch()
        {
            yield return dialogueManager.ShowDialogueWithChoices(actorName, message, choices);
        }
    }

}
