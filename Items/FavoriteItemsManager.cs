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

        PlayerSpellManager playerSpellManager => GetComponent<PlayerSpellManager>();

        MenuManager menuManager;

        private void Awake()
        {
             menuManager = FindObjectOfType<MenuManager>(true);
        }

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

                if (itemEntry.amount <= 1 && itemEntry.item.lostUponUse)
                {
                    RemoveFavoriteItemFromList(currentItem);
                }

                Consumable consumableItem = currentItem as Consumable;
                if (consumableItem != null)
                {
                    consumableItem.OnConsume();
                }
                else
                {
                    Spell spell = currentItem as Spell;

                    if (spell != null)
                    {
                        playerSpellManager.PrepareSpell(spell);
                    }
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

        public void SwitchToFavoriteItem(string itemName)
        {
            if (Player.instance.favoriteItems.Count <= 0)
            {
                return;
            }

            int i = Player.instance.favoriteItems.FindIndex(x => x.name.GetText() == itemName);
            if (i == -1)
            {
                return;
            }

            var thisItemInstance = Player.instance.favoriteItems.ElementAt(i);
            Player.instance.favoriteItems.RemoveAt(i);

            if (Player.instance.favoriteItems.Count > 0)
            {
                Item itemAtFirstIndex = Player.instance.favoriteItems.ElementAt(0);
                Player.instance.favoriteItems[0] = thisItemInstance;
                Player.instance.favoriteItems.Add(itemAtFirstIndex);
            }
            else
            {
                Player.instance.favoriteItems.Add(thisItemInstance);
            }

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

        public void UpdateFavoriteItems()
        {
            uIDocumentPlayerHUDV2.UpdateFavoriteItems();
        }
    }

}