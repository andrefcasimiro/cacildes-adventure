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
            uIDocumentKeyPrompt.key = "E";
            uIDocumentKeyPrompt.action = actionName.GetText();
            uIDocumentKeyPrompt.gameObject.SetActive(true);

            if (inputs.interact)
            {
                OnInvoked();
            }
        }

        public void OnInvoked()
        {
            if (triggerOnlyOnce && hasTriggered)
            {
                return;
            }

            if (inputs.interact)
            {
                if (triggerOnlyOnce)
                {
                    hasTriggered = true;
                }

                inputs.interact = false;

                uIDocumentKeyPrompt.gameObject.SetActive(false);

                onActivate.Invoke();

                if (deactivateTriggerOnInput)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }

    }

}
