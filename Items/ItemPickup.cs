using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AF
{
    public class ItemPickup : SwitchListener, IEventNavigatorCapturable, ISaveable
    {
        public Item item;

        public Item[] items;

        public AlchemyRecipe alchemyRecipe;

        public int quantity = 1;
        public int gold = -1;

        public UnityEvent onPickupEvent;
        public UnityEvent onPickupAfterConfirming;

        StarterAssetsInputs inputs;
        UIDocumentKeyPrompt uIDocumentKeyPrompt;
        UIDocumentReceivedItemPrompt uIDocumentReceivedItemPrompt;
        PlayerInventory playerInventory;

        [Header("Notification")]
        public LocalizedTerms.LocalizedAction actionName = LocalizedTerms.LocalizedAction.PICKUP_ITEM;

        [Header("Analytics")]
        public string analyticsMessage;

        public int reputationToDecreaseOnPickup = -1;

        [Header("Stealing")]
        public bool isStealing = false;
        [Range(0, 100f)]
        public float chanceToSteal = 50;
        public bool isStealingButGetsItemAnyway = false;

        private void Awake()
        {
            inputs = FindObjectOfType<StarterAssetsInputs>(true);
            uIDocumentKeyPrompt = FindObjectOfType<UIDocumentKeyPrompt>(true);
            uIDocumentReceivedItemPrompt = FindObjectOfType<UIDocumentReceivedItemPrompt>(true);
            playerInventory = FindObjectOfType<PlayerInventory>(true);
        }

        private void Start()
        {
            Refresh();
        }

        public override void Refresh()
        {
            gameObject.SetActive(!SwitchManager.instance.GetSwitchCurrentValue(switchEntry));
        }

        public void OnCaptured()
        {
            string key = "E";
            string action = "";

            if (isStealing)
            {
                action = LocalizedTerms.GetStealChance(GetStealChance());
            }
            else
            {
                action = LocalizedTerms.PickupItem();
            }

            uIDocumentKeyPrompt.DisplayPrompt(key, action);
        }

        float GetStealChance()
        {
            return chanceToSteal + playerInventory.GetComponent<EquipmentGraphicsHandler>().chanceToStealBonus;
        }

        public void OnInvoked()
        {
            inputs.interact = false;

            uIDocumentKeyPrompt.gameObject.SetActive(false);



            if (isStealing)
            {
                if (!isStealingButGetsItemAnyway && UnityEngine.Random.Range(0, 100f) > GetStealChance())
                {
                    // Get All items Price

                    var itemsPrice = item != null ? item.value : items.Sum(x => x.value);

                    playerInventory.playerAchievementsManager.achievementForStealing.AwardAchievement();


                    if (Player.instance.currentGold >= itemsPrice)
                    {
                        FindObjectOfType<UIDocumentPlayerGold>(true).NotifyGoldLost((int)itemsPrice);
                        Player.instance.currentGold -= (int)itemsPrice;

                        var notif = FindObjectOfType<NotificationManager>(true);
                        notif.ShowNotification(LocalizedTerms.CaughtStealing(), notif.personBusy);
                        Soundbank.instance.PlayReputationDecreased();
                    }
                    else
                    {
                        FindObjectOfType<UIDocumentPlayerGold>(true).NotifyGoldLost((int)itemsPrice - Player.instance.currentGold);
                        Player.instance.currentGold = 0;

                        // Lose reputation instead
                        FindObjectOfType<NotificationManager>(true).DecreaseReputation(1);
                    }

                    return;
                }
            }


            if (reputationToDecreaseOnPickup != -1)
            {
                FindObjectOfType<NotificationManager>(true).DecreaseReputation(reputationToDecreaseOnPickup);
            }

            if (onPickupEvent != null)
            {
                onPickupEvent.Invoke();
            }

            if (gold != -1)
            {
                FindObjectOfType<UIDocumentPlayerGold>(true).NotifyGold(gold);
                Player.instance.currentGold += gold;

            }

            // Is Alchemy Recipe?
            else if (alchemyRecipe != null)
            {
                Player.instance.alchemyRecipes.Add(alchemyRecipe);
            }
            else if (item != null) // Normal Item
            {
                playerInventory.AddItem(item, quantity);

                UIDocumentReceivedItemPrompt.ItemsReceived itemReceived = new UIDocumentReceivedItemPrompt.ItemsReceived();

                itemReceived.itemName = item.name.GetText();
                itemReceived.quantity = quantity;
                itemReceived.sprite = item.sprite;

                uIDocumentReceivedItemPrompt.itemsUI.Clear();
                uIDocumentReceivedItemPrompt.itemsUI.Add(itemReceived);


                uIDocumentReceivedItemPrompt.gameObject.SetActive(true);

                Soundbank.instance.PlayItemReceived();
            }
            else if (items.Length > 0)
            {
                uIDocumentReceivedItemPrompt.itemsUI.Clear();

                foreach (var item in items)
                {
                    UIDocumentReceivedItemPrompt.ItemsReceived itemReceived = new UIDocumentReceivedItemPrompt.ItemsReceived();

                    itemReceived.itemName = item.name.GetText();
                    itemReceived.quantity = 1;
                    itemReceived.sprite = item.sprite;

                    uIDocumentReceivedItemPrompt.itemsUI.Add(itemReceived);

                    playerInventory.AddItem(item, 1);


                }

                uIDocumentReceivedItemPrompt.gameObject.SetActive(true);

                Soundbank.instance.PlayItemReceived();
            }

            if (onPickupAfterConfirming != null)
            {
                uIDocumentReceivedItemPrompt.onConfirmEvent.AddListener(() =>
                {
                    onPickupAfterConfirming.Invoke();
                });
            }

            if (switchEntry == null)
            {
                return;
            }

            SwitchManager.instance.UpdateSwitch(switchEntry, true, null);

            if (gold != -1)
            {
                // Save Game to prevent us from dying and picking money infinitely
                SaveSystem.instance.SaveGameData(SceneManager.GetActiveScene().name);
            }

            if (!string.IsNullOrEmpty(analyticsMessage))
            {
                FindObjectOfType<Analytics>(true).TrackAnalyticsEvent(analyticsMessage);
            }

        }

        public void OnGameLoaded(GameData gameData)
        {
            if (GetComponentInParent<DynamicChest>() != null)
            {
                return;
            }

            Refresh();
        }

        public void OnReleased()
        {
            throw new NotImplementedException();
        }
    }
}
