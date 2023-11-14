using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{

    public class TimedObject : MonoBehaviour, IClockListener, ISaveable
    {
        public string dayHours = "05-20";
        public string nightHours = "20-5";
        [Header("Systems")]
        public WorldSettings worldSettings;

        private void Start()
        {
            OnHourChanged();
        }

        public void OnHourChanged()
        {
            var dayTriggers = transform.GetComponentsInChildren<DayTrigger>(true);
            var nightTriggers = transform.GetComponentsInChildren<NightTrigger>(true);

            if (worldSettings.timeOfDay >= 5 && worldSettings.timeOfDay < 20)
            {
                if (dayTriggers.Length > 0)
                {
                    foreach (var dayTrigger in dayTriggers)
                    {
                        dayTrigger.gameObject.SetActive(true);
                    }
                }

                if (nightTriggers.Length > 0)
                {
                    foreach (var nightTrigger in nightTriggers)
                    {
                        nightTrigger.gameObject.SetActive(false);
                    }
                }
            }
            else if (worldSettings.timeOfDay >= 20 && worldSettings.timeOfDay <= 24 || worldSettings.timeOfDay >= 0 && worldSettings.timeOfDay < 5)
            {
                if (dayTriggers.Length > 0)
                {
                    foreach (var dayTrigger in dayTriggers)
                    {
                        dayTrigger.gameObject.SetActive(false);
                    }
                }

                if (nightTriggers.Length > 0)
                {
                    foreach (var nightTrigger in nightTriggers)
                    {
                        nightTrigger.gameObject.SetActive(true);
                    }

                }
            }

        }

        public void OnGameLoaded(object gameData)
        {
            OnHourChanged();
        }
    }
}
