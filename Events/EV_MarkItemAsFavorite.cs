using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_MarkItemAsFavorite : EventBase
    {
        public Item item;
        public int consumableSlotToEquipItem = 0;

        public override IEnumerator Dispatch()
        {
            FindObjectOfType<FavoriteItemsManager>(true).AddFavoriteItemToList(item, consumableSlotToEquipItem);

            yield return null;
        }
    }

}
