using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using System;

namespace AF
{

    public class Event : MonoBehaviour, IClockListener, ISaveable
    {
        public string eventName;

        public MoveRoute moveRoute;

        [HideInInspector] public EventPage[] eventPages;

        [HideInInspector] public EventPage currentTarget;

        private void Awake()
        {
            EventPage[] eventPages = transform.GetComponentsInChildren<EventPage>(true);

            this.eventPages = eventPages;
        }

        private void Start()
        {
            RefreshEventPages();

            OnHourChanged();
        }

        public void RefreshEventPages()
        {
            EventPage previousEventPage = null;

            try
            {

                if (transform != null && transform.childCount > 0) {
                    // Deactivate all event pages
                    foreach (EventPage evt in transform.GetComponentsInChildren<EventPage>(true))
                    {
                        if (evt.gameObject.activeSelf)
                        {
                            previousEventPage = evt;
                        }

                        evt.gameObject.SetActive(false);
                    }

                } else
                {
                    Debug.Log("Transform child count is zero for:" + this.gameObject.name);
                }
                
            } catch (Exception e)
            {
                Debug.Log("Exception occurred in event: " + this.gameObject.name);
                Debug.Log("Exception:" + e.Message);
            }

            // Find last event page where all switch conditions are met
            EventPage target = eventPages.LastOrDefault(evtPage =>
            {
                bool allSwitchConditions = evtPage.switchConditions.All(
                    // Evaluate if switch value equals the current value on the switch manager database
                    pageSwitch => (pageSwitch.value == SwitchManager.instance.GetSwitchCurrentValue(pageSwitch.switchEntryCondition))
                    );

                if (allSwitchConditions)
                {
                    return true;
                }

                return false;
            });

            // No target found?
            // Activate the first page, as long as there are no switch conditions
            if (target == null && eventPages.Length > 0 && eventPages[0].switchConditions.Length <= 0)
            {
                eventPages[0].gameObject.SetActive(true);

                currentTarget = eventPages[0];
            }

            else if (target != null)
            {
                target.gameObject.SetActive(true);

                currentTarget = target;
            }

            if (target != null)
            {
                MoveRoute moveRoute = this.GetComponentInChildren<MoveRoute>();
                if (moveRoute != null)
                {
                    // Position new event page graphic in the same position of the previous event page to make the transition seamless
                    if (previousEventPage != null && target != null)
                    {
                        Transform previousEventPageTargetGraphic = previousEventPage.GetComponentInChildren<Animator>().transform;

                        Animator targetGraphic = target.GetComponentInChildren<Animator>();

                        if (targetGraphic != null)
                        {
                            Transform targetGraphicTransform = targetGraphic.transform;

                            if (targetGraphicTransform != null)
                            {
                                targetGraphic.transform.position = previousEventPageTargetGraphic.transform.position;
                                targetGraphic.transform.rotation = previousEventPageTargetGraphic.transform.rotation;
                            }
                        }
                    }

                    moveRoute.animator = target.GetComponentInChildren<Animator>();
                    moveRoute.navMeshAgent = target.GetComponentInChildren<NavMeshAgent>();

                    moveRoute.ResumeCycle();
                }
            }

            // Evaluate events that depend on the hour of the day to make sure we don't trigger
            // wrong events when updating switches
            OnHourChanged();
        }

        public void OnHourChanged()
        {
            if (currentTarget == null)
            {
                return;
            }

            if (currentTarget.useTimeOfDay == false)
            {
                return;
            }

            // If event is running
            if (currentTarget.isRunning)
            {
                return;
            }

            bool isActive = false;

            // If appear until is after midnight, it may become smaller than appearFrom (i. e. appear from 17 until 4)
            if (currentTarget.appearFrom > currentTarget.appearUntil)
            {
                isActive = Player.instance.timeOfDay >= currentTarget.appearFrom && Player.instance.timeOfDay <= 24 || (Player.instance.timeOfDay >= 0 && Player.instance.timeOfDay <= currentTarget.appearUntil);
            }
            else
            {
                isActive = Player.instance.timeOfDay >= currentTarget.appearFrom && Player.instance.timeOfDay <= currentTarget.appearUntil;
            }

            currentTarget.gameObject.SetActive(isActive);

            if (isActive)
            {
                if (moveRoute != null)
                {
                    moveRoute.ResumeCycle();
                }
            }
        }

        public void OnGameLoaded(GameData gameData)
        {
            RefreshEventPages();
        }
    }

}
