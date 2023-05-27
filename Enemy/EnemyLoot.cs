using System.Collections;
using System.Collections.Generic;
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

        private void Awake()
        {
             playerInventory = FindObjectOfType<PlayerInventory>(true);
             notificationManager = FindObjectOfType<NotificationManager>(true);
        }

        public IEnumerator GiveLoot()
        {
            yield return new WaitForSeconds(1f);

            var goldToReceive = Player.instance.CalculateAIGenericValue(enemyManager.enemy.baseGold, enemyManager.currentLevel);

            var equipmentGraphicsHandler = FindObjectOfType<EquipmentGraphicsHandler>(true);

            if (equipmentGraphicsHandler != null)
            {
                var additionalCoinPercentage = equipmentGraphicsHandler.additionalCoinPercentage;

                if (additionalCoinPercentage != 0)
                {
                    var additionalCoin = (int)Mathf.Ceil(goldToReceive * additionalCoinPercentage / 100);

                    goldToReceive += additionalCoin;
                }
            }

            goldToReceive += bonusGold;

            FindObjectOfType<UIDocumentPlayerGold>(true).NotifyGold(goldToReceive);
            Player.instance.currentGold += (int)goldToReceive;

            GetLoot();

            if (enemyBossController != null && enemyBossController.partner != null && enemyBossController.order == 1)
            {
                StartCoroutine(enemyBossController.partner.GetComponent<EnemyLoot>().GiveLoot());
            }
        }

        private void GetLoot()
        {
            var itemsToReceive = new List<Item>();

            bool hasPlayedFanfare = false;

            var finalLootTable = overrideLootTable ? additionalLootTable : enemyManager.enemy.lootTable;

            if (overrideLootTable == false && additionalLootTable.Count > 0)
            {
                foreach (var additionalLoot in additionalLootTable)
                {
                    finalLootTable.Add(additionalLoot);
                }
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
                    uIDocumentReceivedItemPrompt.itemsUI.Clear();
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

                    uIDocumentReceivedItemPrompt.itemsUI.Add(itemReceived);
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
