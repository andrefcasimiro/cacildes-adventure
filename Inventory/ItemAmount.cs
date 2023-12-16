
using UnityEngine;

namespace AF.Inventory
{
    [System.Serializable]
    public class ItemAmount
    {
        public int amount;

        // only applicable to items that are not lost upon use
        public int usages = 0;

        [Range(0, 100)] public int chanceToGet = 100;
    }
}
