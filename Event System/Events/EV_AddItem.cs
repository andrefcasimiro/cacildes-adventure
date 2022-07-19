using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_AddItem : EventBase
    {
        public Item item;

        public override IEnumerator Dispatch()
        {
            yield return StartCoroutine(AddItem());
        }

        IEnumerator AddItem()
        {
            PlayerInventoryManager.instance.currentItems.Add(item);

            yield return null;
        }
    }

}
