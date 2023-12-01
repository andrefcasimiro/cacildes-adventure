using AF.Inventory;

namespace AF.Conditions
{
    public class Condition_HasItem : ConditionBase
    {
        public InventoryDatabase inventoryDatabase;
        public Item requiredItem;

        public override bool IsConditionMet()
        {
            return inventoryDatabase.HasItem(requiredItem);
        }
    }
}
