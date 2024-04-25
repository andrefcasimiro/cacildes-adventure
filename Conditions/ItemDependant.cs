using AF.Inventory;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Conditions
{
    public class ItemDependant : MonoBehaviour
    {
        public Item item;

        public InventoryDatabase inventoryDatabase;

        [Header("Events")]
        public UnityEvent onTrue;
        public UnityEvent onFalse;

        private void Awake()
        {
            Evaluate();
        }

        public void Evaluate()
        {
            if (inventoryDatabase.HasItem(item))
            {
                onTrue?.Invoke();
            }
            else
            {
                onFalse?.Invoke();
            }
        }
    }
}
