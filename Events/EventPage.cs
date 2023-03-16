using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public SwitchEntry switchEntryCondition;

        public bool value;
    }

    public class EventPage : MonoBehaviour, IEventNavigatorCapturable
    {
        public EventTrigger eventTrigger = EventTrigger.ON_KEY_PRESS;

        [Header("Day / Time Settings")]
        public float appearFrom = 00.00f;
        public float appearUntil = 24.00f;
        [HideInInspector] public bool useTimeOfDay;

        [Header("Switches")]
        public EventPageSwitch[] switchConditions;

        [Header("Key Prompt")]
        public LocalizedText notificationText;
        public LocalizedTerms.LocalizedAction localizedAction;

        [Header("Cancel Event")]
        [Tooltip("For events with move route, you need to set the transform of the navmesh agent")]
        public Transform transformReferenceForCancelling;
        public float maxDistanceBeforeStoppingEvent = 5f;

        [HideInInspector] public Event eventParent;
        [HideInInspector] public List<EventBase> pageEvents = new List<EventBase>();
        [HideInInspector] public bool isRunning = false;
        MoveRoute parentMoveRoute;

        [Header("Settings")]
        public bool allowTimePassage = false;

        // Event Page transform child that controls movement
        private GameObject player;

        DayNightManager dayNightManager => FindObjectOfType<DayNightManager>(true);
        EnemyManager enemyManager => GetComponent<EnemyManager>();
        UIDocumentDialogueWindow uIDocumentDialogueWindow => FindObjectOfType<UIDocumentDialogueWindow>(true);
        UIDocumentKeyPrompt documentKeyPrompt => FindObjectOfType<UIDocumentKeyPrompt>(true);

        private void Awake()
        {
            useTimeOfDay = appearFrom != 00.00f || appearUntil != 24.00f;

            eventParent = transform.GetComponentInParent<Event>();

            if (eventParent != null)
            {
                parentMoveRoute = eventParent.moveRoute;
            }

            EventBase[] gatheredEvents = GetComponents<EventBase>();

            pageEvents.Clear();
            foreach (EventBase ev in gatheredEvents)
            {
                pageEvents.Add(ev);
            }

            player = GameObject.FindWithTag("Player");
        }

        private void Update()
        {
            if (parentMoveRoute != null && parentMoveRoute.IsRunning() == false)
            {
                parentMoveRoute.StartCycle();
            }

            if (isRunning && transformReferenceForCancelling != null)
            {
                if (Vector3.Distance(transformReferenceForCancelling.position, player.transform.position) >= maxDistanceBeforeStoppingEvent)
                {
                    StopEvent();
                }
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player") && eventTrigger == EventTrigger.ON_PLAYER_TOUCH && isRunning == false)
            {
                BeginEvent();
            }
        }

        public void BeginEvent()
        {
            if (CanRunEventPage() == false)
            {
                return;
            }

            if (FindObjectOfType<MenuManager>(true).IsMenuOpen())
            {
                return;
            }

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

            StopEvent();
        }

        void StopEvent()
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

            SwitchManager.instance.UpdateQueuedSwitches();
        }

        public bool CanRunEventPage()
        {
            // If event belongs to an enemy and enemy is agressive
            if (enemyManager != null && enemyManager.enemyBehaviorController.isAgressive)
            {
                return false;
            }

            return true;
        }

        public void OnCaptured()
        {
            if (isRunning || eventTrigger != EventTrigger.ON_KEY_PRESS || CanRunEventPage() == false)
            {
                return;
            }

            // If a dialogue is ocurring, ignore event interaction
            if (uIDocumentDialogueWindow.isActiveAndEnabled)
            {
                documentKeyPrompt.gameObject.SetActive(false);

                return;
            }

            documentKeyPrompt.key = "E";

            if (localizedAction != LocalizedTerms.LocalizedAction.NONE)
            {
                documentKeyPrompt.action = LocalizedTerms.GetActionText(localizedAction);
            }
            else
            {
                documentKeyPrompt.action = notificationText.GetText();
            }

            documentKeyPrompt.eventUuid = gameObject.name;
            documentKeyPrompt.gameObject.SetActive(true);

        }

        public void OnInvoked()
        {
            if (isRunning || CanRunEventPage() == false)
            {
                return;
            }

            BeginEvent();
        }

        private void OnDisable()
        {
            isRunning = false;
        }
    }
}
