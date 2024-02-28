using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class GenericTrigger : MonoBehaviour, IEventNavigatorCapturable
    {
        [Header("Components")]
        public UIDocumentKeyPrompt uIDocumentKeyPrompt;

        [Header("Events")]
        public UnityEvent onActivate;

        [Header("Prompt")]
        public string key = "E";
        public string action = "Pickup";

        public void OnCaptured()
        {
            uIDocumentKeyPrompt.DisplayPrompt(key, action);
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
            uIDocumentKeyPrompt.gameObject.SetActive(false);
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
    }
}
