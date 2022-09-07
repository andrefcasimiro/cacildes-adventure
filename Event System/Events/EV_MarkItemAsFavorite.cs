using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_MarkItemAsFavorite : EventBase
    {
        public Item item;

        public override IEnumerator Dispatch()
        {
            yield return StartCoroutine(AddItem());
        }

        IEnumerator AddItem()
        {
            PlayerInventoryManager.instance.FavoriteItemAndSetItAsCurrent(item);

            yield return null;
        }
    }

}
