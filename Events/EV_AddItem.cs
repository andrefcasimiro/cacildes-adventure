﻿using System.Collections;
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
        public string switchUuid;
        public bool switchValue;

        public override IEnumerator Dispatch()
        {
            bool skip = false;

            if (System.String.IsNullOrEmpty(switchUuid) == false)
            {
                // If depends on switch, evaluate value:
                ; if (SwitchManager.instance.GetSwitchValue(switchUuid) == switchValue)
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
                BGMManager.instance.PlayItem();
                FindObjectOfType<NotificationManager>(true).ShowNotification("Found x" + amount + " " + item.name + "", item.sprite);
            }

            yield return null;
        }
    }

}
