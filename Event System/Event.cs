using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{

    public class Event : MonoBehaviour
    {
        public string eventName;

        [HideInInspector]
        public EventPage[] eventPages;

        public LocalSwitch localSwitch;

        private void Awake()
        {
            EventPage[] eventPages = transform.GetComponentsInChildren<EventPage>(true);

            this.eventPages = eventPages;
        }

        private void Start()
        {
            RefreshEventPages();
        }

        public void RefreshEventPages()
        {
            List<LocalSwitch> localSwitches = new List<LocalSwitch>();

            EventPage previousEventPage = null;

            // Deactivate all event pages
            foreach (EventPage evt in transform.GetComponentsInChildren<EventPage>(true))
            {
                if (evt.gameObject.activeSelf)
                {
                    previousEventPage = evt;
                }

                evt.gameObject.SetActive(false);
            }

            // Find last event page where all switch conditions are met
            EventPage target = eventPages.LastOrDefault(evtPage =>
            {
                bool allSwitchConditions = evtPage.switchConditions.All(
                    // Evaluate if switch value equals the current value on the switch manager database
                    switchCondition => (switchCondition.value == SwitchManager.instance.EvaluateSwitch(switchCondition.switchName))
                    );

                if (allSwitchConditions)
                {
                    // All switches of this event page are active, now let's combine them with the local switch
                    if (localSwitch != null)
                    {
                        return evtPage.localSwitchCondition == localSwitch.localSwitchName;
                    }

                    // No local switch set for this event, so the event page with all switches active is returned
                    return true;
                }

                return false;
            });

            if (target == null && eventPages.Length > 0)
            {
                eventPages[0].gameObject.SetActive(true);
            }
            else if (target != null)
            {
                target.gameObject.SetActive(true);
            }

            MoveRoute moveRoute = GetComponentInChildren<MoveRoute>();
            if (moveRoute != null)
            {
                // Position new event page graphic in the same position of the previous event page to make the transition seamless
                if (previousEventPage != null)
                {
                    Transform previousEventPageTargetGraphic = previousEventPage.GetComponentInChildren<Animator>().transform;
                    Transform targetGraphic = target.GetComponentInChildren<Animator>().transform;

                    targetGraphic.transform.position = previousEventPageTargetGraphic.transform.position;
                    targetGraphic.transform.rotation = previousEventPageTargetGraphic.transform.rotation;
                }

                moveRoute.animator = target.GetComponentInChildren<Animator>();
                moveRoute.navMeshAgent = target.GetComponentInChildren<NavMeshAgent>();

                moveRoute.ResumeCycle();
            }
        }
    }

}
