using UnityEngine;
using StarterAssets;
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
        
        [Header("Event Navigator Options")]
        public float distanceToTriggerBonus = 1f;

        public float GetDistanceToTrigger()
        {
            return distanceToTriggerBonus;
        }

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

            uIDocumentKeyPrompt.key = "E";
            uIDocumentKeyPrompt.action = actionName.GetText();
            uIDocumentKeyPrompt.gameObject.SetActive(true);
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

    }

}
