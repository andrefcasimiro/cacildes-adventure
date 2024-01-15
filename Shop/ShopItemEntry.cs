using UnityEngine;

namespace AF.Shops
{

    [System.Serializable]
    public class ShopItemEntry
    {
        public Item item;

        /// <summary>
        /// If true, it won't be shown in the list if the player already owns it
        /// </summary>
        public bool isUnique = false;

        public int quantity = 1;
    }

}