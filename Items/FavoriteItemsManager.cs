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

        MenuManager menuManager => FindObjectOfType<MenuManager>(true);

        // Start is called before the first frame update
        void Start()
        {
            uIDocumentPlayerHUDV2 = FindObjectOfType<UIDocumentPlayerHUDV2>(true);
        }

        // Update is called once per frame
        void Update()
        {
            // Dont allow consuming items when menu is opened, we might be searching in the textbox using the 'R' or 'Q' key

            if (inputs.consumeFavoriteItem)
            {
                inputs.consumeFavoriteItem = false;

                if (menuManager.IsMenuOpen())
                {
                    return;
                }

                ConsumeFavoriteItem();
            }
            else if (inputs.switchFavoriteItem)
            {
                inputs.switchFavoriteItem = false;

                if (menuManager.IsMenuOpen())
                {
                    return;
                }

                SwitchFavoriteItemsOrder();

                Soundbank.instance.PlayQuickItemSwitch();
            }
        }

        public void ConsumeFavoriteItem()
        {
            if (Player.instance.favoriteItems.Count > 0)
            {
                Item currentItem = Player.instance.favoriteItems[0];

                Player.ItemEntry itemEntry = Player.instance.ownedItems.Find(item => item.item.name.GetEnglishText() == currentItem.name.GetEnglishText());

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