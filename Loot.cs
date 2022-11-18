using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace AF
{
    [System.Serializable]
    public class DropCurrency
    {
        public Item item;

        [Range(0, 100)]
        public int chanceToGet;
    }

    public class Loot : MonoBehaviour
    {
        public List<DropCurrency> lootTable = new List<DropCurrency>();
        PlayerInventory playerInventory;
        NotificationManager notificationManager;

        private void Start()
        {
            notificationManager = FindObjectOfType<NotificationManager>(true);
            playerInventory = FindObjectOfType<PlayerInventory>(true);
        }

        public void GetLoot()
        {
            bool hasPlayedFanfare = false;
            foreach (DropCurrency dropCurrency in lootTable)
            {
                int calc_dropChance = Random.RandomRange(0, 101);

                if (calc_dropChance < dropCurrency.chanceToGet)
                {
                    if (hasPlayedFanfare == false)
                    {

                        BGMManager.instance.PlayItem();
                        hasPlayedFanfare = true;
                    }

                    playerInventory.AddItem(dropCurrency.item, 1);
                    notificationManager.ShowNotification("Found " + dropCurrency.item.name, dropCurrency.item.sprite);
                }
            }
        }

    }
}