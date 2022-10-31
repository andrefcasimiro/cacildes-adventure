using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public enum EventTrigger
    {
        ON_KEY_PRESS,
        ON_PLAYER_TOUCH,
        NO_TRIGGER,
    }

    [System.Serializable]
    public class EventPageSwitch
    {
        public Switch switchCondition;

        public bool value;
    }

    public class EventPage : MonoBehaviour
    {
        public EventTrigger eventTrigger = EventTrigger.ON_KEY_PRESS;

        [Header("Day / Time Settings")]
        public float appearFrom = 00.00f;
        public float appearUntil = 24.00f;
        [HideInInspector] public bool useTimeOfDay;

        [Header("Switches")]
        public EventPageSwitch[] switchConditions;

        public string notificationText;

        [Header("Cancel Event")]
        public float maxDistanceBeforeStoppingEvent = 5f;

        [HideInInspector] public Event eventParent;

        [HideInInspector] public List<EventBase> pageEvents = new List<EventBase>();

        [HideInInspector] private bool isRunning = false;

        MoveRoute parentMoveRoute;

        // Event Page transform child that controls movement
        private GameObject eventPageTransform;
        private GameObject player;

        UIDocumentDialogueWindow uIDocumentDialogueWindow;

        DayNightManager dayNightManager;

        public bool allowTimePassage = false;

        private void Awake()
        {
            useTimeOfDay = appearFrom != 00.00f || appearUntil != 24.00f;

            uIDocumentDialogueWindow = FindObjectOfType<UIDocumentDialogueWindow>(true);

            this.eventParent = this.transform.GetComponentInParent<Event>();

            if (this.eventParent != null)
            {
                parentMoveRoute = this.eventParent.moveRoute;
            }

            EventBase[] gatheredEvents = GetComponents<EventBase>();

            this.pageEvents.Clear();
            foreach (EventBase ev in gatheredEvents)
            {
                pageEvents.Add(ev);
            }

            player = GameObject.FindWithTag("Player");
        }


        private void Start()
        {
            dayNightManager = FindObjectOfType<DayNightManager>(true);
        }

        private void Update()
        {
            if (parentMoveRoute != null && parentMoveRoute.IsRunning() == false)
            {
                parentMoveRoute.StartCycle();
            }

            if (IsRunning() && eventPageTransform != null)
            {
                if (Vector3.Distance(eventPageTransform.transform.position, player.transform.position) >= maxDistanceBeforeStoppingEvent)
                {
                    Stop();
                }
            }

        }

        public void BeginEvent()
        {
            if (dayNightManager.TimePassageAllowed() && allowTimePassage == false)
            {
                dayNightManager.tick = false;
            }

            StartCoroutine(DispatchEvents());
        }

        public IEnumerator DispatchEvents()
        {
            if (parentMoveRoute != null)
            {
                parentMoveRoute.Interrupt();
            }

            isRunning = true;

            foreach (EventBase ev in pageEvents)
            {
                if (ev != null)
                {
                    yield return StartCoroutine(ev.Dispatch());
                }
            }

            isRunning = false;

            if (dayNightManager.TimePassageAllowed())
            {
                dayNightManager.tick = true;
            }

            if (parentMoveRoute != null)
            {
                parentMoveRoute.ResumeCycle();
            }
        }

        public void Stop()
        {
            if (dayNightManager.TimePassageAllowed())
            {
                dayNightManager.tick = true;
            }

            StopAllCoroutines();

            isRunning = false;
            

            // Dialogue Cancelling
            uIDocumentDialogueWindow.gameObject.SetActive(false);

            if (parentMoveRoute != null)
            {
                parentMoveRoute.ResumeCycle();
            }
        }

        public bool IsRunning()
        {
            return isRunning;
        }
    }

}
