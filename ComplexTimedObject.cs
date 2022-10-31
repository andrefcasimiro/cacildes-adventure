using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{

    public class ComplexTimedObject : MonoBehaviour, IClockListener, ISaveable
    {

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
            if (Player.instance.timeOfDay >= 7 && Player.instance.timeOfDay < 18f)
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
            else if (Player.instance.timeOfDay >= 18f && Player.instance.timeOfDay < 20)
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
            else if (Player.instance.timeOfDay >= 20 && Player.instance.timeOfDay < 22)
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
            else if (Player.instance.timeOfDay >= 22 && Player.instance.timeOfDay <= 24 || Player.instance.timeOfDay >= 0 && Player.instance.timeOfDay < 5)
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
            else if (Player.instance.timeOfDay >= 5 && Player.instance.timeOfDay < 7)
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

        public void OnGameLoaded(GameData gameData)
        {
            OnHourChanged();
        }
    }
}
