using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class GenericTrigger : MonoBehaviour, IEventNavigatorCapturable
    {
        StarterAssetsInputs inputs;
        UIDocumentKeyPrompt uIDocumentKeyPrompt;

        [Header("Notification")]
        public LocalizedText actionName;

        public UnityEvent onActivate;

        public bool deactivateTriggerOnInput = false;
        public bool deactivatePromptOnInput = false;

        [Tooltip("When we don't want to deactivate the trigger on input, but we also dont want it to register more inputs")]
        public bool triggerOnlyOnce = false;
        bool hasTriggered = false;

        private void Awake()
        {
            uIDocumentKeyPrompt = FindObjectOfType<UIDocumentKeyPrompt>(true);
            inputs = FindObjectOfType<StarterAssetsInputs>(true);
        }

        public void OnCaptured()
        {
            if (!this.enabled)
            {
                return;
            }

            uIDocumentKeyPrompt.DisplayPrompt("E", actionName.GetText());
        }

        public void OnInvoked()
        {
            inputs.interact = false;

            if (triggerOnlyOnce && hasTriggered)
            {
                return;
            }

            if (triggerOnlyOnce)
            {
                hasTriggered = true;
            }

            uIDocumentKeyPrompt.gameObject.SetActive(false);

            onActivate.Invoke();

            if (deactivateTriggerOnInput)
            {
                this.gameObject.SetActive(false);
            }

            if (deactivatePromptOnInput)
            {
                this.enabled = false;
            }
        }

        public void OnReleased()
        {
            throw new System.NotImplementedException();
        }
    }

}
