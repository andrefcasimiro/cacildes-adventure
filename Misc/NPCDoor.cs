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

        NotificationManager notificationManager;

        private void Awake()
        {
            notificationManager = FindObjectOfType<NotificationManager>(true);
        }

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
                shouldOpen = Player.instance.GetCurrentReputation() < 0;

                if (!shouldOpen)
                {
                    showNotification = true;
                }
            }
            else if (requireGoodOrNeutralReputation)
            {
                shouldOpen = Player.instance.GetCurrentReputation() >= 0;

                if (!shouldOpen)
                {
                    showNotification = true;
                }
            }

            if (showNotification)
            {
                Soundbank.instance.PlayUICancel();

                notificationManager.ShowNotification(LocalizedTerms.InsufficientReputationToEnterThisHouse(), notificationManager.personBusy);
            }

            if (shouldOpen)
            {
                close.SetActive(false);
                open.SetActive(true);
            }
        }
    }

}
