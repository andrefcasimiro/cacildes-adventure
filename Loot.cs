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
        public int dropRarity;
    }

    public class Loot : MonoBehaviour
    {
        public List<DropCurrency> lootTable = new List<DropCurrency>();
        PlayerInventory playerInventory;


        private void Start()
        {
            playerInventory = FindObjectOfType<PlayerInventory>(true);
        }

        public void GetLoot()
        {
            foreach (DropCurrency dropCurrency in lootTable)
            {
                int calc_dropChance = Random.RandomRange(0, 101);

                if (calc_dropChance > dropCurrency.dropRarity)
                {
                    playerInventory.AddItem(dropCurrency.item, 1);

                }
            }
        }

    }
}