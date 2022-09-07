using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_AddItem : EventBase
    {
        public Item item;

        public int amount = 1;

        public bool showNotificationText = true;

        public AudioClip pickUpSfx;

        public override IEnumerator Dispatch()
        {
            yield return StartCoroutine(AddItem());
        }

        IEnumerator AddItem()
        {
            PlayerInventoryManager.instance.AddItem(item, amount);

            if (pickUpSfx != null)
            {
                BGMManager.instance.PlaySound(pickUpSfx, null);
            }

            if (showNotificationText)
            {
                NotificationManager notificationManager = FindObjectOfType<NotificationManager>(true);
                if (notificationManager != null)
                {
                    notificationManager.ShowNotification("Cacildes received " + amount + "x " + item.name);
                }
            }

            yield return null;
        }
    }

}
