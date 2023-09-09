using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF
{
    public class ClockDependant : MonoBehaviour, IClockListener, ISaveable
    {
        public enum ClockDependency
        {
            BETWEEN_RANGE,
            OUTSIDE_RANGE,
        }

        public int startHour;
        public int endHour;

        public ClockDependency clockDependency = ClockDependency.BETWEEN_RANGE;

        [Header("Switch Dependencies")]
        public SwitchEntry switchDependant;
        public bool requiredValue;

        [Header("Switch 2 Dependencies")]
        public SwitchEntry switch2Dependant;
        public bool switch2RequiredValue;

        [Header("Dont run this if Companion is in party")]
        public Companion companion;

        private void Start()
        {
            OnHourChanged();
        }

        public void OnHourChanged()
        {
            if (switchDependant != null)
            {
                if (SwitchManager.instance.GetSwitchCurrentValue(switchDependant) != requiredValue)
                {
                    transform.GetChild(0).gameObject.SetActive(false);
                    return;
                }
            }

            if (switch2Dependant != null)
            {
                if (SwitchManager.instance.GetSwitchCurrentValue(switch2Dependant) != switch2RequiredValue)
                {
                    transform.GetChild(0).gameObject.SetActive(false);
                    return;
                }
            }

            // If companion in party is set, we dont want to run the logic of day / night
            if (CompanionInParty())
            {
                transform.GetChild(0).gameObject.SetActive(true);
                return;
            }

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

        public void OnGameLoaded(GameData gameData)
        {
            OnHourChanged();
        }

        bool CompanionInParty()
        {
            return companion != null && Player.instance.companions.Exists(c => c.companionId == companion.companionId);
        }
    }

}
