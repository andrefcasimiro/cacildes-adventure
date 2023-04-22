using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_RemoveItem : EventBase
    {
        public Item item;

        public int amount = 1;

        public bool showNotificationText = true;

        public AudioClip pickUpSfx;

        public override IEnumerator Dispatch()
        {
            yield return StartCoroutine(RemoveItem());
        }

        IEnumerator RemoveItem()
        {
            FindObjectOfType<PlayerInventory>(true).RemoveItem(item, amount);

            if (pickUpSfx != null)
            {
                BGMManager.instance.PlaySound(pickUpSfx, null);
            }

            if (showNotificationText)
            {
                Soundbank.instance.PlayUICancel();
                FindObjectOfType<NotificationManager>(true).ShowNotification(LocalizedTerms.Used() + " x" + amount + " " + item.name.GetText() + "", item.sprite);
            }

            yield return null;
        }
    }

}
