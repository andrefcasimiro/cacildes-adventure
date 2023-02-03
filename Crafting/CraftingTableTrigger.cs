using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.Events;

namespace AF
{
    public class CraftingTableTrigger : MonoBehaviour
    {
        public UIDocumentAlchemyCraftScreen.CraftActivity craftActivity;

        StarterAssetsInputs inputs;
        UIDocumentKeyPromptAction uIDocumentKeyPrompt;

        UIDocumentAlchemyCraftScreen alchemyCraftScreen;

        private void Awake()
        {
            alchemyCraftScreen = FindObjectOfType<UIDocumentAlchemyCraftScreen>(true);
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

            if (craftActivity == UIDocumentAlchemyCraftScreen.CraftActivity.ALCHEMY)
            {
                uIDocumentKeyPrompt.action = "Use Alchemy Table";
            }
            else if (craftActivity == UIDocumentAlchemyCraftScreen.CraftActivity.COOKING)
            {
                uIDocumentKeyPrompt.action = "Cook";
            }

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


                alchemyCraftScreen.craftActivity = craftActivity;
                alchemyCraftScreen.gameObject.SetActive(true);
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
            if (uIDocumentKeyPrompt != null)
            {
                uIDocumentKeyPrompt.gameObject.SetActive(false);
            }
        }


    }

}
