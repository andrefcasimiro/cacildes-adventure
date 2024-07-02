using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;

namespace AF.Misc
{
    public class VariableManager : MonoBehaviour
    {
        public int counter;
        public int maxCounter;

        [Header("Notifications")]
        public bool displayNotification = true;
        public NotificationManager notificationManager;
        public Sprite notificationSprite;
        public LocalizedString notificationSuffixText;

        [Header("Events")]
        public UnityEvent onCounterIncreased;
        public UnityEvent onCounterMaxed;

        bool hasReachedLimit = false;

        public void IncreaseCounter()
        {
            if (hasReachedLimit)
            {
                return;
            }

            counter++;

            UpdateCounter();
        }

        void UpdateCounter()
        {
            if (hasReachedLimit)
            {
                return;
            }

            if (displayNotification)
            {
                notificationManager.ShowNotification($"{counter}/{maxCounter}{notificationSuffixText.GetLocalizedString()}", notificationSprite);
            }

            onCounterIncreased?.Invoke();

            if (counter >= maxCounter)
            {
                hasReachedLimit = true;
                onCounterMaxed?.Invoke();
            }
        }
    }
}
