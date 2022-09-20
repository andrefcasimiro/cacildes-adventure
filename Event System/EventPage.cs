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
        ON_TARGET_TOUCH,
        NO_TRIGGERED,
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

        public bool debug = false;

        Player player;
        PlayerCombatManager playerCombatManager;

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

            if (eventTrigger == EventTrigger.AUTOMATIC)
            {   
                // By Default, disable combat when player is near an event
                PlayerCombatManager playerCombatManager = FindObjectOfType<Player>(true).GetComponent<PlayerCombatManager>();
                playerCombatManager.DisableCombat();

                StartCoroutine(DispatchEvents());
            }
        }

        private new void OnEnable()
        {
            base.OnEnable();

            canInteract = false;

            if (eventTrigger == EventTrigger.NO_TRIGGERED)
            {
                return;
            }

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

            if (player != null)
            {
                player.SetBusy(false);
                player.playerCombatManager.EnableCombat();
            }

            canInteract = false;
            isRunning = false;
        }

        private void Update()
        {
            if (isRunning)
            {
                if (eventTrigger != EventTrigger.ON_PLAYER_TOUCH)
                {
                    if (player == null)
                    {
                        player = FindObjectOfType<Player>();
                    }
                    if (playerCombatManager == null)
                    {
                        playerCombatManager = player.GetComponent<PlayerCombatManager>();
                    }

                    playerCombatManager.canCombat = false;
                    player.SetBusy(true);
                }
            }

            if (eventTrigger == EventTrigger.AUTOMATIC)
            {
                FindObjectOfType<Player>(true).SetBusy(true);
            }

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
            if (debug)
            {
                Debug.Log("event started: " + this.eventParent.eventName);
            }

            if (moveRoute != null)
            {
                moveRoute.Interrupt();
            }

            keyPressPromptManager.Close();


            if (eventTrigger != EventTrigger.ON_PLAYER_TOUCH)
            {
                Player player = FindObjectOfType<Player>();
                if (player != null)
                {
                    PlayerCombatManager playerCombatManager = player.GetComponent<PlayerCombatManager>();
                    playerCombatManager.canCombat = false;
                    player.SetBusy(true);
                }
            }

            isRunning = true;

            foreach (EventBase ev in events)
            {
                if (debug)
                {
                    Debug.Log("event entry: ");
                    Debug.Log(ev.name);
                }

                if (ev != null)
                {
                    yield return StartCoroutine(ev.Dispatch());
                }
            }

            if (eventTrigger != EventTrigger.ON_PLAYER_TOUCH)
            {
                Player player = FindObjectOfType<Player>();
                if (player != null)
                {
                    PlayerCombatManager playerCombatManager = player.GetComponent<PlayerCombatManager>();
                    playerCombatManager.canCombat = true;
                    player.SetBusy(false);
                }
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
