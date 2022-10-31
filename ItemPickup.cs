using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

namespace AF
{

    public class ItemPickup : SwitchListener
    {
        public Item item;

        public AlchemyRecipe alchemyRecipe;

        public int quantity = 1;

        StarterAssetsInputs inputs;
        UIDocumentKeyPromptAction uIDocumentKeyPrompt;
        UIDocumentReceivedItemPrompt uIDocumentReceivedItemPrompt;
        PlayerInventory playerInventory;

        [Header("Notification")]
        public string actionName = "Pick up Item";

        private void Awake()
        {
            playerInventory = FindObjectOfType<PlayerInventory>(true);
            uIDocumentKeyPrompt = FindObjectOfType<UIDocumentKeyPromptAction>(true);
            uIDocumentReceivedItemPrompt = FindObjectOfType<UIDocumentReceivedItemPrompt>(true);
            inputs = FindObjectOfType<StarterAssetsInputs>(true);
        }

        private void Start()
        {
            EvaluateSwitch();
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

                // Is Alchemy Recipe?
                if (alchemyRecipe != null)
                {
                    Player.instance.alchemyRecipes.Add(alchemyRecipe);
                }
                else // Normal Item
                {
                    uIDocumentReceivedItemPrompt.itemName = item.name;
                    uIDocumentReceivedItemPrompt.quantity = quantity;
                    uIDocumentReceivedItemPrompt.sprite = item.sprite;

                    uIDocumentReceivedItemPrompt.gameObject.SetActive(true);
                    playerInventory.AddItem(item, quantity);
                }


                SwitchManager.instance.UpdateSwitch(this._switch.ID, true);
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

        public override void EvaluateSwitch()
        {
            this.gameObject.SetActive(!SwitchManager.instance.GetSwitchValue(this._switch.ID));
        }

        private void OnDisable()
        {
            if (uIDocumentKeyPrompt == null) { return; }
            uIDocumentKeyPrompt.gameObject.SetActive(false);
        }

    }

}
