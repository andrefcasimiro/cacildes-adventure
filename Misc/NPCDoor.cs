using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class NPCDoor : MonoBehaviour
    {
        public GameObject open;
        public GameObject close;

        public bool requireGoodOrNeutralReputation = true;
        public bool requireBadReputation = false;
        public bool noReputationRequired = false;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;

        [Header("Components")]
        public NotificationManager notificationManager;
        public Soundbank soundbank;


        public void ActivateDoor()
        {
            bool shouldOpen = false;
            bool showNotification = false;

            if (noReputationRequired)
            {
                shouldOpen = true;
            }
            else if (requireBadReputation)
            {
                shouldOpen = playerStatsDatabase.GetCurrentReputation() < 0;

                if (!shouldOpen)
                {
                    showNotification = true;
                }
            }
            else if (requireGoodOrNeutralReputation)
            {
                shouldOpen = playerStatsDatabase.GetCurrentReputation() >= 0;

                if (!shouldOpen)
                {
                    showNotification = true;
                }
            }

            if (showNotification)
            {
                soundbank.PlaySound(soundbank.uiCancel);

                notificationManager.ShowNotification("Your bad reputation prevents you from entering this house.", notificationManager.personBusy);
            }

            if (shouldOpen)
            {
                close.SetActive(false);
                open.SetActive(true);
            }
        }
    }

}
