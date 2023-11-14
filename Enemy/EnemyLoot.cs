using System.Collections;
using System.Collections.Generic;
using AF.Stats;
using UnityEngine;

namespace AF
{
    public class EnemyLoot : MonoBehaviour
    {
        [System.Serializable]
        public class DropCurrency
        {
            public Item item;

            [Range(0, 100)]
            public float chanceToGet;
        }

        [Header("Additional Loot")]
        public List<DropCurrency> additionalLootTable = new List<DropCurrency>();
        [Tooltip("If set, will ignore the enemy's default loot table for the additionalLootTable")] public bool overrideLootTable = false;

        EnemyManager enemyManager => GetComponent<EnemyManager>();

        PlayerInventory playerInventory;
        NotificationManager notificationManager;
        EnemyBossController enemyBossController => GetComponent<EnemyBossController>();

        public int bonusGold = 0;

        EquipmentGraphicsHandler equipmentGraphicsHandler;
        UIDocumentPlayerGold uIDocumentPlayerGold;

        [Header("Components")]
        public StatsBonusController playerStatsBonusController;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;

        private void Awake()
        {
            playerInventory = FindFirstObjectByType<PlayerInventory>(FindObjectsInactive.Include);
            notificationManager = FindFirstObjectByType<NotificationManager>(FindObjectsInactive.Include);
            equipmentGraphicsHandler = FindFirstObjectByType<EquipmentGraphicsHandler>(FindObjectsInactive.Include);
            uIDocumentPlayerGold = FindFirstObjectByType<UIDocumentPlayerGold>(FindObjectsInactive.Include);

        }

        public IEnumerator GiveLoot()
        {
            yield return new WaitForSeconds(1f);

            var goldToReceive = enemyManager.enemy.baseGold;

            if (enemyManager.currentLevel > 1)
            {
                goldToReceive = Formulas.CalculateAIGenericValue(enemyManager.enemy.baseGold, enemyManager.currentLevel);
            }


            if (playerStatsBonusController != null)
            {
                var additionalCoinPercentage = playerStatsBonusController.additionalCoinPercentage;

                if (additionalCoinPercentage != 0)
                {
                    var additionalCoin = (int)Mathf.Ceil(goldToReceive * additionalCoinPercentage / 100);

                    goldToReceive += additionalCoin;
                }
            }

            goldToReceive += bonusGold;


            GetLoot();

            yield return new WaitForSeconds(0.2f);

            uIDocumentPlayerGold.PlayCoinsFX(transform);
            uIDocumentPlayerGold.NotifyGold(goldToReceive);
            playerStatsDatabase.gold += (int)goldToReceive;
        }

        private void GetLoot()
        {
            var itemsToReceive = new List<Item>();

            bool hasPlayedFanfare = false;

            var provisionalLootTable = overrideLootTable ? additionalLootTable : enemyManager.enemy.lootTable;

            if (overrideLootTable == false && additionalLootTable.Count > 0)
            {
                foreach (var additionalLoot in additionalLootTable)
                {
                    provisionalLootTable.Add(additionalLoot);
                }
            }

            var finalLootTable = new List<DropCurrency>();

            // Filter armors or weapons that user might already have
            foreach (var dropCurrency in provisionalLootTable)
            {

                if (dropCurrency.item is ArmorBase || dropCurrency.item is Weapon)
                {
                    if (playerInventory.GetItemQuantity(dropCurrency.item) > 0)
                    {
                        // If player owns armor, dont allow him to have multiple armors of that kind
                        continue;
                    }
                }

                finalLootTable.Add(dropCurrency);
            }



            foreach (DropCurrency dropCurrency in finalLootTable)
            {
                float calc_dropChance = Random.Range(0, 100f);

                if (calc_dropChance <= dropCurrency.chanceToGet)
                {
                    if (hasPlayedFanfare == false)
                    {

                        Soundbank.instance.PlayItemReceived();
                        hasPlayedFanfare = true;
                    }

                    itemsToReceive.Add(dropCurrency.item);
                }
            }

            UIDocumentReceivedItemPrompt uIDocumentReceivedItemPrompt = null;
            bool isBoss = enemyBossController != null;

            if (isBoss)
            {
                uIDocumentReceivedItemPrompt = FindObjectOfType<UIDocumentReceivedItemPrompt>(true);

                if (uIDocumentReceivedItemPrompt != null)
                {
                    //uIDocumentReceivedItemPrompt.itemsUI.Clear();
                }
            }

            foreach (var item in itemsToReceive)
            {
                playerInventory.AddItem(item, 1);

                if (isBoss && uIDocumentReceivedItemPrompt != null)
                {
                    UIDocumentReceivedItemPrompt.ItemsReceived itemReceived = new UIDocumentReceivedItemPrompt.ItemsReceived();

                    itemReceived.itemName = item.name.GetText();
                    itemReceived.quantity = 1;
                    itemReceived.sprite = item.sprite;

                    //uIDocumentReceivedItemPrompt.itemsUI.Add(itemReceived);
                }
                else
                {
                    notificationManager.ShowNotification(LocalizedTerms.Found() + " " + item.name.GetText(), item.sprite);
                }
            }

            if (isBoss && itemsToReceive.Count > 0)
            {
                uIDocumentReceivedItemPrompt.gameObject.SetActive(true);
            }
        }

    }
}
