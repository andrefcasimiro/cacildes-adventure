using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public enum EventTrigger
    {
        AUTOMATIC,
        ON_KEY_PRESS,
        ON_PLAYER_TOUCH,
    }

    public class EventPage : InputListener
    {
        // EDITOR
        public EventTrigger eventTrigger = EventTrigger.ON_KEY_PRESS;

        public Switch[] switchConditions;

        public LocalSwitchName localSwitchCondition;

        public string notificationText;

        public MoveRoute moveRoute;

        [HideInInspector]
        public Event eventParent;

        [HideInInspector]
        public List<EventBase> events = new List<EventBase>();

        [HideInInspector] public bool isRunning = false;

        [HideInInspector] public bool canInteract = false;

        KeyPressPromptManager keyPressPromptManager;

        private void Awake()
        {
            this.eventParent = this.transform.GetComponentInParent<Event>();

            EventBase[] gatheredEvents = GetComponents<EventBase>();

            this.events.Clear();
            foreach (EventBase ev in gatheredEvents)
            {
                events.Add(ev);
            }
        }

        private void Start()
        {
            keyPressPromptManager = FindObjectOfType<KeyPressPromptManager>(true);
        }

        private new void OnEnable()
        {
            base.OnEnable();

            canInteract = false;

            StartCoroutine(AllowEventInteraction());
        }

        IEnumerator AllowEventInteraction()
        {
            yield return new WaitForSeconds(1f);

            canInteract = true;
        }

        private new void OnDisable()
        {
            base.OnDisable();

            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.MarkAsActive();
                player.playerCombatManager.EnableCombat();
            }

            canInteract = false;
            isRunning = false;
        }

        private void Update()
        {
            if (moveRoute == null)
            {
                return;
            }

            if (moveRoute.isRunning)
            {
                return;
            }

            moveRoute.StartCycle();
        }

        public IEnumerator DispatchEvents()
        {
            if (moveRoute != null)
            {
                moveRoute.Interrupt();
            }

            keyPressPromptManager.Close();

            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.MarkAsBusy();
            }

            isRunning = true;

            foreach (EventBase ev in events)
            {
                if (ev != null)
                {
                    yield return StartCoroutine(ev.Dispatch());
                }
            }

            if (player != null)
            {
                player.MarkAsActive();
            }

            isRunning = false;

            if (moveRoute != null)
            {
                // Return movement route
                moveRoute.ResumeCycle();

            }
        }

        public bool IsRunning()
        {
            return isRunning;
        }
    }

}
