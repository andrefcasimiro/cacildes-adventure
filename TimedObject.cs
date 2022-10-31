using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{

    public class TimedObject : MonoBehaviour, IClockListener, ISaveable
    {

        DayTrigger[] dayTriggers;
        NightTrigger[] nightTriggers;

        private void Start()
        {
            this.dayTriggers = transform.GetComponentsInChildren<DayTrigger>(true);
            this.nightTriggers = transform.GetComponentsInChildren<NightTrigger>(true);

            OnHourChanged();
        }

        public void OnHourChanged()
        {
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
