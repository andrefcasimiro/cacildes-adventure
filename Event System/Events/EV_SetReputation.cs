using System.Collections;
using UnityEngine;

namespace AF
{
    public enum SetReputation
    {
        INCREASE,
        DECREASE,
        SET
    }

    public class EV_SetReputation : EventBase
    {
        public SetReputation setReputation;

        public int amount = 1;

        public bool showNotificationText = true;

        public AudioClip sfx;

        public override IEnumerator Dispatch()
        {
            if (setReputation == SetReputation.SET) {
                PlayerStatsManager.instance.currentReputation = amount;
            }
            else if (setReputation == SetReputation.DECREASE)
            {
                PlayerStatsManager.instance.currentReputation -= amount;

                if (showNotificationText)
                {
                    NotificationManager notificationManager = FindObjectOfType<NotificationManager>(true);
                    if (notificationManager != null)
                    {
                        if (amount <= 1)
                        {
                            notificationManager.ShowNotification("Cacildes lost " + amount + " point of reputation");
                        }
                        else
                        {
                            notificationManager.ShowNotification("Cacildes lost " + amount + " points of reputation");
                        }
                    }
                }
            }
            else
            {
                PlayerStatsManager.instance.currentReputation += amount;
                if (showNotificationText)
                {
                    NotificationManager notificationManager = FindObjectOfType<NotificationManager>(true);
                    if (notificationManager != null)
                    {
                        if (amount <= 1)
                        {
                            notificationManager.ShowNotification("Cacildes gained " + amount + " point of reputation");
                        }
                        else
                        {
                            notificationManager.ShowNotification("Cacildes gained " + amount + " points of reputation");
                        }
                    }
                }
            }


            if (sfx != null)
            {
                BGMManager.instance.PlaySound(sfx, null);
            }

            yield return null;
        }
    }

}
