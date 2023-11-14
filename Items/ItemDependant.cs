using AF.Inventory;
using UnityEngine;

namespace AF
{

    public class ItemDependant : MonoBehaviour
    {
        public Item item;

        [Header("Databases")]
        public InventoryDatabase inventoryDatabase;

        private void Start()
        {
            this.transform.GetChild(0).gameObject.SetActive(inventoryDatabase.ownedItems.Find(x => x.item.name.GetEnglishText() == item.name.GetEnglishText()) != null);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                this.transform.GetChild(0).gameObject.SetActive(inventoryDatabase.ownedItems.Find(x => x.item.name.GetEnglishText() == item.name.GetEnglishText()) != null);
            }
        }
    }

}
