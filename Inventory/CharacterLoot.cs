using System.Collections;
using System.Collections.Generic;
using AF.Inventory;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Localization;

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

        // Scene References
        private PlayerManager playerManager;
        private NotificationManager notificationManager;

        private Soundbank soundbank;

        private UIDocumentPlayerGold uIDocumentPlayerGold;
        private UIDocumentReceivedItemPrompt uIDocumentReceivedItemPrompt;

        [Header("Localization")]
        // "Found: "
        public LocalizedString found;


        public void GiveLoot()
        {
            StartCoroutine(GiveLoot_Coroutine());
        }

        public IEnumerator GiveLoot_Coroutine()
        {
            int goldToReceive = baseGold + bonusGold;

            yield return new WaitForSeconds(1f);

            if (GetPlayerManager().statsBonusController != null)
            {
                var additionalCoinPercentage = GetPlayerManager().statsBonusController.additionalCoinPercentage;

                if (additionalCoinPercentage != 0)
                {
                    var additionalCoin = (int)Mathf.Ceil(goldToReceive * additionalCoinPercentage / 100);

                    goldToReceive += additionalCoin;
                }

                if (GetPlayerManager().statsBonusController.ShouldDoubleCoinFromFallenEnemy())
                {
                    goldToReceive *= 2;
                }
            }

            GetLoot();

            yield return new WaitForSeconds(0.2f);

            GetUIDocumentPlayerGold().AddGold(goldToReceive);
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

                        GetSoundbank().PlaySound(GetSoundbank().uiItemReceived);
                        hasPlayedFanfare = true;
                    }

                    itemsToReceive.Add(dropCurrency.Key, dropCurrency.Value);
                }
            }

            bool isBoss = lootOwner.characterBossController.IsBoss();

            List<UIDocumentReceivedItemPrompt.ItemsReceived> itemsToDisplay = new();
            foreach (var item in itemsToReceive)
            {
                GetPlayerManager().playerInventory.AddItem(item.Key, item.Value.amount);

                if (isBoss && GetUIDocumentReceivedItemPrompt() != null)
                {
                    itemsToDisplay.Add(new()
                    {
                        itemName = item.Key.GetName(),
                        quantity = 1,
                        sprite = item.Key.sprite
                    });
                }
                else
                {
                    GetNotificationManager().ShowNotification(found.GetLocalizedString() + " " + item.Key.GetName(), item.Key.sprite);
                }
            }

            if (isBoss && itemsToDisplay.Count > 0)
            {
                GetUIDocumentReceivedItemPrompt().gameObject.SetActive(true);

                GetUIDocumentReceivedItemPrompt().DisplayItemsReceived(itemsToDisplay);
            }
        }

        PlayerManager GetPlayerManager()
        {
            if (playerManager == null)
            {
                playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include);
            }

            return playerManager;
        }

        Soundbank GetSoundbank()
        {
            if (soundbank == null)
            {
                soundbank = FindAnyObjectByType<Soundbank>(FindObjectsInactive.Include);
            }

            return soundbank;
        }

        NotificationManager GetNotificationManager()
        {
            if (notificationManager == null)
            {
                notificationManager = FindAnyObjectByType<NotificationManager>(FindObjectsInactive.Include);
            }

            return notificationManager;
        }

        UIDocumentPlayerGold GetUIDocumentPlayerGold()
        {
            if (uIDocumentPlayerGold == null)
            {
                uIDocumentPlayerGold = FindAnyObjectByType<UIDocumentPlayerGold>(FindObjectsInactive.Include);
            }

            return uIDocumentPlayerGold;
        }

        UIDocumentReceivedItemPrompt GetUIDocumentReceivedItemPrompt()
        {
            if (uIDocumentReceivedItemPrompt == null)
            {
                uIDocumentReceivedItemPrompt = FindAnyObjectByType<UIDocumentReceivedItemPrompt>(FindObjectsInactive.Include);
            }

            return uIDocumentReceivedItemPrompt;
        }
    }
}
