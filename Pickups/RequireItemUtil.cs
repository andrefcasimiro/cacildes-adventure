using AF.Flags;
using AF.Inventory;
using AF.Music;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;

namespace AF.Pickups
{
    public class RequireItemUtil : MonoBehaviour
    {
        [Header("Events")]
        public UnityEvent onItemUsed;
        public UnityEvent onItemMissing;
        public UnityEvent onAwake_ItemAlreadyUsed;

        [Header("Required Item")]
        public Item requiredItem;

        [Header("Databases")]
        public InventoryDatabase inventoryDatabase;


        [Header("Flags")]
        public MonoBehaviourID monoBehaviourID;
        public FlagsDatabase flagsDatabase;

        // Scene Refs
        NotificationManager notificationManager;
        Soundbank soundbank;

        private void Awake()
        {
            if (flagsDatabase.ContainsFlag(monoBehaviourID.ID))
            {
                onAwake_ItemAlreadyUsed?.Invoke();
            }
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void Unlock()
        {
            if (!inventoryDatabase.HasItem(requiredItem))
            {
                onItemMissing?.Invoke();
                return;
            }

            GetNotificationManager().ShowNotification(
                requiredItem.GetName() + " " + LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "was lost with its use."),
                GetNotificationManager().systemError
            );

            GetSoundbank().PlaySound(GetSoundbank().itemLostWithUse);

            inventoryDatabase.RemoveItem(requiredItem, 1);
            onItemUsed?.Invoke();
            flagsDatabase.AddFlag(monoBehaviourID);
        }

        Soundbank GetSoundbank()
        {
            if (soundbank == null)
            {
                soundbank = FindAnyObjectByType<Soundbank>(FindObjectsInactive.Include);
            }

            return soundbank;
        }

        NotificationManager GetNotificationManager()
        {
            if (notificationManager == null)
            {
                notificationManager = FindAnyObjectByType<NotificationManager>(FindObjectsInactive.Include);
            }

            return notificationManager;
        }
    }
}
