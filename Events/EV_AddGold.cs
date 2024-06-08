using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class EV_AddGold : EventBase
    {
        public int goldToAdd = 0;

        public UnityEvent onGoldAdd;

        public override IEnumerator Dispatch()
        {
            AddGold();
            yield return null;
        }

        void AddGold()
        {
            FindAnyObjectByType<UIDocumentPlayerGold>(FindObjectsInactive.Include)?.AddGold(goldToAdd);

            onGoldAdd?.Invoke();
        }
    }
}
