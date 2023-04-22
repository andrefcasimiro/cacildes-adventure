using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class ItemDependant : MonoBehaviour
    {
        public Item item;

        private void Start()
        {
            this.transform.GetChild(0).gameObject.SetActive(Player.instance.ownedItems.Find(x => x.item.name.GetEnglishText() == item.name.GetEnglishText()) != null);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                this.transform.GetChild(0).gameObject.SetActive(Player.instance.ownedItems.Find(x => x.item.name.GetEnglishText() == item.name.GetEnglishText()) != null);
            }
        }
    }

}
