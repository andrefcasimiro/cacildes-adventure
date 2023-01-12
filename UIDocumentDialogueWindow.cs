using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using StarterAssets;
using UnityEngine.InputSystem;
using System.Linq;

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

        MenuManager menuManager;

        private void Awake()
        {
            menuManager = FindObjectOfType<MenuManager>(true);
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


            this.root.Q<VisualElement>("HintMessage").style.display = Gamepad.current != null ? DisplayStyle.None : DisplayStyle.Flex;
            this.root.Q<VisualElement>("HintMessageGamePad").style.display = Gamepad.current != null ? DisplayStyle.Flex : DisplayStyle.None;

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

            if (Gamepad.current != null)
            {
                yield return new WaitUntil(() => Gamepad.current.buttonWest.IsActuated() == false);
            }

            if (dialogueChoices.Count > 0)
            {
                Utils.ShowCursor();

                dialogueChoicePanel.style.display = DisplayStyle.Flex;

                DialogueChoice selectedChoice = null;

                // Hack for gamepad support
                DialogueChoice focusedSelectedChoice = null;

                Button elementToFocus = null;
                foreach (var dialogueChoice in dialogueChoices)
                {
                    var newDialogueChoiceItem = dialogueChoiceItem.CloneTree();
                    newDialogueChoiceItem.Q<Button>().text = EvaluateDialogueChoiceText(dialogueChoice.choiceText);

                    menuManager.SetupButton(newDialogueChoiceItem.Q<Button>(), () =>
                    {
                        selectedChoice = dialogueChoice;
                    });

                    // Hack for gamepad support
                    newDialogueChoiceItem.Q<Button>().RegisterCallback<FocusEvent>(ev =>
                    {
                        focusedSelectedChoice = dialogueChoice;
                    });

                    if (elementToFocus == null)
                    {
                        elementToFocus = newDialogueChoiceItem.Q<Button>();
                        focusedSelectedChoice = dialogueChoice;
                    }

                    dialogueChoicePanel.Add(newDialogueChoiceItem);
                }

                FindObjectOfType<PlayerComponentManager>(true).DisableCharacterController();
                FindObjectOfType<PlayerComponentManager>(true).DisableComponents();
                elementToFocus.Focus();

                yield return new WaitUntil(() => selectedChoice != null || (Gamepad.current != null && Gamepad.current.buttonWest.isPressed && focusedSelectedChoice != null));

                Utils.HideCursor();

                FindObjectOfType<PlayerComponentManager>(true).EnableCharacterController();
                FindObjectOfType<PlayerComponentManager>(true).EnableComponents();

                // Hack for gamepad support
                if (selectedChoice == null && focusedSelectedChoice != null)
                {
                    selectedChoice = focusedSelectedChoice;
                }

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

        public string EvaluateDialogueChoiceText(string text)
        {
            if (text.StartsWith("[companion_wait="))
            {
                var str = text.Split('=');
                var companionId = str[1];
                var companions = FindObjectsOfType<CompanionManager>(true);
                var companion = companions.FirstOrDefault(x => x.companion.companionId == companionId);

                if (companion.waitingForPlayer)
                {
                    return "Follow me";
                }
                else
                {
                    return "Wait here";
                }
            }

            return text;
        }
    }

}
