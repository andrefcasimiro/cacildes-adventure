using System.Collections;
using UnityEngine;

namespace AF
{
    public class EventTriggerCollider : MonoBehaviour
    {
        [Tooltip("If set, event will trigger on the given target instead of the player")]
        public GameObject target;

        EventPage eventPage;
        KeyPressPromptManager keyPressPromptManager;
        MenuManager menuManager;

        private void Awake()
        {
            eventPage = GetComponentInParent<EventPage>();
            keyPressPromptManager = FindObjectOfType<KeyPressPromptManager>(true);
            menuManager = FindObjectOfType<MenuManager>(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (target != null)
            {
                if (GameObject.ReferenceEquals(other.gameObject, target) && eventPage.eventTrigger == EventTrigger.ON_TARGET_TOUCH) {
                    StartCoroutine(eventPage.DispatchEvents());
                    return;
                }

                return;
            }

            if (other.gameObject.tag != "Player")
            {
                return;
            }

            if (eventPage.eventTrigger == EventTrigger.ON_PLAYER_TOUCH)
            {
                StartCoroutine(eventPage.DispatchEvents());
                return;
            }

            if (menuManager.IsMenuOpened())
            {
                keyPressPromptManager.Close();
                return;
            }

            // By Default, disable combat when player is near an event
            PlayerCombatManager playerCombatManager = other.GetComponent<PlayerCombatManager>();
            playerCombatManager.DisableCombat();

            if (eventPage.eventTrigger == EventTrigger.ON_KEY_PRESS && System.String.IsNullOrEmpty(eventPage.notificationText) == false)
            {
                ShowNotification();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag != "Player")
            {
                return;
            }

            if (menuManager.IsMenuOpened())
            {
                keyPressPromptManager.Close();
                return;
            }

            // Player already in another event?
            Player player = other.gameObject.GetComponent<Player>();
            if (player.IsBusy())
            {
                return;
            }

            if (eventPage.canInteract == false || eventPage.isRunning)
            {
                return;
            }

            if (eventPage.hasPressedConfirmButton && eventPage.eventTrigger == EventTrigger.ON_KEY_PRESS)
            {
                keyPressPromptManager.Close();
                StartCoroutine(eventPage.DispatchEvents());
                return;
            }
            
            // Case where we have already had the event and finished it, and want the key press ui to reappear
            if (keyPressPromptManager.IsVisible() == false)
            {
                if (eventPage.eventTrigger == EventTrigger.ON_KEY_PRESS && System.String.IsNullOrEmpty(eventPage.notificationText) == false)
                {
                    ShowNotification();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag != "Player")
            {
                return;
            }

            keyPressPromptManager.Close();

            PlayerCombatManager playerCombatManager = other.GetComponent<PlayerCombatManager>();
            playerCombatManager.EnableCombat();
        }

        void ShowNotification()
        {
            keyPressPromptManager.ShowNotification(eventPage.notificationText);
        }

    }
}
