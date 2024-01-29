using System.Collections;
using AF.Music;
using UnityEngine;

namespace AF
{

    public class EV_RemoveItem : EventBase
    {
        public Item item;

        public int amount = 1;

        public bool showNotificationText = true;

        public AudioClip pickUpSfx;

        [Header("Components")]
        public BGMManager bgmManager;
        public Soundbank soundbank;
        public NotificationManager notificationManager;
        public PlayerManager playerManager;

        public override IEnumerator Dispatch()
        {
            yield return StartCoroutine(RemoveItem());
        }

        IEnumerator RemoveItem()
        {
            playerManager.playerInventory.RemoveItem(item, amount);

            if (pickUpSfx != null)
            {
                bgmManager.PlaySound(pickUpSfx, null);
            }

            if (showNotificationText)
            {
                soundbank.PlaySound(soundbank.uiCancel);

                notificationManager.ShowNotification("Used x" + amount + " " + item.name + "", item.sprite);
            }

            yield return null;
        }
    }

}
