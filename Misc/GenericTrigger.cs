using AF.Inventory;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class GenericTrigger : MonoBehaviour, IEventNavigatorCapturable
    {

        [Header("Events")]
        public UnityEvent onActivate;

        [Header("Prompt")]
        public string key = "E";
        public string action = "Pickup";

        // Scene Refs
        UIDocumentKeyPrompt uIDocumentKeyPrompt;

        [Header("Alchemy Pickable Info")]
        public Item item;

        [Header("Required Item to Open")]
        public Item requiredItemToOpen;
        public InventoryDatabase inventoryDatabase;

        public void OnCaptured()
        {
            GetUIDocumentKeyPrompt().DisplayPrompt(key, action, item);
        }

        public void OnInvoked()
        {
            DisableKeyPrompt();

            HandleActivation();
        }

        public void OnReleased()
        {
            DisableKeyPrompt();
        }

        public void DisableKeyPrompt()
        {
            GetUIDocumentKeyPrompt().gameObject.SetActive(false);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void TurnCapturable()
        {
            this.gameObject.layer = LayerMask.NameToLayer("IEventNavigatorCapturable");
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void DisableCapturable()
        {
            this.gameObject.layer = 0;
        }

        UIDocumentKeyPrompt GetUIDocumentKeyPrompt()
        {
            if (uIDocumentKeyPrompt == null)
            {
                uIDocumentKeyPrompt = FindAnyObjectByType<UIDocumentKeyPrompt>(FindObjectsInactive.Include);
            }

            return uIDocumentKeyPrompt;
        }

        public void HandleActivation()
        {
            bool canActivate = true;

            if (requiredItemToOpen != null && inventoryDatabase != null)
            {
                if (inventoryDatabase.HasItem(requiredItemToOpen))
                {
                    inventoryDatabase.RemoveItem(requiredItemToOpen);
                    GetNotificationManager().ShowNotification($"{requiredItemToOpen.name} was lost with its use.");
                }
                else
                {
                    GetNotificationManager().ShowNotification($"{requiredItemToOpen.name} is required to activate.");
                    canActivate = false;
                }
            }

            if (canActivate)
            {
                onActivate?.Invoke();
            }
        }

        NotificationManager GetNotificationManager()
        {
            return FindAnyObjectByType<NotificationManager>(FindObjectsInactive.Include);
        }
    }
}
