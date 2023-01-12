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

        private void Start()
        {
            OnHourChanged();
        }

        public void OnHourChanged()
        {
            var dayTriggers = transform.GetComponentsInChildren<DayTrigger>(true);
            var nightTriggers = transform.GetComponentsInChildren<NightTrigger>(true);

            if (Player.instance.timeOfDay >= 5 && Player.instance.timeOfDay < 20)
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
            else if (Player.instance.timeOfDay >= 20 && Player.instance.timeOfDay <= 24 || Player.instance.timeOfDay >= 0 && Player.instance.timeOfDay < 5)
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

        public void OnGameLoaded(GameData gameData)
        {
            OnHourChanged();
        }
    }
}
