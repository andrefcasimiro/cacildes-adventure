namespace AF.Inventory
{
    [System.Serializable]
    public class ItemEntry
    {
        public Item item;

        public int amount;

        // only applicable to items that are not lost upon use
        public int usages = 0;
    }
}
