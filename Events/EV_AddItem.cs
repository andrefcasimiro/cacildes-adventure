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

        [Header("Conditions")]
        public SwitchEntry switchEntry;
        public bool switchValue;

        public override IEnumerator Dispatch()
        {
            bool skip = false;

            if (switchEntry != null)
            {
                // If depends on switch, evaluate value:
                ; if (SwitchManager.instance.GetSwitchCurrentValue(switchEntry) == switchValue)
                {
                    skip = false;
                }
                else
                {
                    skip = true;
                }
            }

            if (skip == false)
            {
                yield return StartCoroutine(AddItem());
            }
            else
            {
                yield return null;
            }
        }

        IEnumerator AddItem()
        {
            FindObjectOfType<PlayerInventory>(true).AddItem(item, amount);

            if (pickUpSfx != null)
            {
                BGMManager.instance.PlaySound(pickUpSfx, null);
            }

            if (showNotificationText)
            {
                Soundbank.instance.PlayItemReceived();
                FindObjectOfType<NotificationManager>(true).ShowNotification(LocalizedTerms.Found() + " x" + amount + " " + item.name.GetText() + "", item.sprite);
            }

            yield return null;
        }
    }

}
