using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using StarterAssets;

namespace AF
{

    public class UIDocumentDialogueWindow : MonoBehaviour
    {
        public UIDocument uiDocument;

        public VisualTreeAsset dialogueChoiceItem;

        [Header("SFX")]
        public AudioClip choiceClickSfx;

        [Header("Debug Only")]
        public string actorName;
        public string dialogueText;

        [HideInInspector] public string actorTitle;

        [HideInInspector] public Sprite actorAvatar;
        
        [HideInInspector] public List<DialogueChoice> dialogueChoices = new List<DialogueChoice>();

        VisualElement root;
        VisualElement dialogueChoicePanel;
        Label text;


        [HideInInspector] public bool hasFinishedTypewriter = false;
        [HideInInspector] public float textDelay = 0.5f;

        [HideInInspector] public bool showHint = false;

        private void Awake()
        {
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            hasFinishedTypewriter = false;

            this.root = uiDocument.rootVisualElement;
            
            this.dialogueChoicePanel = this.root.Q<VisualElement>("ChoiceContainer");
            dialogueChoicePanel.Clear();
            dialogueChoicePanel.style.display = DisplayStyle.None;

            var actorNameLabel = this.root.Q<Label>("ActorName");
            var actorTitleLabel = this.root.Q<Label>("ActorTitle");
            this.text = this.root.Q<Label>("MessageText");
            var actorSprite = this.root.Q<IMGUIContainer>("ActorSprite");

            this.root.Q<VisualElement>("Hint").style.display = showHint ? DisplayStyle.Flex : DisplayStyle.None;

            if (System.String.IsNullOrEmpty(actorName) == false)
            {
                actorNameLabel.style.display = DisplayStyle.Flex;
                actorNameLabel.text = actorName;
                this.root.Q<VisualElement>("ActorInfoContainer").style.display = DisplayStyle.Flex;
            }
            else
            {
                this.root.Q<VisualElement>("ActorInfoContainer").style.display = DisplayStyle.None;
                actorNameLabel.style.display = DisplayStyle.None;
            }

            if (System.String.IsNullOrEmpty(actorTitle) == false)
            {
                actorTitleLabel.style.display = DisplayStyle.Flex;
                actorTitleLabel.text = actorTitle;
            }
            else
            {
                actorTitleLabel.style.display = DisplayStyle.None;
            }

            if (actorAvatar != null)
            {
                actorSprite.style.backgroundImage = new StyleBackground(actorAvatar);
                actorSprite.style.display = DisplayStyle.Flex;
            }
            else
            {
                actorSprite.style.display = DisplayStyle.None;
            }

            StartCoroutine(Typewrite());
        }

        public IEnumerator Typewrite()
        {
            for (int i = 0; i < dialogueText.Length + 1; i++)
            {
                var letter = dialogueText.Substring(0, i);
                text.text = letter;
                yield return new WaitForSeconds(textDelay);
            }

            hasFinishedTypewriter = true;
        }

        public void ShowAllTextAtOnce()
        {
            StopAllCoroutines();
            hasFinishedTypewriter = true;

            this.text.text = dialogueText;
        }

        public IEnumerator ShowChoices()
        {
            dialogueChoicePanel = this.root.Q<VisualElement>("ChoiceContainer");
            dialogueChoicePanel.Clear();
            dialogueChoicePanel.style.display = DisplayStyle.None;

            if (dialogueChoices.Count > 0)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                dialogueChoicePanel.style.display = DisplayStyle.Flex;

                DialogueChoice selectedChoice = null;

                foreach (var dialogueChoice in dialogueChoices)
                {
                    var newDialogueChoiceItem = dialogueChoiceItem.CloneTree();
                    newDialogueChoiceItem.Q<Button>().text = dialogueChoice.choiceText;
                    newDialogueChoiceItem.RegisterCallback<ClickEvent>(ev =>
                    {
                        BGMManager.instance.PlaySound(choiceClickSfx, null);
                        selectedChoice = dialogueChoice;
                    });
                    dialogueChoicePanel.Add(newDialogueChoiceItem);
                }

                yield return new WaitUntil(() => selectedChoice != null);
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;

                EventBase[] choiceEvents = selectedChoice.subEventPage.GetComponents<EventBase>();

                if (choiceEvents.Length > 0)
                {
                    foreach (EventBase subEvent in choiceEvents)
                    {
                        yield return subEvent.Dispatch();
                    }
                }
            }
            else
            {
                dialogueChoicePanel.style.display = DisplayStyle.None;
            }

        }
    }

}
