using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF
{
    public class ClockDependant : MonoBehaviour
    {
        public enum ClockDependency
        {
            BETWEEN_RANGE,
            OUTSIDE_RANGE,
        }

        public int startHour;
        public int endHour;

        public ClockDependency clockDependency = ClockDependency.BETWEEN_RANGE;


        [Header("Systems")]
        public WorldSettings worldSettings;

        private void Awake()
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        private void Start()
        {
            OnHourChanged();

            EventManager.StartListening(EventMessages.ON_HOUR_CHANGED, OnHourChanged);
        }

        public void OnHourChanged()
        {
            bool isActive;

            // If appear until is after midnight, it may become smaller than appearFrom (i. e. appear from 17 until 4)
            if (startHour > endHour)
            {
                isActive = worldSettings.timeOfDay >= startHour && worldSettings.timeOfDay <= 24 || (worldSettings.timeOfDay >= 0 && worldSettings.timeOfDay <= endHour);
            }
            else
            {
                isActive = worldSettings.timeOfDay >= startHour && worldSettings.timeOfDay <= endHour;
            }

            if (clockDependency == ClockDependency.OUTSIDE_RANGE)
            {
                isActive = !isActive;
            }

            Utils.UpdateTransformChildren(transform, isActive);
        }
    }
}
