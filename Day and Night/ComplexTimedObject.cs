using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{

    public class ComplexTimedObject : MonoBehaviour, IClockListener, ISaveable
    {
        [Header("Systems")]
        public WorldSettings worldSettings;

        DawnTrigger[] dawnTriggers;
        DayTrigger[] dayTriggers;
        DuskTrigger[] duskTriggers;
        NightFallTrigger[] nightfallTriggers;
        NightTrigger[] nightTriggers;

        private void Start()
        {
            dawnTriggers = transform.GetComponentsInChildren<DawnTrigger>(true);
            dayTriggers = transform.GetComponentsInChildren<DayTrigger>(true);
            duskTriggers = transform.GetComponentsInChildren<DuskTrigger>(true);
            nightfallTriggers = transform.GetComponentsInChildren<NightFallTrigger>(true);
            nightTriggers = transform.GetComponentsInChildren<NightTrigger>(true);

            OnHourChanged();
        }

        public void OnHourChanged()
        {
            if (worldSettings.timeOfDay >= 7 && worldSettings.timeOfDay < 18f)
            {
                foreach (var trigger in dawnTriggers)
                {
                    trigger.gameObject.SetActive(false);
                }
                foreach (var trigger in dayTriggers)
                {
                    trigger.gameObject.SetActive(true);
                }
                foreach (var trigger in duskTriggers)
                {
                    trigger.gameObject.SetActive(false);
                }
                foreach (var trigger in nightfallTriggers)
                {
                    trigger.gameObject.SetActive(false);
                }
                foreach (var trigger in nightTriggers)
                {
                    trigger.gameObject.SetActive(false);
                }
            }
            else if (worldSettings.timeOfDay >= 18f && worldSettings.timeOfDay < 20)
            {
                foreach (var trigger in dawnTriggers)
                {
                    trigger.gameObject.SetActive(false);
                }
                foreach (var trigger in dayTriggers)
                {
                    trigger.gameObject.SetActive(false);
                }
                foreach (var trigger in duskTriggers)
                {
                    trigger.gameObject.SetActive(true);
                }
                foreach (var trigger in nightfallTriggers)
                {
                    trigger.gameObject.SetActive(false);
                }
                foreach (var trigger in nightTriggers)
                {
                    trigger.gameObject.SetActive(false);
                }
            }
            else if (worldSettings.timeOfDay >= 20 && worldSettings.timeOfDay < 22)
            {
                foreach (var trigger in dawnTriggers)
                {
                    trigger.gameObject.SetActive(false);
                }
                foreach (var trigger in dayTriggers)
                {
                    trigger.gameObject.SetActive(false);
                }
                foreach (var trigger in duskTriggers)
                {
                    trigger.gameObject.SetActive(false);
                }
                foreach (var trigger in nightfallTriggers)
                {
                    trigger.gameObject.SetActive(true);
                }
                foreach (var trigger in nightTriggers)
                {
                    trigger.gameObject.SetActive(false);
                }
            }
            else if (worldSettings.timeOfDay >= 22 && worldSettings.timeOfDay <= 24 || worldSettings.timeOfDay >= 0 && worldSettings.timeOfDay < 5)
            {
                foreach (var trigger in dawnTriggers)
                {
                    trigger.gameObject.SetActive(false);
                }
                foreach (var trigger in dayTriggers)
                {
                    trigger.gameObject.SetActive(false);
                }
                foreach (var trigger in duskTriggers)
                {
                    trigger.gameObject.SetActive(false);
                }
                foreach (var trigger in nightfallTriggers)
                {
                    trigger.gameObject.SetActive(false);
                }
                foreach (var trigger in nightTriggers)
                {
                    trigger.gameObject.SetActive(true);
                }
            }
            else if (worldSettings.timeOfDay >= 5 && worldSettings.timeOfDay < 7)
            {
                foreach (var trigger in dawnTriggers)
                {
                    trigger.gameObject.SetActive(true);
                }
                foreach (var trigger in dayTriggers)
                {
                    trigger.gameObject.SetActive(false);
                }
                foreach (var trigger in duskTriggers)
                {
                    trigger.gameObject.SetActive(false);
                }
                foreach (var trigger in nightfallTriggers)
                {
                    trigger.gameObject.SetActive(false);
                }
                foreach (var trigger in nightTriggers)
                {
                    trigger.gameObject.SetActive(false);
                }
            }

        }

        public void OnGameLoaded(object gameData)
        {
            OnHourChanged();
        }
    }
}
