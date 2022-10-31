using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

namespace AF
{
    public class AlchemyTableTrigger : MonoBehaviour
    {
        StarterAssetsInputs inputs;
        UIDocumentKeyPromptAction uIDocumentKeyPrompt;

        private void Awake()
        {
            uIDocumentKeyPrompt = FindObjectOfType<UIDocumentKeyPromptAction>(true);
            inputs = FindObjectOfType<StarterAssetsInputs>(true);
        }

        private void Start()
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "Player")
            {
                return;
            }

            uIDocumentKeyPrompt.key = "E";
            uIDocumentKeyPrompt.action = "Use Alchemy Table";
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

                FindObjectOfType<PlayerComponentManager>(true).DisableComponents();
                FindObjectOfType<PlayerComponentManager>(true).DisableCharacterController();

                FindObjectOfType<UIDocumentAlchemyCraftScreen>(true).gameObject.SetActive(true);
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


    }

}
