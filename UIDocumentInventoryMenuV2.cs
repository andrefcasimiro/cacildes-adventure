using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{

    public class UIDocumentInventoryMenuV2 : MonoBehaviour
    {
        MenuManager menuManager;

        public List<string> filters = new List<string>();

        DropdownField filterDropdown;
        TextField textField;

        VisualElement root;

        public VisualTreeAsset itemButtonPrefab;

        private void Awake()
        {
            menuManager = FindObjectOfType<MenuManager>(true);
        }

        private void OnEnable()
        {
            this.root = GetComponent<UIDocument>().rootVisualElement;
            menuManager.SetupNavMenu(root);
            menuManager.SetActiveMenu(root, "ButtonInventory");

            filterDropdown = root.Q<DropdownField>("FilterDropdown");
            filterDropdown.RegisterValueChangedCallback(ev =>
            {
                UpdateItemsList();
            });
            filterDropdown.choices = filters;
            filterDropdown.value = "All";

            textField = root.Q<TextField>("FilterText");
            textField.value = "";
            textField.RegisterValueChangedCallback(ev =>
            {
                UpdateItemsList();
            });

            UpdateItemsList();
        }

        void UpdateItemsList()
        {
            var itemsToShow = GetItemsToShow();

            root.Q<ScrollView>().Clear();


            foreach (var item in itemsToShow)
            {
                VisualElement cloneButton = itemButtonPrefab.CloneTree();
                cloneButton.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(item.item.sprite);
                cloneButton.Q<Label>("ItemName").text = item.item.name + " ( " + item.amount + " )";

                var itemIsFavorited = Player.instance.favoriteItems.Contains(item.item);
                cloneButton.Q<VisualElement>("FavoriteIcon").style.display = itemIsFavorited ? DisplayStyle.Flex : DisplayStyle.None;

                var favoriteButton = cloneButton.Q<Button>("FavoriteButton");

                var consumable = item.item as Consumable;
                var isFavoritableConsumable = consumable != null && consumable.canFavorite;
                favoriteButton.style.display = isFavoritableConsumable ? DisplayStyle.Flex : DisplayStyle.None;

                if (isFavoritableConsumable)
                {
                    favoriteButton.Q<Label>().text = itemIsFavorited ? "Unfavorite" : "Favorite";

                    favoriteButton.RegisterCallback<ClickEvent>((ev) =>
                    {
                        var itemIsFavorited = Player.instance.favoriteItems.Contains(item.item);
                        if (itemIsFavorited)
                        {
                            FindObjectOfType<FavoriteItemsManager>(true).RemoveFavoriteItemFromList(item.item);
                        }
                        else
                        {
                            FindObjectOfType<FavoriteItemsManager>(true).AddFavoriteItemToList(item.item);
                        }

                        UpdateItemsList();
                    });
                }

                var useButton = cloneButton.Q<Button>("UseButton");
                useButton.style.display = consumable != null ? DisplayStyle.Flex : DisplayStyle.None;
                if (consumable != null)
                {
                    useButton.RegisterCallback<ClickEvent>(ev =>
                    {
                        consumable.OnConsume();

                        UpdateItemsList();
                    });
                }

                cloneButton.RegisterCallback<PointerOverEvent>(ev =>
                {
                    RenderItemPreview(item.item);
                });

                cloneButton.RegisterCallback<PointerOutEvent>(ev =>
                {
                    HideItemPreview();
                });
                
                root.Q<ScrollView>().Add(cloneButton);
            }
        }

        void RenderItemPreview(Item item)
        {
            var itemPreview = root.Q<VisualElement>("ItemPreview");
            itemPreview.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(item.sprite);
            itemPreview.Q<Label>("Title").text = item.name;
            itemPreview.Q<Label>("Description").text = item.description;


            itemPreview.style.opacity = 1;
        }

        void HideItemPreview()
        {
            root.Q<VisualElement>("ItemPreview").style.opacity = 0;
        }

        List<Player.ItemEntry> GetItemsToShow()
        {
            List<Player.ItemEntry> finalItems = new List<Player.ItemEntry>();
         
            List<Player.ItemEntry> list = new List<Player.ItemEntry>();

            if (filterDropdown.value.Length > 0)
            {
                if (filterDropdown.value == "All")
                {
                    foreach (var item in Player.instance.ownedItems)
                    {   
                        list.Add(item);
                    }
                }
                else if (filterDropdown.value == "Consumables")
                {
                    foreach (var i in GetUnknownItem<Consumable>())
                    {
                        list.Add(i);
                    }
                }
                else if (filterDropdown.value == "Weapons")
                {
                    foreach (var i in GetUnknownItem<Weapon>())
                    {
                        list.Add(i);
                    }
                }
                else if (filterDropdown.value == "Shields")
                {
                    foreach (var i in GetUnknownItem<Shield>())
                    {
                        list.Add(i);
                    }
                }
                else if (filterDropdown.value == "Helmets")
                {
                    foreach (var i in GetUnknownItem<Helmet>())
                    {
                        list.Add(i);
                    }
                }
                else if (filterDropdown.value == "Armors")
                {
                    foreach (var i in GetUnknownItem<Armor>())
                    {
                        list.Add(i);
                    }
                }
                else if (filterDropdown.value == "Armors")
                {
                    foreach (var i in GetUnknownItem<Gauntlet>())
                    {
                        list.Add(i);
                    }
                }
                else if (filterDropdown.value == "Legwear")
                {
                    foreach (var i in GetUnknownItem<Legwear>())
                    {
                        list.Add(i);
                    }
                }
                else if (filterDropdown.value == "Accessories")
                {
                    foreach (var i in GetUnknownItem<Accessory>())
                    {
                        list.Add(i);
                    }
                }
                else if (filterDropdown.value == "Alchemy Ingredients")
                {
                    foreach (var i in GetUnknownItem<AlchemyIngredient>())
                    {
                        list.Add(i);
                    }
                }
            }

            if (textField.value.Length > 0 && textField.value != "Search Item...")
            {
                foreach (var item in list)
                {
                    if (item.item.name.ToLower().Contains(textField.value.ToLower()))
                    {
                        finalItems.Add(item);
                    }
                }
            }
            else
            {
                return list;
            }

            return finalItems;
        }

        List<Player.ItemEntry> GetUnknownItem<T>() where T:Item
        {
            List<Player.ItemEntry> unknownList = new List<Player.ItemEntry>();
            foreach (var item in Player.instance.ownedItems)
            {
                var castItem = item.item as T;
                if (castItem != null)
                {
                    unknownList.Add(item);
                }
            }

            return unknownList;
        }

    }
    
}
