using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Linq;
using DG.Tweening;
using UnityEngine.Events;
using AF.Dialogue;
using AF.Flags;

namespace AF
{
    public class UIDocumentDialogueWindow : MonoBehaviour
    {
        [Header("UI Documents")]
        public UIDocument uiDocument;
        public VisualTreeAsset dialogueChoiceItem;

        VisualElement root;
        VisualElement dialogueChoicePanel;

        // Flags
        [HideInInspector] public bool hasFinishedTypewriter = false;

        [Header("Settings")]
        public float textDelay = 0.05f;

        [Header("Components")]
        public StarterAssetsInputs inputs;
        public Soundbank soundbank;
        public CursorManager cursorManager;
        public PlayerManager playerManager;
        public FlagsDatabase flagsDatabase;

        [Header("Unity Events")]
        public UnityEvent onEnableEvent;
        public UnityEvent onDisableEvent;

        // Internal
        Label actorNameLabel, actorTitleLabel, messageTextLabel, pressToContinueLabel;
        IMGUIContainer actorSprite;
        VisualElement actorInfoContainer;

        private void Awake()
        {
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            this.root = uiDocument.rootVisualElement;
            this.dialogueChoicePanel = this.root.Q<VisualElement>("ChoiceContainer");
            this.actorNameLabel = this.root.Q<Label>("ActorName");
            this.actorTitleLabel = this.root.Q<Label>("ActorTitle");
            this.messageTextLabel = this.root.Q<Label>("MessageText");
            this.actorSprite = this.root.Q<IMGUIContainer>("ActorSprite");
            this.pressToContinueLabel = this.root.Q<Label>("PressToContinue");
            this.actorInfoContainer = this.root.Q<VisualElement>("ActorInfoContainer");

            onEnableEvent?.Invoke();
        }

        private void OnDisable()
        {
            onDisableEvent?.Invoke();
        }

        public IEnumerator DisplayMessage(
                    Character character,
                    string message,
                    Response[] responses
                )
        {
            gameObject.SetActive(true);

            ShowMessage(character, message);

            // Create a new copy to prevent mutation
            Response[] clonedResponses = responses.ToArray();

            yield return new WaitUntil(() => inputs.interact == false);

            while (hasFinishedTypewriter == false)
            {
                if (inputs.interact)
                {
                    ShowAllTextAtOnce(message);
                }

                yield return null;
            }

            yield return new WaitUntil(() => inputs.interact == false);
            yield return new WaitUntil(() => inputs.interact == true);

            if (clonedResponses != null && clonedResponses.Length > 0)
            {
                yield return ShowResponses(clonedResponses);
            }

            yield return new WaitUntil(() => inputs.interact == false);

            gameObject.SetActive(false);

        }

        private void ShowMessage(Character character, string message)
        {
            hasFinishedTypewriter = false;

            soundbank.PlaySound(soundbank.uiDialogue);

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

            dialogueChoicePanel.Clear();
            dialogueChoicePanel.style.display = DisplayStyle.None;

            if (Gamepad.current == null)
            {
                string oldText = pressToContinueLabel.text;
                pressToContinueLabel.text = "E) " + oldText;
            }

            if (character != null && string.IsNullOrEmpty(character.name) == false)
            {
                actorNameLabel.style.display = DisplayStyle.Flex;
                actorNameLabel.text = character.isPlayer ? playerManager.playerAppearance.GetPlayerName() : character.GetCharacterName();
                actorInfoContainer.style.display = DisplayStyle.Flex;
            }
            else
            {
                actorInfoContainer.style.display = DisplayStyle.None;
                actorNameLabel.style.display = DisplayStyle.None;
            }

            if (character != null && string.IsNullOrEmpty(character.title) == false)
            {
                actorTitleLabel.style.display = DisplayStyle.Flex;
                actorTitleLabel.text = character.GetCharacterTitle();
            }
            else
            {
                actorTitleLabel.style.display = DisplayStyle.None;
            }

            if (character != null && character.avatar != null)
            {
                actorSprite.style.backgroundImage = new StyleBackground(
                                   character.isPlayer ? playerManager.playerAppearance.GetPlayerPortrait() : character.avatar);
                actorSprite.style.display = DisplayStyle.Flex;
            }
            else
            {
                actorSprite.style.display = DisplayStyle.None;
            }

            StartCoroutine(Typewrite(message, messageTextLabel));
        }

        public IEnumerator Typewrite(string dialogueText, Label messageTextLabel)
        {

            for (int i = 0; i < dialogueText.Length + 1; i++)
            {
                var letter = dialogueText.Substring(0, i);
                messageTextLabel.text = letter;
                yield return new WaitForSeconds(textDelay);
            }

            hasFinishedTypewriter = true;
        }

        public void ShowAllTextAtOnce(string dialogueText)
        {
            StopAllCoroutines();
            hasFinishedTypewriter = true;
            messageTextLabel.text = dialogueText;
        }

        public IEnumerator ShowResponses(Response[] responses)
        {
            dialogueChoicePanel.Clear();
            dialogueChoicePanel.style.display = DisplayStyle.None;

            if (responses.Length <= 0)
            {
                dialogueChoicePanel.style.display = DisplayStyle.None;
                yield break;
            }

            dialogueChoicePanel.style.display = DisplayStyle.Flex;

            Response selectedResponse = null;
            Button elementToFocus = null;

            foreach (var response in responses)
            {
                var newDialogueChoiceItem = dialogueChoiceItem.CloneTree();
                newDialogueChoiceItem.Q<Button>().text = response.text;

                UIUtils.SetupButton(newDialogueChoiceItem.Q<Button>(), () =>
                {
                    cursorManager.HideCursor();
                    selectedResponse = response;
                    response.onResponseSelected?.Invoke();
                }, soundbank);

                elementToFocus ??= newDialogueChoiceItem.Q<Button>();

                dialogueChoicePanel.Add(newDialogueChoiceItem);
            }

            cursorManager.ShowCursor();
            elementToFocus?.Focus();
            playerManager.thirdPersonController.LockCameraPosition = true;

            yield return new WaitUntil(() => selectedResponse != null);

            playerManager.thirdPersonController.LockCameraPosition = false;

            selectedResponse.AwardReputation(flagsDatabase, playerManager.playerReputation);

            // Use Sub Events Option
            if (selectedResponse.subEventPage != null)
            {
                EventBase[] choiceEvents = selectedResponse.subEventPage.GetComponents<EventBase>();

                if (choiceEvents.Length > 0)
                {
                    foreach (EventBase subEvent in choiceEvents)
                    {
                        yield return subEvent.Dispatch();
                    }
                }
            }
            else if (string.IsNullOrEmpty(selectedResponse.reply) == false)
            {
                yield return DisplayMessage(selectedResponse.replier, selectedResponse.reply, new Response[] { });
            }

            selectedResponse.onResponseFinished?.Invoke();
        }
    }
}
