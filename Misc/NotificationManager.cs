using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class NotificationManager : MonoBehaviour, ISaveable
    {
        public const float MAX_ITEM_DURATION = 4f;
        public const float MAX_ITEMS_IN_QUEUE = 5;

        [Header("Icons")]
        public Sprite systemSuccess;
        public Sprite systemError;
        public Sprite alchemyLackOfIngredients;
        public Sprite recipeIcon;
        public Sprite door;
        public Sprite levelUp;
        public Sprite personBusy;
        public Sprite notEnoughSpells;

        public Sprite reputationIncreaseSprite;
        public Sprite reputationDecreaseSprite;

        public UIDocument uiDocumentNotificationUI;

        public VisualTreeAsset notificationItemPrefab;

        VisualElement root;
        VisualElement notificationsPanel;
        float timePassed = 0f;

        [Header("Achievements")]
        public Achievement negativeReputationAchievement;

        private void Start()
        {
            this.root = uiDocumentNotificationUI.rootVisualElement;
            this.root.pickingMode = PickingMode.Ignore;
            this.root.focusable = false;
            notificationsPanel = this.root.Q<VisualElement>("NotificationContainer");
            notificationsPanel.pickingMode = PickingMode.Ignore;
            notificationsPanel.focusable = false;

            notificationsPanel.Clear();
        }

        public void Update()
        {
            if (notificationsPanel.childCount <= 0)
            {
                return;
            }

            if (timePassed >= MAX_ITEM_DURATION)
            {
                notificationsPanel.RemoveAt(0);

                timePassed = 0f;
            }

            timePassed += Time.deltaTime;
        }

        public void ShowNotification(string message, Sprite sprite)
        {
            // Delete last entry
            if (this.notificationsPanel != null && this.notificationsPanel.childCount >= MAX_ITEMS_IN_QUEUE)
            {
                this.notificationsPanel.RemoveAt(0);
            }

            VisualElement notificationInstance = notificationItemPrefab.CloneTree();
            notificationInstance.Q<Label>().text = message;

            if (sprite == null)
            {
                notificationInstance.Q<IMGUIContainer>().style.display = DisplayStyle.None;
            }
            else
            {
                notificationInstance.Q<IMGUIContainer>().style.backgroundImage = new StyleBackground(sprite);
                notificationInstance.Q<IMGUIContainer>().style.display = DisplayStyle.Flex;
            }

            this.notificationsPanel.Add(notificationInstance);
            timePassed = 0f;
        }

        // TODO: This should be on a proper class
        public void IncreaseReputation(int value)
        {
            Player.instance.currentReputation += value;

            Soundbank.instance.PlayReputationIncreased();
            ShowNotification(LocalizedTerms.ReputationIncreased(value), reputationIncreaseSprite);
        }

        // TODO: This should be on a proper class
        public void DecreaseReputation(int value)
        {
            Player.instance.currentReputation -= value;

            if (Player.instance.currentReputation <= -25f)
            {
                negativeReputationAchievement.AwardAchievement();
            }

            Soundbank.instance.PlayReputationDecreased();
            ShowNotification(LocalizedTerms.ReputationDecreased(value), reputationDecreaseSprite);
        }

        public void OnGameLoaded(object gameData)
        {
            this.notificationsPanel.Clear();
        }
    }
}