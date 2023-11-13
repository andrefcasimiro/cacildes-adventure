using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Linq;
using DG.Tweening;
using System;
using System.IO;
using UnityEngine.Video;

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


        DialogueManager dialogueManager;

        PlayerComponentManager playerComponentManager;

        CursorManager cursorManager;

        public GameObject videoCanvasManager;

        [Header("Show Image")]
        public Texture2D image;

        [Header("Show Video")]
        public VideoPlayer video;

        [Header("Components")]
        public UIManager uiManager;

        private void Awake()
        {
            dialogueManager = FindObjectOfType<DialogueManager>(true);
            playerComponentManager = FindObjectOfType<PlayerComponentManager>(true);
            cursorManager = FindObjectOfType<CursorManager>(true);

            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            hasFinishedTypewriter = false;

            this.root = uiDocument.rootVisualElement;

            Soundbank.instance.PlayUIDialogue();

            DOTween.To(
                  () => root.contentContainer.style.opacity.value,
                  (value) => root.contentContainer.style.opacity = value,
                  1,
                  .05f
            );

            // Set the starting position below the screen
            Vector3 startPosition = new Vector3(root.contentContainer.transform.position.x, root.contentContainer.transform.position.y - 10, root.contentContainer.transform.position.z);

            // Set the ending position (original position)
            Vector3 endPosition = root.contentContainer.transform.position;

            // Tween the position from the starting position to the ending position
            DOTween.To(() => startPosition, position => root.contentContainer.transform.position = position, endPosition, 0.5f);

            this.root.Q<VisualElement>("ImageContainer").style.display = DisplayStyle.None;

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

                if (Gamepad.current != null)
                {
                    this.root.Q<Label>("HintMessageTextGamepad").text = GamePreferences.instance.IsEnglish() ? "Press to continue" : "Pressiona para continuar";
                }

            }

            this.root.Q<VisualElement>("Hint").style.display = showHint ? DisplayStyle.Flex : DisplayStyle.None;
            #endregion


            this.root.Q<Label>("HintMessageText").text = pressKeyToContinueText.GetText();
            this.root.Q<IMGUIContainer>("GamepadIcon").style.display = Gamepad.current != null ? DisplayStyle.Flex : DisplayStyle.None;

            if (Gamepad.current != null)
            {
                this.root.Q<Label>("PressToContinue").text = GamePreferences.instance.IsEnglish() ? "Speed up / Continue" : "Acelerar / Continuar";
            }
            else
            {
                this.root.Q<Label>("PressToContinue").text = GamePreferences.instance.IsEnglish() ? "E) Speed up / Continue" : "E) Acelerar / Continuar";
            }

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


            if (image != null)
            {
                this.root.Q<VisualElement>("ImageContainer").style.display = DisplayStyle.Flex;
                this.root.Q<VisualElement>("ImageContainer").Q<IMGUIContainer>().style.backgroundImage = new StyleBackground(image);
                this.root.Q<VisualElement>("ImageContainer").Q<IMGUIContainer>().style.width = image.width / 2;
                this.root.Q<VisualElement>("ImageContainer").Q<IMGUIContainer>().style.height = image.height / 2;
            }

            if (video != null)
            {
                video.gameObject.SetActive(true);

                CanvasGroup canvasGroup = videoCanvasManager.GetComponent<CanvasGroup>();
                canvasGroup.alpha = 0f;

                videoCanvasManager.gameObject.SetActive(true);
                canvasGroup.DOFade(1f, 0.5f);


            }

            StartCoroutine(Typewrite());
        }


        private void Update()
        {
            if (dialogueChoices.Count > 0)
            {
                cursorManager.ShowCursor();
            }
        }

        private void OnDisable()
        {
            videoCanvasManager.SetActive(false);

            if (video != null)
            {
                video.gameObject.SetActive(false);
                video = null;
            }

            cursorManager.HideCursor();
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

                    UIUtils.SetupButton(newDialogueChoiceItem.Q<Button>(), () =>
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

                playerComponentManager.GetComponent<ThirdPersonController>().LockCameraPosition = true;

                yield return new WaitUntil(() => selectedChoice != null);

                playerComponentManager.GetComponent<ThirdPersonController>().LockCameraPosition = false;

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
                    yield return dialogueManager.ShowDialogueWithChoices(selectedChoice.replier, selectedChoice.response, new List<DialogueChoice>(), 0.05f, false, null, null);

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
