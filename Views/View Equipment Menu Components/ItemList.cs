using System.Linq;
using AF.Equipment;
using AF.Stats;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF.UI.EquipmentMenu
{
    public class ItemList : MonoBehaviour
    {
        public enum EquipmentType
        {
            WEAPON,
            SHIELD,
            ARROW,
            SPELL,
            HELMET,
            ARMOR,
            GAUNTLET,
            BOOTS,
            ACCESSORIES,
            CONSUMABLES,
            OTHER_ITEMS,
        }

        ScrollView itemsScrollView;

        Label menuLabel;

        public const string SCROLL_ITEMS_LIST = "ItemsList";

        [Header("UI Components")]
        public VisualTreeAsset itemButtonPrefab;
        public ItemTooltip itemTooltip;
        public PlayerStatsAndAttributesUI playerStatsAndAttributesUI;
        public EquipmentSlots equipmentSlots;
        [Header("UI Documents")]
        public UIDocument uIDocument;
        public VisualElement root;


        [Header("Components")]
        public MenuManager menuManager;
        public CursorManager cursorManager;
        public StatsBonusController playerStatsBonusController;
        public AttackStatManager attackStatManager;
        public PlayerWeaponsManager playerWeaponsManager;
        public EquipmentGraphicsHandler equipmentGraphicsHandler;
        public PlayerLevelManager playerLevelManager;
        public DefenseStatManager defenseStatManager;
        public PlayerPoiseController playerPoiseController;
        public FavoriteItemsManager favoriteItemsManager;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;

        Button returnButton;

        [HideInInspector] public bool shouldRerender = true;

        private void OnEnable()
        {
            if (shouldRerender)
            {
                shouldRerender = false;

                SetupRefs();
            }

            returnButton.transform.scale = new Vector3(1, 1, 1);
            root.Q<VisualElement>("ItemList").style.display = DisplayStyle.Flex;
        }

        private void OnDisable()
        {
            root.Q<VisualElement>("ItemList").style.display = DisplayStyle.None;
        }

        public void SetupRefs()
        {
            root = uIDocument.rootVisualElement;
            menuLabel = root.Q<Label>("MenuLabel");

            returnButton = root.Q<Button>("ReturnButton");
            UIUtils.SetupButton(returnButton, () =>
            {
                ReturnToEquipmentSlots();
            });

            itemsScrollView = root.Q<ScrollView>(SCROLL_ITEMS_LIST);
        }


        void ReturnToEquipmentSlots()
        {
            equipmentSlots.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }

        public void DrawUI(EquipmentType equipmentType, int slotIndex)
        {
            menuLabel.style.display = DisplayStyle.None;

            if (equipmentType == EquipmentType.WEAPON)
            {
                PopulateScrollView<Weapon>(false, slotIndex);
            }
            else if (equipmentType == EquipmentType.SHIELD)
            {
                PopulateScrollView<Shield>(false, slotIndex);
            }
            else if (equipmentType == EquipmentType.ARROW)
            {
                PopulateScrollView<ConsumableProjectile>(false, slotIndex);
            }
            else if (equipmentType == EquipmentType.SPELL)
            {
                PopulateScrollView<Spell>(false, slotIndex);
            }
            else if (equipmentType == EquipmentType.HELMET)
            {
                PopulateScrollView<Helmet>(false, slotIndex);
            }
            else if (equipmentType == EquipmentType.ARMOR)
            {
                PopulateScrollView<Armor>(false, slotIndex);
            }
            else if (equipmentType == EquipmentType.GAUNTLET)
            {
                PopulateScrollView<Gauntlet>(false, slotIndex);
            }
            else if (equipmentType == EquipmentType.BOOTS)
            {
                PopulateScrollView<Legwear>(false, slotIndex);
            }
            else if (equipmentType == EquipmentType.ACCESSORIES)
            {
                PopulateScrollView<Accessory>(false, slotIndex);
            }
            else if (equipmentType == EquipmentType.CONSUMABLES)
            {
                PopulateScrollView<Consumable>(false, slotIndex);
            }
            else if (equipmentType == EquipmentType.OTHER_ITEMS)
            {
                PopulateScrollView<Item>(true, slotIndex);
            }

            // Delay the focus until the next frame, required as a hack for now
            Invoke(nameof(GiveFocus), 0f);
        }

        void GiveFocus()
        {
            returnButton.Focus();
        }

        void PopulateScrollView<T>(bool showOnlyKeyItems, int slotIndex) where T : Item
        {
            this.itemsScrollView.Clear();

            bool isEnglish = GamePreferences.instance.IsEnglish();

            bool isWeapon = typeof(T) == typeof(Weapon);
            bool isShield = typeof(T) == typeof(Shield);
            bool isHelmet = typeof(T) == typeof(Helmet);
            bool isArmor = typeof(T) == typeof(Armor);
            bool isGauntlet = typeof(T) == typeof(Gauntlet);
            bool isLegwear = typeof(T) == typeof(Legwear);
            bool isAccessory = typeof(T) == typeof(Accessory);

            if (isWeapon || isShield || isHelmet || isArmor || isGauntlet || isLegwear || isAccessory)
            {
                var instance = itemButtonPrefab.CloneTree();
                instance.Q<VisualElement>("Sprite").style.display = DisplayStyle.None;
                instance.Q<Label>("ItemName").text = isEnglish ? "Unequip" : "Desequipar";

                instance.Q<Button>("UseItemButton").style.display = DisplayStyle.None;

                instance.Q<Button>("EquipButton").clicked += () =>
                {
                    if (isWeapon)
                    {
                        playerWeaponsManager.UnequipWeapon(slotIndex);
                    }
                    else if (isShield)
                    {
                        playerWeaponsManager.UnequipShield(slotIndex);
                    }
                    else if (isHelmet)
                    {
                        equipmentGraphicsHandler.UnequipHelmet();
                    }
                    else if (isArmor)
                    {
                        equipmentGraphicsHandler.UnequipArmor();
                    }
                    else if (isGauntlet)
                    {
                        equipmentGraphicsHandler.UnequipGauntlet();
                    }
                    else if (isLegwear)
                    {
                        equipmentGraphicsHandler.UnequipLegwear();
                    }
                    else if (isAccessory)
                    {
                        equipmentGraphicsHandler.UnequipAccessory(slotIndex);
                    }

                    Soundbank.instance.PlayUIUnequip();

                    ReturnToEquipmentSlots();
                    //  RedrawUI();
                };

                instance.Q<VisualElement>("Indicator").style.display = DisplayStyle.None;
                instance.Q("Favorite").style.display = DisplayStyle.None;
                this.itemsScrollView.Add(instance);
            }

            foreach (var item in Player.instance.ownedItems)
            {
                if (item.item is not T typedItem)
                {
                    continue;
                }

                if (showOnlyKeyItems)
                {
                    if (item.item is Weapon || item.item is Shield || item.item is Helmet || item.item is Armor || item.item is Gauntlet || item.item is Legwear
                        || item.item is Accessory || item.item is Consumable || item.item is Spell)
                    {
                        continue;
                    }
                }

                var instance = itemButtonPrefab.CloneTree();
                instance.Q<VisualElement>("Sprite").style.backgroundImage = new StyleBackground(item.item.sprite);
                var itemName = instance.Q<Label>("ItemName");
                itemName.text = item.item.name.GetText();

                bool itemIsEquipped = false;

                if (item.item is Weapon weapon)
                {
                    itemIsEquipped = Player.instance.equippedWeapon != null && Player.instance.equippedWeapon.name.GetEnglishText() == weapon.name.GetEnglishText();

                    if (weapon.level > 1)
                    {
                        itemName.text += " +" + weapon.level;
                    }
                }
                else if (item.item is Shield shield)
                {
                    itemIsEquipped = Player.instance.equippedShield != null && Player.instance.equippedShield.name.GetEnglishText() == shield.name.GetEnglishText();
                }
                else if (item.item is Helmet helmet)
                {
                    itemIsEquipped = Player.instance.equippedHelmet != null && Player.instance.equippedHelmet.name.GetEnglishText() == helmet.name.GetEnglishText();
                }
                else if (item.item is Armor armor)
                {
                    itemIsEquipped = Player.instance.equippedArmor != null && Player.instance.equippedArmor.name.GetEnglishText() == armor.name.GetEnglishText();
                }
                else if (item.item is Gauntlet gauntlet)
                {
                    itemIsEquipped = Player.instance.equippedGauntlets != null && Player.instance.equippedGauntlets.name.GetEnglishText() == gauntlet.name.GetEnglishText();
                }
                else if (item.item is Legwear legwear)
                {
                    itemIsEquipped = Player.instance.equippedLegwear != null && Player.instance.equippedLegwear.name.GetEnglishText() == legwear.name.GetEnglishText();
                }
                else if (item.item is Accessory accessory)
                {
                    itemIsEquipped = Player.instance.IsAccessoryEquiped(accessory);
                }
                else
                {
                    itemName.text += " (" + item.amount + ")";
                }

                if (itemIsEquipped)
                {
                    instance.style.opacity = 1.5f;
                    itemName.text += GamePreferences.instance.IsEnglish() ? " (Equipped)" : " (Equipado)";
                }

                var equipmentColorIndicator = GetEquipmentColorIndicator(item.item);
                if (equipmentColorIndicator == Color.black)
                {
                    instance.Q<VisualElement>("Indicator").style.display = DisplayStyle.None;
                }
                else
                {
                    instance.Q<VisualElement>("Indicator").style.backgroundColor = GetEquipmentColorIndicator(item.item);
                    instance.Q<VisualElement>("Indicator").style.display = DisplayStyle.Flex;
                }

                var btn = instance.Q<Button>("EquipButton");
                btn.clicked += () =>
                {
                    Soundbank.instance.PlayUIEquip();

                    if (item.item is Weapon weapon)
                    {
                        playerWeaponsManager.EquipWeapon(weapon, slotIndex);
                    }
                    else if (item.item is Shield shield)
                    {
                        playerWeaponsManager.EquipShield(shield, slotIndex);
                    }
                    else if (item.item is Helmet helmet)
                    {
                        equipmentGraphicsHandler.EquipHelmet(helmet);
                    }
                    else if (item.item is Armor armor)
                    {
                        equipmentGraphicsHandler.EquipArmor(armor);
                    }
                    else if (item.item is Gauntlet gauntlet)
                    {
                        equipmentGraphicsHandler.EquipGauntlet(gauntlet);
                    }
                    else if (item.item is Legwear legwear)
                    {
                        equipmentGraphicsHandler.EquipLegwear(legwear);
                    }
                    else if (item.item is Accessory accessory)
                    {
                        equipmentGraphicsHandler.EquipAccessory(accessory, slotIndex);
                    }
                    else if (item.item is Consumable)
                    {
                        equipmentDatabase.EquipConsumable(item.item as Consumable, slotIndex);
                    }
                    else if (item.item is Spell)
                    {
                        equipmentDatabase.EquipSpell(item.item as Spell, slotIndex);
                    }

                    ReturnToEquipmentSlots();
                };

                instance.RegisterCallback<MouseEnterEvent>(ev =>
                {
                    itemTooltip.gameObject.SetActive(true);
                    itemTooltip.PrepareTooltipForItem(item.item);
                    itemTooltip.DisplayTooltip(btn);

                    playerStatsAndAttributesUI.DrawStats(item.item);
                });
                instance.RegisterCallback<MouseOutEvent>(ev =>
                {
                    itemTooltip.gameObject.SetActive(false);

                    playerStatsAndAttributesUI.DrawStats(null);
                });

                if (favoriteItemsManager.IsItemFavorited(item.item))
                {
                    instance.Q("Favorite").style.display = DisplayStyle.Flex;
                }
                else
                {
                    instance.Q("Favorite").style.display = DisplayStyle.None;
                }

                if (item.item is Consumable consumable)
                {
                    var useItemButton = instance.Q<Button>("UseItemButton");
                    useItemButton.Q<Label>().text = "Use";
                    useItemButton.style.display = DisplayStyle.Flex;

                    useItemButton.clicked += () =>
                    {
                        consumable.OnConsume();

                        menuManager.CloseMenu();

                        cursorManager.HideCursor();
                    };
                }
                else
                {
                    instance.Q<Button>("UseItemButton").style.display = DisplayStyle.None;
                }

                if (!itemIsEquipped && isAccessory && equipmentGraphicsHandler.CanEquipMoreAccessories())
                {
                    instance.SetEnabled(false);
                }


                this.itemsScrollView.Add(instance);
            }
        }

        public Color GetEquipmentColorIndicator<T>(T item) where T : Item
        {
            bool shouldReturn = false;
            int value = 0;
            if (item is Weapon weapon)
            {
                value = attackStatManager.CompareWeapon(weapon);
                shouldReturn = true;
            }
            else if (item is Helmet helmet)
            {
                value = defenseStatManager.CompareHelmet(helmet);
                shouldReturn = true;
            }
            else if (item is Armor armor)
            {
                value = defenseStatManager.CompareArmor(armor);
                shouldReturn = true;
            }
            else if (item is Gauntlet gauntlet)
            {
                value = defenseStatManager.CompareGauntlet(gauntlet);
                shouldReturn = true;
            }
            else if (item is Legwear legwear)
            {
                value = defenseStatManager.CompareLegwear(legwear);
                shouldReturn = true;
            }

            if (shouldReturn)
            {
                if (value > 0) return Color.green;
                else if (value == 0) return Color.yellow;
                else if (value < 0) return Color.red;
            }

            return Color.black;
        }
    }
}
