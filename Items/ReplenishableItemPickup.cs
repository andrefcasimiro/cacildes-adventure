using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using UnityEngine.Events;

namespace AF
{


    public class ReplenishableItemPickup : VariableListener, IClockListener
    {
        [System.Serializable]
        public class ReplenishableItemEntry
        {
            public Item item;
            public int quantity = 1;
            [Range(0, 100)] public float chance = 100;
        }

        [Header("Single Item")]
        public Item item;
        public int quantity = 1;

        [Header("Multiple Items")]
        public ReplenishableItemEntry[] replenishableItemEntries;

        StarterAssetsInputs inputs;
        UIDocumentKeyPromptAction uIDocumentKeyPrompt;
        UIDocumentReceivedItemPrompt uIDocumentReceivedItemPrompt;
        PlayerInventory playerInventory;

        [Header("Notification")]
        public string actionName = "Pick up Item";

        [Header("Day / Night Manager")]
        public int daysToRespawn = 3;
        [Range(0, 23)] public int hourToReplenish = 6;

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


                if (item != null)
                {

                    UIDocumentReceivedItemPrompt.ItemsReceived itemReceived = new UIDocumentReceivedItemPrompt.ItemsReceived();

                    itemReceived.itemName = item.name;
                    itemReceived.quantity = quantity;
                    itemReceived.sprite = item.sprite;

                    uIDocumentReceivedItemPrompt.itemsUI.Clear();
                    uIDocumentReceivedItemPrompt.itemsUI.Add(itemReceived);

                    uIDocumentReceivedItemPrompt.gameObject.SetActive(true);
                    playerInventory.AddItem(item, quantity);
                }
                else if (replenishableItemEntries.Length > 0)
                {
                    uIDocumentReceivedItemPrompt.itemsUI.Clear();

                    foreach (var possibleItem in replenishableItemEntries)
                    {
                        var calcChance = Random.Range(0, 100);
                        if (calcChance <= possibleItem.chance)
                        {
                            UIDocumentReceivedItemPrompt.ItemsReceived itemReceived = new UIDocumentReceivedItemPrompt.ItemsReceived();

                            itemReceived.itemName = possibleItem.item.name;
                            itemReceived.quantity = possibleItem.quantity;
                            itemReceived.sprite = possibleItem.item.sprite;

                            uIDocumentReceivedItemPrompt.itemsUI.Add(itemReceived);
                            playerInventory.AddItem(possibleItem.item, possibleItem.quantity);
                        }
                    }

                    uIDocumentReceivedItemPrompt.gameObject.SetActive(true);
                }

                Soundbank.instance.PlayItemReceived();

                if (onItemPickup != null)
                {
                    onItemPickup.Invoke();
                }

                // Record the day that the item was picked
                VariableManager.instance.UpdateVariable(variableUuid, Player.instance.daysPassed);
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
        }

        // Runs on start and game loaded
        public override void EvaluateVariable()
        {
            var variableValue = VariableManager.instance.GetVariableValue(variableUuid);

            // Item not picked up
            if (variableValue == -1)
            {
                graphic.SetActive(true);
                return;
            }

            if (HasReplenished())
            {
                graphic.SetActive(true);
                VariableManager.instance.UpdateVariable(variableUuid, -1);
                return;
            }

            graphic.SetActive(false);
        }

        // Runs every hour change
        public void OnHourChanged()
        {
            // Graphic is active, has not been picked up yet
            if (graphic.activeSelf)
            {
                return;
            }

            if (HasReplenished())
            {
                // If is only one day passed, only enable after passing the hour threshold
                if ((Player.instance.daysPassed - 1) == VariableManager.instance.GetVariableValue(variableUuid))
                {
                    if (Player.instance.timeOfDay >= hourToReplenish)
                    {
                        Replenish();
                    }
                }
                else
                {
                    Replenish();
                }
            }
        }

        void Replenish()
        {
            VariableManager.instance.UpdateVariable(variableUuid, -1);
            graphic.SetActive(true);
        }

        bool HasReplenished()
        {
            if (Player.instance.daysPassed > VariableManager.instance.GetVariableValue(variableUuid) + daysToRespawn)
            {
                return true;
            }

            return false;

        }
    }

}
