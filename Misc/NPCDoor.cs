using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;

namespace AF
{
    public class NPCDoor : MonoBehaviour
    {
        public GameObject open;

        public bool requireGoodOrNeutralReputation = true;
        public bool requireBadReputation = false;
        public bool noReputationRequired = false;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;

        [Header("Components")]
        NotificationManager notificationManager;
        Soundbank soundbank;

        [Header("Events")]
        public UnityEvent onDoorOpen;

        Soundbank GetSoundbank()
        {
            if (soundbank == null)
            {
                soundbank = FindAnyObjectByType<Soundbank>(FindObjectsInactive.Include);
            }

            return soundbank;
        }

        NotificationManager GetNotificationManager()
        {
            if (notificationManager == null)
            {
                notificationManager = FindAnyObjectByType<NotificationManager>(FindObjectsInactive.Include);
            }

            return notificationManager;
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
                GetSoundbank().PlaySound(GetSoundbank().uiCancel);

                if (requireBadReputation)
                {
                    GetNotificationManager().ShowNotification(
                        LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Your good reputation prevents you from entering this house."),
                         GetNotificationManager().personBusy);
                }
                else
                {
                    GetNotificationManager().ShowNotification(
                        LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Your bad reputation prevents you from entering this house."),
                        GetNotificationManager().personBusy);

                }
            }

            if (shouldOpen)
            {
                open.SetActive(true);
                onDoorOpen?.Invoke();
            }
        }
    }

}
