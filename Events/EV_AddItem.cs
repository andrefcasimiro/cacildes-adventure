using System.Collections;
using AF.Music;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{
    public class EV_AddItem : EventBase
    {
        [Header("Item To Add")]
        public Item item;
        public int amount = 1;


        [Header("Components")]
        public MonoBehaviourID monoBehaviourID;
        public Soundbank soundbank;
        public PlayerManager playerManager;

        [Header("Notifications")]
        public bool showNotificationText = true;
        public NotificationManager notificationManager;

        [Header("Databases")]
        public PickupDatabase pickupDatabase;

        public override IEnumerator Dispatch()
        {
            if (pickupDatabase.ContainsPickup(monoBehaviourID.ID))
            {
                yield break;
            }

            yield return StartCoroutine(AddItem());
        }

        IEnumerator AddItem()
        {
            playerManager.playerInventory.AddItem(item, amount);
            pickupDatabase.AddPickup(monoBehaviourID.ID, SceneManager.GetActiveScene().name + " - " + " - " + gameObject.name + " - " + item.name);

            if (showNotificationText)
            {
                soundbank.PlaySound(soundbank.uiItemReceived);
                notificationManager.ShowNotification("Found x" + amount + " " + item.name.GetEnglishText() + "", item.sprite);
            }

            yield return null;
        }
    }

}
