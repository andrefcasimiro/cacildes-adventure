
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class NotificationManager : MonoBehaviour, ISaveable
    {
        public const float MAX_ITEM_DURATION = 5f;
        public const float MAX_ITEMS_IN_QUEUE = 5f;

        [Header("Icons")]
        public Sprite systemSuccess;
        public Sprite systemError;
        public Sprite alchemyLackOfIngredients;
        public Sprite recipeIcon;
        public Sprite door;
        public Sprite levelUp;

        public UIDocument uiDocumentNotificationUI;

        public VisualTreeAsset notificationItemPrefab;

        VisualElement root;
        VisualElement notificationsPanel;
        float timePassed = 0f;

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
            if (this.notificationsPanel.childCount >= MAX_ITEMS_IN_QUEUE)
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

        public void OnGameLoaded(GameData gameData)
        {
            this.notificationsPanel.Clear();
        }
    }
}