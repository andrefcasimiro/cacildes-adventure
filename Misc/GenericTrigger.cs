using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class GenericTrigger : MonoBehaviour, IEventNavigatorCapturable
    {
        [Header("Components (Optional assignment, but recommended)")]
        public UIDocumentKeyPrompt uIDocumentKeyPrompt;

        [Header("Events")]
        public UnityEvent onActivate;

        [Header("Prompt")]
        public string key = "E";
        public string action = "Pickup";

        public void OnCaptured()
        {
            GetUIDocumentKeyPrompt().DisplayPrompt(key, action);
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
