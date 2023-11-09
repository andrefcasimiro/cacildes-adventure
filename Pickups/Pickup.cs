using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace AF.Pickups
{
    public class Pickup : MonoBehaviour, IEventNavigatorCapturable
    {
        [Header("Components")]
        public MonoBehaviourID monoBehaviourID;
        public UIDocumentKeyPrompt uIDocumentKeyPrompt;

        [Header("Systems")]
        public PickupDatabase pickupDatabase;

        [Header("Events")]
        public UnityEvent onChestPickup;
        public UnityEvent onAlreadyPickedUp;

        [Header("Prompt")]
        public string key = "E";
        public string action = "Pickup";

        public void OnEnable()
        {
            if (pickupDatabase.Contains(monoBehaviourID.ID))
            {
                onAlreadyPickedUp?.Invoke();
                return;
            }
        }

        public void OnCaptured()
        {
            uIDocumentKeyPrompt.DisplayPrompt(key, action);
        }

        public void OnInvoked()
        {
            uIDocumentKeyPrompt.gameObject.SetActive(false);

            pickupDatabase.Add(monoBehaviourID.ID, SceneManager.GetActiveScene().name + "-" + name);
            onChestPickup?.Invoke();
        }

        public void OnReleased()
        {
            uIDocumentKeyPrompt.gameObject.SetActive(false);
        }
    }
}
