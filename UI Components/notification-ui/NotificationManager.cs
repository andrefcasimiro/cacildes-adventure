using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF
{
    public class NotificationManager : MonoBehaviour, ISaveable
    {
        public const float MAX_ITEM_DURATION = 5f;
        public const float MAX_ITEMS_IN_QUEUE = 5f;

        bool isActive = false;

        float timePassed = 0f;

        public UIDocumentNotificationUI uiDocumentNotificationUI;

        private void Start()
        {
            this.uiDocumentNotificationUI = GetComponent<UIDocumentNotificationUI>();
        }

        public void Update()
        {
            if (!isActive) { return; }

            if (uiDocumentNotificationUI.notificationInstances.Count <= 0)
            {
                timePassed = 0f;
                isActive = false;
                return;
            }

            if (timePassed >= MAX_ITEM_DURATION)
            {
                var notificationEntryToDestroy = this.uiDocumentNotificationUI.notificationInstances.First();
                this.uiDocumentNotificationUI.notificationInstances.Remove(notificationEntryToDestroy);
                
                this.uiDocumentNotificationUI.Refresh();

                timePassed = 0f;
            }

            timePassed += Time.deltaTime;

        }

        public void ShowNotification(string message)
        {
            // Delete last entry
            if (this.uiDocumentNotificationUI.notificationInstances.Count >= MAX_ITEMS_IN_QUEUE)
            {
                var notificationEntryToDestroy = this.uiDocumentNotificationUI.notificationInstances.First();
                this.uiDocumentNotificationUI.notificationInstances.Remove(notificationEntryToDestroy);
            }

            this.uiDocumentNotificationUI.notificationInstances.Add(message);

            this.uiDocumentNotificationUI.Refresh();

            Wake();
        }

        void Wake()
        {
            if (isActive == false)
            {
                isActive = true;
                timePassed = 0f;
            }
        }

        public void OnGameLoaded(GameData gameData)
        {
            this.uiDocumentNotificationUI.notificationInstances.Clear();

            this.uiDocumentNotificationUI.Refresh();
        }
    }
}
