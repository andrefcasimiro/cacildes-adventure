using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.Events;

namespace AF
{

    public class ReplenishableItemPickup : VariableListener
    {
        public Item item;

        public int quantity = 1;

        StarterAssetsInputs inputs;
        UIDocumentKeyPromptAction uIDocumentKeyPrompt;
        UIDocumentReceivedItemPrompt uIDocumentReceivedItemPrompt;
        PlayerInventory playerInventory;

        [Header("Notification")]
        public string actionName = "Pick up Item";

        [Header("Day / Night Manager")]
        public int daysToRespawn = 3;

        private GameObject graphic;

        public UnityEvent onItemPickup;

        private void Awake()
        {
            playerInventory = FindObjectOfType<PlayerInventory>(true);
            uIDocumentKeyPrompt = FindObjectOfType<UIDocumentKeyPromptAction>(true);
            uIDocumentReceivedItemPrompt = FindObjectOfType<UIDocumentReceivedItemPrompt>(true);
            inputs = FindObjectOfType<StarterAssetsInputs>(true);
            graphic = transform.GetChild(0).gameObject;
        }

        private void Start()
        {
            this._variable = VariableManager.instance.GetVariableInstance(this.variableUuid);

            EvaluateVariable();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "Player")
            {
                return;
            }

            if (graphic.gameObject.activeSelf == false)
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

            if (graphic.gameObject.activeSelf == false)
            {
                return;
            }

            if (inputs.interact)
            {
                inputs.interact = false;

                uIDocumentKeyPrompt.gameObject.SetActive(false);

                uIDocumentReceivedItemPrompt.itemName = item.name;
                uIDocumentReceivedItemPrompt.quantity = quantity;
                uIDocumentReceivedItemPrompt.sprite = item.sprite;

                uIDocumentReceivedItemPrompt.gameObject.SetActive(true);
                playerInventory.AddItem(item, quantity);
                
                BGMManager.instance.PlayItem();

                if (onItemPickup != null) {
                    onItemPickup.Invoke();
                }

                // Record the day that the item was picked
                VariableManager.instance.UpdateVariable(this._variable.uuid.ToString(), Player.instance.daysPassed);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag != "Player")
            {
                return;
            }

            if (graphic.gameObject.activeSelf == false)
            {
                return;
            }

            uIDocumentKeyPrompt.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (graphic.activeSelf == true)
            {
                return;
            }

            if (Player.instance.daysPassed > VariableManager.instance.GetVariableValue(_variable.uuid) + daysToRespawn)
            {
                VariableManager.instance.UpdateVariable(this._variable.uuid, -1);

                graphic.SetActive(true);
            }
        }

        public override void EvaluateVariable()
        {
            var variableValue = VariableManager.instance.GetVariableValue(_variable.uuid);

            // Item not picked up
            if (variableValue == -1)
            {
                graphic.SetActive(true);
                return;
            }

            bool hasReplenished = Player.instance.daysPassed > variableValue + daysToRespawn;
            if (hasReplenished)
            {
                graphic.SetActive(true);
                VariableManager.instance.UpdateVariable(this._variable.uuid, -1);
                return;
            }

            graphic.SetActive(false);
        }


    }

}
