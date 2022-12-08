using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AF
{

    public class FavoriteItemsManager : MonoBehaviour
    {
        UIDocumentPlayerHUDV2 uIDocumentPlayerHUDV2;

        public StarterAssets.StarterAssetsInputs inputs;

        // Start is called before the first frame update
        void Start()
        {
            uIDocumentPlayerHUDV2 = FindObjectOfType<UIDocumentPlayerHUDV2>(true);
        }

        // Update is called once per frame
        void Update()
        {
            if (inputs.consumeFavoriteItem)
            {
                inputs.consumeFavoriteItem = false;
                ConsumeFavoriteItem();
            }
            else if (inputs.switchFavoriteItem)
            {
                inputs.switchFavoriteItem = false;

                SwitchFavoriteItemsOrder();
            }
        }

        public void ConsumeFavoriteItem()
        {
            if (Player.instance.favoriteItems.Count > 0)
            {
                Item currentItem = Player.instance.favoriteItems[0];

                Player.ItemEntry itemEntry = Player.instance.ownedItems.Find(item => item.item == currentItem);

                if (itemEntry.amount <= 1)
                {
                    RemoveFavoriteItemFromList(currentItem);
                }

                Consumable consumableItem = (Consumable)currentItem;
                if (consumableItem != null)
                {
                    consumableItem.OnConsume();
                }

            }

            uIDocumentPlayerHUDV2.UpdateFavoriteItems();
        }

        public void SwitchFavoriteItemsOrder()
        {
            if (Player.instance.favoriteItems.Count <= 0)
            {
                return;
            }

            Item firstItem = Player.instance.favoriteItems.First();
            Player.instance.favoriteItems.Remove(firstItem);
            Player.instance.favoriteItems.Add(firstItem);

            uIDocumentPlayerHUDV2.UpdateFavoriteItems();
        }

        public void RemoveFavoriteItemFromList(Item item)
        {
            Player.instance.favoriteItems.Remove(item);

            uIDocumentPlayerHUDV2.UpdateFavoriteItems();
        }
        public void AddFavoriteItemToList(Item item)
        {
            Player.instance.favoriteItems.Add(item);

            uIDocumentPlayerHUDV2.UpdateFavoriteItems();
        }
    }

}