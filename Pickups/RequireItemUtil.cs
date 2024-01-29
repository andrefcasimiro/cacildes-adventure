using AF.Flags;
using AF.Inventory;
using AF.Music;
using UnityEngine;
using UnityEngine.Events;

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

        [Header("Notifications")]
        public NotificationManager notificationManager;
        public Soundbank soundbank;

        [Header("Flags")]
        public MonoBehaviourID monoBehaviourID;
        public FlagsDatabase flagsDatabase;

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

            notificationManager.ShowNotification(
                requiredItem.name + " was lost with use.",
                notificationManager.systemError
            );

            soundbank.PlaySound(soundbank.itemLostWithUse);

            inventoryDatabase.RemoveItem(requiredItem, 1);
            onItemUsed?.Invoke();
            flagsDatabase.AddFlag(monoBehaviourID);
        }
    }
}
