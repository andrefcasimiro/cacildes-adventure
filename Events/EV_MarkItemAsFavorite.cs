using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_MarkItemAsFavorite : EventBase
    {
        public Item item;

        public override IEnumerator Dispatch()
        {
            FindObjectOfType<FavoriteItemsManager>(true).AddFavoriteItemToList(item);

            yield return null;
        }
    }

}
