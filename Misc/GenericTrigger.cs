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

        public void OnCaptured()
        {
            GetUIDocumentKeyPrompt().DisplayPrompt(key, action, item);
        }

        public void OnInvoked()
        {
            DisableKeyPrompt();
            onActivate?.Invoke();
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
    }
}
