using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using AF.Stats;

namespace AF
{
    public class ReplenishableItemPickup : VariableListener, IClockListener, IEventNavigatorCapturable, ISaveable
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

        [Header("Notification")]
        public LocalizedTerms.LocalizedAction action;

        [Header("Day / Night Manager")]
        public int daysToRespawn = 3;
        [Range(0, 23)] public int hourToReplenish = 6;

        [Header("Events")]
        public UnityEvent onItemPickup;

        [Header("Stealing")]
        public bool isStealing = false;
        [Range(0, 100f)]
        public float chanceToSteal = 50;

        private GameObject graphic => transform.GetChild(0).gameObject;

        StarterAssetsInputs inputs;
        UIDocumentKeyPrompt uIDocumentKeyPrompt;
        UIDocumentReceivedItemPrompt uIDocumentReceivedItemPrompt;
        PlayerInventory playerInventory;
        EquipmentGraphicsHandler equipmentGraphicsHandler;

        public StatsBonusController playerStatsBonusController;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        [Header("Systems")]
        public WorldSettings worldSettings;

        private void Awake()
        {
            inputs = FindObjectOfType<StarterAssetsInputs>(true);
            uIDocumentKeyPrompt = FindObjectOfType<UIDocumentKeyPrompt>(true);
            uIDocumentReceivedItemPrompt = FindObjectOfType<UIDocumentReceivedItemPrompt>(true);
            playerInventory = FindObjectOfType<PlayerInventory>(true);
            equipmentGraphicsHandler = playerInventory.GetComponent<EquipmentGraphicsHandler>();
        }

        private void Start()
        {
            Refresh();
        }

        public override void Refresh()
        {
            var variableValue = VariableManager.instance.GetVariableValue(variableEntry);

            // Item not picked up
            if (variableValue == -1)
            {
                graphic.SetActive(true);
                return;
            }

            if (HasReplenished())
            {
                graphic.SetActive(true);
                VariableManager.instance.UpdateVariable(variableEntry, -1);
                return;
            }

            graphic.SetActive(false);
        }

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
                if ((worldSettings.daysPassed - 1) == VariableManager.instance.GetVariableValue(variableEntry))
                {
                    if (worldSettings.timeOfDay >= hourToReplenish)
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
            VariableManager.instance.UpdateVariable(variableEntry, -1);
            graphic.SetActive(true);
        }

        bool HasReplenished()
        {
            if (worldSettings.daysPassed > VariableManager.instance.GetVariableValue(variableEntry) + daysToRespawn)
            {
                return true;
            }

            return false;
        }

        public void OnCaptured()
        {
            if (graphic.activeSelf == false)
            {
                return;
            }

            string key = "E";
            string actionName = "";

            if (isStealing)
            {
                actionName = LocalizedTerms.GetStealChance(GetStealChance());
            }
            else
            {
                actionName = LocalizedTerms.GetActionText(action);
            }

            uIDocumentKeyPrompt.DisplayPrompt(key, actionName);
        }

        float GetStealChance()
        {
            return chanceToSteal + playerStatsBonusController.chanceToStealBonus;
        }

        public void OnInvoked()
        {
            if (graphic.activeSelf == false)
            {
                return;
            }

            uIDocumentKeyPrompt.gameObject.SetActive(false);

            inputs.interact = false;


            if (isStealing)
            {
                if (Random.Range(0, 100f) > GetStealChance())
                {
                    // Get All items Price

                    var itemsPrice = item != null ? item.value : replenishableItemEntries.Sum(x => x.item.value);

                    playerInventory.playerAchievementsManager.achievementForStealing.AwardAchievement();

                    if (playerStatsDatabase.gold >= itemsPrice)
                    {
                        FindObjectOfType<UIDocumentPlayerGold>(true).NotifyGoldLost((int)itemsPrice);
                        playerStatsDatabase.gold -= (int)itemsPrice;

                        var notif = FindObjectOfType<NotificationManager>(true);
                        notif.ShowNotification(LocalizedTerms.CaughtStealing(), notif.personBusy);
                        Soundbank.instance.PlayReputationDecreased();
                    }
                    else
                    {
                        FindObjectOfType<UIDocumentPlayerGold>(true).NotifyGoldLost((int)itemsPrice - playerStatsDatabase.gold);
                        playerStatsDatabase.gold = 0;

                        // Lose reputation instead
                        FindObjectOfType<NotificationManager>(true).DecreaseReputation(1);
                    }

                    return;
                }
            }


            if (item != null)
            {

                UIDocumentReceivedItemPrompt.ItemsReceived itemReceived = new UIDocumentReceivedItemPrompt.ItemsReceived();

                itemReceived.itemName = item.name.GetText();
                itemReceived.quantity = quantity;
                itemReceived.sprite = item.sprite;

                //uIDocumentReceivedItemPrompt.itemsUI.Clear();
                //uIDocumentReceivedItemPrompt.itemsUI.Add(itemReceived);

                uIDocumentReceivedItemPrompt.gameObject.SetActive(true);
                playerInventory.AddItem(item, quantity);
            }
            else if (replenishableItemEntries.Length > 0)
            {
                //uIDocumentReceivedItemPrompt.itemsUI.Clear();

                foreach (var possibleItem in replenishableItemEntries)
                {
                    var calcChance = Random.Range(0, 100);
                    if (calcChance <= possibleItem.chance)
                    {
                        UIDocumentReceivedItemPrompt.ItemsReceived itemReceived = new UIDocumentReceivedItemPrompt.ItemsReceived();

                        itemReceived.itemName = possibleItem.item.name.GetText();
                        itemReceived.quantity = possibleItem.quantity;
                        itemReceived.sprite = possibleItem.item.sprite;

                        //uIDocumentReceivedItemPrompt.itemsUI.Add(itemReceived);
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
            VariableManager.instance.UpdateVariable(variableEntry, worldSettings.daysPassed);
        }


        public void OnGameLoaded(object gameData)
        {
            Refresh();
        }

        public void OnReleased()
        {
            throw new System.NotImplementedException();
        }
    }
}
