using System.Collections;

namespace AF
{
    public class EV_Condition_HasItem : EV_Condition
    {
        public Item item;

        public override IEnumerator Dispatch()
        {
            yield return DispatchConditionResults(Player.instance.ownedItems.Exists(x => x.item == item));
        }
    }
}
