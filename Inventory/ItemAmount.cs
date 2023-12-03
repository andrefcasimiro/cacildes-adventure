namespace AF.Inventory
{
    [System.Serializable]
    public class ItemAmount
    {
        public int amount;

        // only applicable to items that are not lost upon use
        public int usages = 0;
    }
}
