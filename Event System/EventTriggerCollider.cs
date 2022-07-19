using System.Collections;
using UnityEngine;

namespace AF
{
    public class EventTriggerCollider : MonoBehaviour
    {
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
            if (other.gameObject.tag == "Player")
            {
                if (menuManager.IsMenuOpened())
                {
                    keyPressPromptManager.Close();

                    return;
                }

                // By Default, disable combat when player is near an event
                PlayerCombatManager playerCombatManager = other.GetComponent<PlayerCombatManager>();
                playerCombatManager.DisableCombat();

                if (eventPage.eventTrigger == EventTrigger.ON_PLAYER_TOUCH)
                {
                    StartCoroutine(eventPage.DispatchEvents());
                }

                if (eventPage.eventTrigger == EventTrigger.ON_KEY_PRESS && System.String.IsNullOrEmpty(eventPage.notificationText) == false)
                {
                    keyPressPromptManager.ShowNotification(eventPage.notificationText);
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (menuManager.IsMenuOpened())
            {
                keyPressPromptManager.Close();

                return;
            }

            if (eventPage.canInteract == false)
            {
                return;
            }

            if (other.gameObject.tag == "Player")
            {
                if (eventPage.isRunning)
                {
                    return;
                }

                if (eventPage.hasPressedConfirmButton && eventPage.eventTrigger == EventTrigger.ON_KEY_PRESS)
                {
                    PlayerCombatManager playerCombatManager = other.GetComponent<PlayerCombatManager>();
                    playerCombatManager.DisableCombat();

                    StartCoroutine(eventPage.DispatchEvents());
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                keyPressPromptManager.Close();

                PlayerCombatManager playerCombatManager = other.GetComponent<PlayerCombatManager>();
                playerCombatManager.EnableCombat();
            }
        }

    }
}