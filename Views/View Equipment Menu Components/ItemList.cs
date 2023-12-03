using AF.Equipment;
using AF.Inventory;
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
        public PlayerManager playerManager;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;
        public PlayerStatsDatabase playerStatsDatabase;
        public InventoryDatabase inventoryDatabase;

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
                PopulateScrollView<Arrow>(false, slotIndex);
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
            bool isArrow = typeof(T) == typeof(Arrow);
            bool isConsumable = typeof(T) == typeof(Consumable);

            if (isWeapon || isShield || isHelmet || isArmor || isGauntlet || isLegwear || isAccessory || isArrow || isConsumable)
            {
                var instance = itemButtonPrefab.CloneTree();
                instance.Q<VisualElement>("Sprite").style.display = DisplayStyle.None;
                instance.Q<Label>("ItemName").text = isEnglish ? "Unequip" : "Desequipar";

                instance.Q<Button>("UseItemButton").style.display = DisplayStyle.None;

                instance.Q<Button>("EquipButton").clicked += () =>
                {
                    if (isWeapon)
                    {
                        playerManager.playerWeaponsManager.UnequipWeapon(slotIndex);
                    }
                    else if (isShield)
                    {
                        playerManager.playerWeaponsManager.UnequipShield(slotIndex);
                    }
                    else if (isHelmet)
                    {
                        playerManager.equipmentGraphicsHandler.UnequipHelmet();
                    }
                    else if (isArmor)
                    {
                        playerManager.equipmentGraphicsHandler.UnequipArmor();
                    }
                    else if (isGauntlet)
                    {
                        playerManager.equipmentGraphicsHandler.UnequipGauntlet();
                    }
                    else if (isLegwear)
                    {
                        playerManager.equipmentGraphicsHandler.UnequipLegwear();
                    }
                    else if (isAccessory)
                    {
                        playerManager.equipmentGraphicsHandler.UnequipAccessory(slotIndex);
                    }
                    else if (isArrow)
                    {
                        equipmentDatabase.UnequipArrow(slotIndex);
                    }
                    else if (isConsumable)
                    {
                        equipmentDatabase.UnequipConsumable(slotIndex);
                    }

                    Soundbank.instance.PlayUIUnequip();

                    //ReturnToEquipmentSlots();
                    //  RedrawUI();
                };

                instance.Q<VisualElement>("Indicator").style.display = DisplayStyle.None;
                instance.Q("Favorite").style.display = DisplayStyle.None;
                this.itemsScrollView.Add(instance);
            }

            foreach (var item in inventoryDatabase.ownedItems)
            {

                if (item.Key is not T typedItem)
                {
                    continue;
                }

                if (showOnlyKeyItems)
                {
                    if (item.Key is Weapon || item.Key is Shield || item.Key is Helmet || item.Key is Armor || item.Key is Gauntlet || item.Key is Legwear
                        || item.Key is Accessory || item.Key is Consumable || item.Key is Spell)
                    {
                        continue;
                    }
                }

                var instance = itemButtonPrefab.CloneTree();
                instance.Q<VisualElement>("Sprite").style.backgroundImage = new StyleBackground(item.Key.sprite);
                var itemName = instance.Q<Label>("ItemName");
                itemName.text = item.Key.name.GetText();

                var equipmentColorIndicator = GetEquipmentColorIndicator(item.Key);
                if (equipmentColorIndicator == Color.black)
                {
                    instance.Q<VisualElement>("Indicator").style.display = DisplayStyle.None;
                }
                else
                {
                    instance.Q<VisualElement>("Indicator").style.backgroundColor = GetEquipmentColorIndicator(item.Key);
                    instance.Q<VisualElement>("Indicator").style.display = DisplayStyle.Flex;
                }

                var btn = instance.Q<Button>("EquipButton");
                btn.clicked += () =>
                {
                    Soundbank.instance.PlayUIEquip();

                    if (item.Key is Weapon weapon)
                    {
                        playerManager.playerWeaponsManager.EquipWeapon(weapon, slotIndex);
                    }
                    else if (item.Key is Shield shield)
                    {
                        playerManager.playerWeaponsManager.EquipShield(shield, slotIndex);
                    }
                    else if (item.Key is Helmet helmet)
                    {
                        playerManager.equipmentGraphicsHandler.EquipHelmet(helmet);
                    }
                    else if (item.Key is Armor armor)
                    {
                        playerManager.equipmentGraphicsHandler.EquipArmor(armor);
                    }
                    else if (item.Key is Gauntlet gauntlet)
                    {
                        playerManager.equipmentGraphicsHandler.EquipGauntlet(gauntlet);
                    }
                    else if (item.Key is Legwear legwear)
                    {
                        playerManager.equipmentGraphicsHandler.EquipLegwear(legwear);
                    }
                    else if (item.Key is Accessory accessory)
                    {
                        playerManager.equipmentGraphicsHandler.EquipAccessory(accessory, slotIndex);
                    }
                    else if (item.Key is Arrow)
                    {
                        equipmentDatabase.EquipArrow(item.Key as Arrow, slotIndex);
                    }
                    else if (item.Key is Consumable)
                    {
                        equipmentDatabase.EquipConsumable(item.Key as Consumable, slotIndex);
                    }
                    else if (item.Key is Spell)
                    {
                        equipmentDatabase.EquipSpell(item.Key as Spell, slotIndex);
                    }

                    //ReturnToEquipmentSlots();
                };

                instance.RegisterCallback<MouseEnterEvent>(ev =>
                {
                    itemTooltip.gameObject.SetActive(true);
                    itemTooltip.PrepareTooltipForItem(item.Key);
                    itemTooltip.DisplayTooltip(btn);

                    playerStatsAndAttributesUI.DrawStats(item.Key);
                });
                instance.RegisterCallback<MouseOutEvent>(ev =>
                {
                    itemTooltip.gameObject.SetActive(false);

                    playerStatsAndAttributesUI.DrawStats(null);
                });

                if (item.Key is Consumable consumable)
                {
                    var useItemButton = instance.Q<Button>("UseItemButton");
                    useItemButton.Q<Label>().text = "Use";
                    useItemButton.style.display = DisplayStyle.Flex;

                    useItemButton.clicked += () =>
                    {
                        if (playerStatsDatabase.currentHealth <= 0)
                        {
                            return;
                        }

                        playerManager.playerInventory.PrepareItemForConsuming(consumable);
                        menuManager.CloseMenu();
                        cursorManager.HideCursor();
                    };
                }
                else
                {
                    instance.Q<Button>("UseItemButton").style.display = DisplayStyle.None;
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
                value = playerManager.attackStatManager.CompareWeapon(weapon);
                shouldReturn = true;
            }
            else if (item is Helmet helmet)
            {
                value = playerManager.defenseStatManager.CompareHelmet(helmet);
                shouldReturn = true;
            }
            else if (item is Armor armor)
            {
                value = playerManager.defenseStatManager.CompareArmor(armor);
                shouldReturn = true;
            }
            else if (item is Gauntlet gauntlet)
            {
                value = playerManager.defenseStatManager.CompareGauntlet(gauntlet);
                shouldReturn = true;
            }
            else if (item is Legwear legwear)
            {
                value = playerManager.defenseStatManager.CompareLegwear(legwear);
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
