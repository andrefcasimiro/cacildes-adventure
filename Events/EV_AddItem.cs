using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

namespace AF
{
    public class EV_AddItem : EventBase
    {
        [System.Serializable]
        public class ItemToAdd
        {
            public Item item;
            public int amount;
            [Range(0, 100f)] public float chance = 100f;
        }

        [Header("Item To Add")]
        public Item item;
        public int amount = 1;

        [Header("Multiple Items To Add - Optional")]

        public ItemToAdd[] itemsToAdd;

        [Header("Components (Optional)")]
        public MonoBehaviourID monoBehaviourID;
        Soundbank _soundbank;
        PlayerManager _playerManager;

        [Header("Notifications")]
        public bool showNotificationText = true;
        NotificationManager _notificationManager;

        [Header("Databases")]
        public PickupDatabase pickupDatabase;


        Soundbank GetSoundbank()
        {
            if (_soundbank == null)
            {
                _soundbank = FindAnyObjectByType<Soundbank>(FindObjectsInactive.Include);
            }

            return _soundbank;
        }

        PlayerManager GetPlayerManager()
        {
            if (_playerManager == null)
            {
                _playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include);
            }

            return _playerManager;
        }

        NotificationManager GetNotificationManager()
        {
            if (_notificationManager == null)
            {
                _notificationManager = FindAnyObjectByType<NotificationManager>(FindObjectsInactive.Include);
            }

            return _notificationManager;
        }

        public override IEnumerator Dispatch()
        {
            if (monoBehaviourID != null && pickupDatabase.ContainsPickup(monoBehaviourID.ID))
            {
                yield break;
            }

            yield return StartCoroutine(AddItem());
        }

        IEnumerator AddItem()
        {
            if (itemsToAdd != null && itemsToAdd.Length > 0)
            {
                foreach (var itemToAdd in itemsToAdd)
                {
                    if (Random.Range(0, 100) <= itemToAdd.chance)
                    {
                        GetPlayerManager().playerInventory.AddItem(itemToAdd.item, itemToAdd.amount);
                        NotifyItem(itemToAdd.item, itemToAdd.amount);
                    }
                }
            }
            else
            {
                GetPlayerManager().playerInventory.AddItem(item, amount);
                NotifyItem(item, amount);
            }

            if (monoBehaviourID != null && pickupDatabase != null)
            {
                pickupDatabase.AddPickup(monoBehaviourID.ID, SceneManager.GetActiveScene().name + " - " + " - " + gameObject.name + " - " + (item != null ? item.name : ""));
            }

            yield return null;
        }

        void NotifyItem(Item _item, int _amount)
        {
            if (showNotificationText)
            {
                GetSoundbank().PlaySound(GetSoundbank().uiItemReceived);
                GetNotificationManager().ShowNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Found x") + _amount + " " + _item.GetName() + "", _item.sprite);
            }
        }
    }

}
