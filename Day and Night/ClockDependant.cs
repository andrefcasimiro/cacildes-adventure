using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class ClockDependant : MonoBehaviour, IClockListener
    {
        public enum ClockDependency
        {
            BETWEEN_RANGE,
            OUTSIDE_RANGE,
        }

        public int startHour;
        public int endHour;

        public ClockDependency clockDependency = ClockDependency.BETWEEN_RANGE;

        private void Start()
        {
            OnHourChanged();
        }

        public void OnHourChanged()
        {
            bool isActive = false;

            // If appear until is after midnight, it may become smaller than appearFrom (i. e. appear from 17 until 4)
            if (startHour > endHour)
            {
                isActive = Player.instance.timeOfDay >= startHour && Player.instance.timeOfDay <= 24 || (Player.instance.timeOfDay >= 0 && Player.instance.timeOfDay <= endHour);
            }
            else
            {
                isActive = Player.instance.timeOfDay >=  startHour && Player.instance.timeOfDay <= endHour;
            }

            if (clockDependency == ClockDependency.OUTSIDE_RANGE)
            {
                isActive = !isActive;
            }

            if (transform.childCount > 0)
            {
                transform.GetChild(0).gameObject.SetActive(isActive);
            }

        }

    }

}
