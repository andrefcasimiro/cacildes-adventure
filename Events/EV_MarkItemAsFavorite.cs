using System.Collections;
namespace AF
{
    public class EV_MarkItemAsFavorite : EventBase
    {
        public Consumable item;
        public int consumableSlotToEquipItem = 0;

        public EquipmentDatabase equipmentDatabase;

        public override IEnumerator Dispatch()
        {
            if (equipmentDatabase != null)
            {
                equipmentDatabase.EquipConsumable(item, consumableSlotToEquipItem);
            }

            yield return null;
        }
    }
}
