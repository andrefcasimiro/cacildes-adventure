using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.Events;

namespace AF
{

    public class GenericTrigger : MonoBehaviour
    {
        StarterAssetsInputs inputs;
        UIDocumentKeyPromptAction uIDocumentKeyPrompt;

        [Header("Notification")]
        public string actionName = "Activate";

        public UnityEvent onActivate;

        public bool deactivateTriggerOnInput = false;

        private void Awake()
        {
            uIDocumentKeyPrompt = FindObjectOfType<UIDocumentKeyPromptAction>(true);
            inputs = FindObjectOfType<StarterAssetsInputs>(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "Player")
            {
                return;
            }

            uIDocumentKeyPrompt.key = "E";
            uIDocumentKeyPrompt.action = actionName;
            uIDocumentKeyPrompt.gameObject.SetActive(true);
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag != "Player")
            {
                return;
            }

            if (inputs.interact)
            {
                inputs.interact = false;

                uIDocumentKeyPrompt.gameObject.SetActive(false);

                onActivate.Invoke();

                if (deactivateTriggerOnInput)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag != "Player")
            {
                return;
            }

            uIDocumentKeyPrompt.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            if (uIDocumentKeyPrompt == null) { return; }
            uIDocumentKeyPrompt.gameObject.SetActive(false);
        }

    }

}
