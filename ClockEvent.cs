using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{

    public class ClockEvent : MonoBehaviour
    {
        public string eventName;

        [Tooltip("05-07")]
        public EventPage dawnEventPage;

        [Tooltip("07-18")]
        public EventPage dayEventPage;

        [Tooltip("18-20")]
        public EventPage duskEventPage;

        [Tooltip("20-22")]
        public EventPage nightfallEventPage;

        [Tooltip("22-05")]
        public EventPage nightEventPage;

        [Tooltip("If true, will use dayEventPage for 06-20 and nightEventPage from 20-06")]
        public bool useSimpleDayAndNight = false;

        private void Update()
        {
            if (useSimpleDayAndNight)
            {
                if (Player.instance.timeOfDay >= 6 && Player.instance.timeOfDay < 20)
                {
                    dayEventPage.gameObject.SetActive(true);
                    nightEventPage.gameObject.SetActive(false);
                }
                else if (Player.instance.timeOfDay >= 20 && Player.instance.timeOfDay <= 24 || Player.instance.timeOfDay >= 0 && Player.instance.timeOfDay < 6)
                {
                    dayEventPage.gameObject.SetActive(false);
                    nightEventPage.gameObject.SetActive(true);
                }

                return;
            }

            if (Player.instance.timeOfDay >= 7 && Player.instance.timeOfDay < 18f)
            {
                dawnEventPage.gameObject.SetActive(false);
                dayEventPage.gameObject.SetActive(true);
                duskEventPage.gameObject.SetActive(false);
                nightfallEventPage.gameObject.SetActive(false);
                nightEventPage.gameObject.SetActive(false);
            }
            else if (Player.instance.timeOfDay >= 18f && Player.instance.timeOfDay < 20)
            {
                dawnEventPage.gameObject.SetActive(false);
                dayEventPage.gameObject.SetActive(false);
                duskEventPage.gameObject.SetActive(true);
                nightfallEventPage.gameObject.SetActive(false);
                nightEventPage.gameObject.SetActive(false);
            }
            else if (Player.instance.timeOfDay >= 20 && Player.instance.timeOfDay < 22)
            {
                dawnEventPage.gameObject.SetActive(false);
                dayEventPage.gameObject.SetActive(false);
                duskEventPage.gameObject.SetActive(false);
                nightfallEventPage.gameObject.SetActive(true);
                nightEventPage.gameObject.SetActive(false);
            }
            else if (Player.instance.timeOfDay >= 22 && Player.instance.timeOfDay <= 24 || Player.instance.timeOfDay >= 0 && Player.instance.timeOfDay < 5)
            {
                dawnEventPage.gameObject.SetActive(false);
                dayEventPage.gameObject.SetActive(false);
                duskEventPage.gameObject.SetActive(false);
                nightfallEventPage.gameObject.SetActive(false);
                nightEventPage.gameObject.SetActive(true);
            }
            else if (Player.instance.timeOfDay >= 5 && Player.instance.timeOfDay < 7)
            {
                dawnEventPage.gameObject.SetActive(true);
                dayEventPage.gameObject.SetActive(false);
                duskEventPage.gameObject.SetActive(false);
                nightfallEventPage.gameObject.SetActive(false);
                nightEventPage.gameObject.SetActive(false);
            }
        }

        public void RefreshEventPages()
        {
        }
    }

}
