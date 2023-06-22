using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
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

        [Header("Localization")]
        public LocalizedText pressKeyToContinueText;

        [HideInInspector] public string actorTitle;
        [HideInInspector] public Sprite actorAvatar;
        [HideInInspector] public List<DialogueChoice> dialogueChoices = new List<DialogueChoice>();

        VisualElement root;
        VisualElement dialogueChoicePanel;
        Label messageText;

        [HideInInspector] public bool hasFinishedTypewriter = false;
        [HideInInspector] public float textDelay = 0.5f;
        [HideInInspector] public bool showHint = false;

        MenuManager menuManager;

        DialogueManager dialogueManager;

        PlayerComponentManager playerComponentManager;

        CursorManager cursorManager;

        private void Awake()
        {
             menuManager = FindObjectOfType<MenuManager>(true);
            dialogueManager = FindObjectOfType<DialogueManager>(true);
            playerComponentManager = FindObjectOfType<PlayerComponentManager>(true);
            cursorManager = FindObjectOfType<CursorManager>(true);

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
            messageText = this.root.Q<Label>("MessageText");
            var actorSprite = this.root.Q<IMGUIContainer>("ActorSprite");
            #region Hint
            if (showHint)
            {
                this.root.Q<Label>("HintMessageText").text = pressKeyToContinueText.GetText();
                this.root.Q<VisualElement>("HintMessage").style.display = Gamepad.current != null ? DisplayStyle.None : DisplayStyle.Flex;
                this.root.Q<VisualElement>("HintMessageGamePad").style.display = Gamepad.current != null ? DisplayStyle.Flex : DisplayStyle.None;
            }

            this.root.Q<VisualElement>("Hint").style.display = showHint ? DisplayStyle.Flex : DisplayStyle.None;
            #endregion

            if (string.IsNullOrEmpty(actorName) == false)
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
                messageText.text = letter;
                yield return new WaitForSeconds(textDelay);
            }

            hasFinishedTypewriter = true;
        }

        public void ShowAllTextAtOnce()
        {
            StopAllCoroutines();
            hasFinishedTypewriter = true;

            messageText.text = dialogueText;
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
                cursorManager.ShowCursor();

                dialogueChoicePanel.style.display = DisplayStyle.Flex;

                DialogueChoice selectedChoice = null;

                Button elementToFocus = null;
                foreach (var dialogueChoice in dialogueChoices)
                {
                    var newDialogueChoiceItem = dialogueChoiceItem.CloneTree();
                    newDialogueChoiceItem.Q<Button>().text = EvaluateDialogueChoiceText(dialogueChoice.choiceText.GetText());

                    menuManager.SetupButton(newDialogueChoiceItem.Q<Button>(), () =>
                    {
                        selectedChoice = dialogueChoice;
                    });

                    if (elementToFocus == null)
                    {
                        elementToFocus = newDialogueChoiceItem.Q<Button>();
                    }

                    dialogueChoicePanel.Add(newDialogueChoiceItem);
                }
                elementToFocus.Focus();

                playerComponentManager.DisableCharacterController();
                playerComponentManager.DisableComponents();

                yield return new WaitUntil(() => selectedChoice != null);

                #region Reputation on response
                if (selectedChoice.reputationAmountToIncrease != 0 || selectedChoice.reputationAmountToDecrease != 0)
                {

                    if (SwitchManager.instance.GetSwitchCurrentValue(selectedChoice.reputationSwitchEntry) == false)
                    {
                        if (selectedChoice.reputationAmountToIncrease != 0)
                        {
                            FindObjectOfType<NotificationManager>(true).IncreaseReputation(selectedChoice.reputationAmountToIncrease);
                        }
                        else if (selectedChoice.reputationAmountToDecrease != 0)
                        {
                            FindObjectOfType<NotificationManager>(true).DecreaseReputation(selectedChoice.reputationAmountToDecrease);
                        }
                        SwitchManager.instance.UpdateSwitchWithoutRefreshingEvents(selectedChoice.reputationSwitchEntry, true);
                    }
                }
                #endregion

                cursorManager.HideCursor();

                #region Reeenable player movement


                playerComponentManager.EnableCharacterController();
                playerComponentManager.EnableComponents();
                #endregion

                // Use Sub Events Option
                if (selectedChoice.subEventPage != null)
                {
                    EventBase[] choiceEvents = selectedChoice.subEventPage.GetComponents<EventBase>();

                    if (choiceEvents.Length > 0)
                    {
                        foreach (EventBase subEvent in choiceEvents)
                        {
                            yield return subEvent.Dispatch();
                        }
                    }
                }
                else if (selectedChoice.response.localizedTexts.Count() > 0)
                {
                    yield return dialogueManager.ShowDialogueWithChoices(selectedChoice.replier, selectedChoice.response, new List<DialogueChoice>(), 0.05f, false);
                
                    if (selectedChoice.onResponseFinished != null)
                    {
                        selectedChoice.onResponseFinished.Invoke();
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
                var companion = companions.FirstOrDefault(x => x.companion.companionId.StartsWith(companionId));

                if (companion.waitingForPlayer)
                {
                    return LocalizedTerms.FollowMe();
                }
                else
                {
                    return LocalizedTerms.WaitHere();
                }
            }

            return text;
        }
    }
}
