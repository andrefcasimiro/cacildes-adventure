using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        [Header("Localization")]
        public LocalizedText searchItemLabel;


        private void Awake()
        {
            menuManager = FindObjectOfType<MenuManager>(true);
        }

        private void OnEnable()
        {
            this.root = GetComponent<UIDocument>().rootVisualElement;
            menuManager.SetupNavMenu(root);
            menuManager.SetActiveMenu(root, "ButtonInventory");
            menuManager.TranslateNavbar(root);

            filterDropdown = root.Q<DropdownField>("FilterDropdown");
            filterDropdown.RegisterValueChangedCallback(ev =>
            {
                UpdateItemsList();
            });

            
            filterDropdown.choices = new List<string>{
                    LocalizedTerms.ShowAll(),
                    LocalizedTerms.ShowConsumables(),
                    LocalizedTerms.Weapon() + "s",
                    LocalizedTerms.Shield() + "s",
                    LocalizedTerms.Helmet() + "s",
                    LocalizedTerms.Armor() + "s",
                    LocalizedTerms.Gauntlets(),
                    LocalizedTerms.Boots(),
                    LocalizedTerms.Accessory() + "s",
                    LocalizedTerms.ShowAlchemyIngredients(),
                    LocalizedTerms.ShowCookingIngredients(),
                    LocalizedTerms.ShowCraftingMaterials(),
                    LocalizedTerms.ShowKeyItems(),
            };

            filterDropdown.value = LocalizedTerms.ShowAll();

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
                cloneButton.Q<Label>("ItemName").text = item.item.name.GetText() + " ( " + item.amount + " )";

                cloneButton.RegisterCallback<PointerOverEvent>(ev =>
                {
                    root.Q<ScrollView>().ScrollTo(cloneButton);

                    RenderItemPreview(item.item);
                });

                cloneButton.RegisterCallback<PointerOutEvent>(ev =>
                {
                    HideItemPreview();
                });

                cloneButton.RegisterCallback<FocusInEvent>(ev =>
                {
                    RenderItemPreview(item.item);

                    root.Q<ScrollView>().ScrollTo(cloneButton);
                });

                cloneButton.RegisterCallback<FocusOutEvent>(ev =>
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
            itemPreview.Q<Label>("Title").text = item.name.GetText();
            itemPreview.Q<Label>("Description").text = item.description.GetText();


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
                if (filterDropdown.value == LocalizedTerms.ShowAll())
                {
                    foreach (var item in Player.instance.ownedItems)
                    {   
                        list.Add(item);
                    }
                }
                else if (filterDropdown.value == LocalizedTerms.ShowConsumables())
                {
                    foreach (var i in GetUnknownItem<Consumable>())
                    {
                        list.Add(i);
                    }
                }
                else if (filterDropdown.value == LocalizedTerms.Weapon() + "s")
                {
                    foreach (var i in GetUnknownItem<Weapon>())
                    {
                        list.Add(i);
                    }
                }
                else if (filterDropdown.value == LocalizedTerms.Shield() + "s")
                {
                    foreach (var i in GetUnknownItem<Shield>())
                    {
                        list.Add(i);
                    }
                }
                else if (filterDropdown.value == LocalizedTerms.Helmet() + "s")
                {
                    foreach (var i in GetUnknownItem<Helmet>())
                    {
                        list.Add(i);
                    }
                }
                else if (filterDropdown.value == LocalizedTerms.Armor() + "s")
                {
                    foreach (var i in GetUnknownItem<Armor>())
                    {
                        list.Add(i);
                    }
                }
                else if (filterDropdown.value == LocalizedTerms.Gauntlets())
                {
                    foreach (var i in GetUnknownItem<Gauntlet>())
                    {
                        list.Add(i);
                    }
                }
                else if (filterDropdown.value == LocalizedTerms.Boots())
                {
                    foreach (var i in GetUnknownItem<Legwear>())
                    {
                        list.Add(i);
                    }
                }
                else if (filterDropdown.value == LocalizedTerms.Accessory() + "s")
                {
                    foreach (var i in GetUnknownItem<Accessory>())
                    {
                        list.Add(i);
                    }
                }
                else if (filterDropdown.value == LocalizedTerms.ShowAlchemyIngredients())
                {
                    foreach (var i in GetUnknownItem<AlchemyIngredient>())
                    {
                        list.Add(i);
                    }
                }
                else if (filterDropdown.value == LocalizedTerms.ShowCookingIngredients())
                {
                    foreach (var i in GetUnknownItem<CookingIngredient>())
                    {
                        list.Add(i);
                    }
                }
                else if (filterDropdown.value == LocalizedTerms.ShowCraftingMaterials())
                {
                    foreach (var i in GetUnknownItem<CraftingMaterial>())
                    {
                        list.Add(i);
                    }
                }
                else if (filterDropdown.value == LocalizedTerms.ShowKeyItems())
                {
                }
            }

            if (textField.value.Length > 0 && textField.value != searchItemLabel.GetText())
            {
                foreach (var item in list)
                {
                    if (item.item.name.GetText().ToLower().Contains(textField.value.ToLower()))
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
