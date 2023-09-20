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


        float inputDelayForSwitchingFavItems = Mathf.Infinity;
        float maxInputDelayForSwitchingFavItems = 0.1f;

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


            if (inputDelayForSwitchingFavItems < maxInputDelayForSwitchingFavItems)
            {
                inputDelayForSwitchingFavItems += Time.deltaTime;
            }

            if (inputs.consumeFavoriteItem)
            {
                inputs.consumeFavoriteItem = false;

                if (menuManager.IsMenuOpen())
                {
                    return;
                }

                ConsumeFavoriteItem();
            }
            else if (inputs.switchFavoriteItemNext || inputs.switchFavoriteItemBack)
            {

                int direction = inputs.switchFavoriteItemNext == true ? 1 : 0;
                inputs.switchFavoriteItemNext = false;
                inputs.switchFavoriteItemBack = false;

                if (inputDelayForSwitchingFavItems < maxInputDelayForSwitchingFavItems)
                {
                    return;
                }

                if (menuManager.IsMenuOpen())
                {
                    return;
                }

                SwitchFavoriteItemsOrder(direction);

                Soundbank.instance.PlayQuickItemSwitch();
                inputDelayForSwitchingFavItems = 0;
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

        public void SwitchFavoriteItemsOrder(int direction)
        {
            if (Player.instance.favoriteItems.Count <= 0)
            {
                return;
            }

            if (direction == 1)
            {
                Item firstItem = Player.instance.favoriteItems.First();
                Player.instance.favoriteItems.Remove(firstItem);
                Player.instance.favoriteItems.Add(firstItem);
            }
            else
            {
                Item lastItem = Player.instance.favoriteItems.Last();
                Player.instance.favoriteItems.Remove(lastItem);
                Player.instance.favoriteItems.Insert(0, lastItem);
            }

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
            var idx = Player.instance.favoriteItems.FindIndex(x => x.name.GetEnglishText() == item.name.GetEnglishText());
            if (idx != -1)
            {
                Player.instance.favoriteItems.RemoveAt(idx);
            }

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

        public bool IsItemFavorited(Item item) {
            return Player.instance.favoriteItems.FirstOrDefault(x => x.name.GetEnglishText() == item.name.GetEnglishText()) != null;
        }
    }

}