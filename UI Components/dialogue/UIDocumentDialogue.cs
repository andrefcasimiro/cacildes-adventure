using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

namespace AF
{
    [System.Serializable]
    public class Choice
    {
        public string choiceText;

        public GameObject subEventPage;

        public UnityAction callbackOnClick;
    }

    public class UIDocumentDialogue : UIDocumentBase
    {
        VisualElement choiceOptions;
        Label actorName;
        Label message;

        protected override void Start()
        {
            base.Start();

            choiceOptions = this.root.Q<VisualElement>("ChoiceOptions");
            choiceOptions.AddToClassList("hide");

            actorName = this.root.Q<Label>("ActorName");
            message = this.root.Q<Label>("Message");

            this.Disable();
        }

        public void DisplayDialogue(string actorName, string message)
        {
            if (System.String.IsNullOrEmpty(actorName))
            {
                this.actorName.AddToClassList("hide");
            } else
            {
                this.actorName.RemoveFromClassList("hide");
                this.actorName.text = actorName;
            }
            this.message.text = message;
            choiceOptions.AddToClassList("hide");

            this.Enable();
        }

        public IEnumerator DisplayDialogueWithChoices(string actorName, string message, Choice[] choices)
        {
            Choice selectedChoice = null;

            this.actorName.text = actorName;
            this.message.text = message;

            choiceOptions.Clear();

            bool hasFocusedOnButtonAlready = false;
            foreach (Choice choice in choices)
            {
                Button btn = new Button();
                btn.text = choice.choiceText;

                btn.AddToClassList("game-button");
                btn.AddToClassList("dialogue-button");

                // Enter and Mouse Click
                SetupButtonClick(btn, () =>
                {
                    selectedChoice = choice;
                });
                // Z Button
                btn.RegisterCallback<KeyDownEvent>(ev =>
                {
                    if (hasPressedConfirmButton)
                    {
                        selectedChoice = choice;
                    }
                });


                choiceOptions.Add(btn);


                if (hasFocusedOnButtonAlready == false)
                {
                    btn.Focus();
                    hasFocusedOnButtonAlready = true;
                }
            }

            choiceOptions.RemoveFromClassList("hide");

            this.Enable();

            yield return new WaitUntil(() => selectedChoice != null);

            this.Disable();

            EventBase[] choiceEvents = selectedChoice.subEventPage.GetComponents<EventBase>();

            if (choiceEvents.Length > 0)
            {
                foreach (EventBase subEvent in choiceEvents)
                {
                    yield return subEvent.Dispatch();
                }
            }
        }

        public void CloseDialogue()
        {
            this.Disable();
        }
    }

}