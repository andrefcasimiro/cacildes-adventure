using System.Collections;
using System.Collections.Generic;
using AF.Inventory;
using AF.Stats;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace AF
{
    public class CharacterLoot : MonoBehaviour
    {

        [Header("Loot and Experience")]

        [SerializedDictionary("Item", "Chance To Get")]
        public SerializedDictionary<Item, ItemAmount> lootTable;

        public int baseGold = 100;
        public int bonusGold = 0;

        [Header("Components")]
        public CharacterManager lootOwner;
        public PlayerManager playerManager;
        public NotificationManager notificationManager;

        public StatsBonusController playerStatsBonusController;
        public Soundbank soundbank;

        [Header("UI Components")]
        public UIDocumentPlayerGold uIDocumentPlayerGold;
        public UIDocumentReceivedItemPrompt uIDocumentReceivedItemPrompt;

        public void GiveLoot()
        {
            StartCoroutine(GiveLoot_Coroutine());
        }

        public IEnumerator GiveLoot_Coroutine()
        {
            int goldToReceive = baseGold + bonusGold;

            yield return new WaitForSeconds(1f);

            if (playerStatsBonusController != null)
            {
                var additionalCoinPercentage = playerStatsBonusController.additionalCoinPercentage;

                if (additionalCoinPercentage != 0)
                {
                    var additionalCoin = (int)Mathf.Ceil(goldToReceive * additionalCoinPercentage / 100);

                    goldToReceive += additionalCoin;
                }

                if (playerStatsBonusController.ShouldDoubleCoinFromFallenEnemy())
                {
                    goldToReceive *= 2;
                }
            }

            GetLoot();

            yield return new WaitForSeconds(0.2f);

            uIDocumentPlayerGold.AddGold(goldToReceive);
        }

        private void GetLoot()
        {
            var itemsToReceive = new SerializedDictionary<Item, ItemAmount>();

            bool hasPlayedFanfare = false;

            foreach (var dropCurrency in lootTable)
            {
                float calc_dropChance = Random.Range(0, 100f);

                if (calc_dropChance <= dropCurrency.Value.chanceToGet)
                {
                    if (hasPlayedFanfare == false)
                    {

                        soundbank.PlaySound(soundbank.uiItemReceived);
                        hasPlayedFanfare = true;
                    }

                    itemsToReceive.Add(dropCurrency.Key, dropCurrency.Value);
                }
            }

            bool isBoss = lootOwner.characterBossController.IsBoss();

            List<UIDocumentReceivedItemPrompt.ItemsReceived> itemsToDisplay = new();
            foreach (var item in itemsToReceive)
            {
                playerManager.playerInventory.AddItem(item.Key, item.Value.amount);

                if (isBoss && uIDocumentReceivedItemPrompt != null)
                {
                    itemsToDisplay.Add(new()
                    {
                        itemName = item.Key.name,
                        quantity = 1,
                        sprite = item.Key.sprite
                    });
                }
                else
                {
                    notificationManager.ShowNotification("Found " + item.Key.name, item.Key.sprite);
                }
            }

            if (isBoss && itemsToDisplay.Count > 0)
            {
                uIDocumentReceivedItemPrompt.gameObject.SetActive(true);

                uIDocumentReceivedItemPrompt.DisplayItemsReceived(itemsToDisplay);
            }
        }
    }
}
