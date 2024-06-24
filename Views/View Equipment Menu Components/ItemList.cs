using System.Collections.Generic;
using System.Linq;
using AF.Equipment;
using AF.Inventory;
using AF.Stats;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
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
        public StarterAssetsInputs inputs;
        public Soundbank soundbank;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;
        public PlayerStatsDatabase playerStatsDatabase;
        public InventoryDatabase inventoryDatabase;

        Button returnButton;

        [HideInInspector] public bool shouldRerender = true;

        VisualElement itemListKeyHints, itemListGamepadHints, equipItemKeyHint, equipItemButtonHint, useItemKeyHint, useItemButtonHint;

        int lastScrollElementIndex = -1;

        public NotificationManager notificationManager;


        private void OnEnable()
        {
            if (shouldRerender)
            {
                shouldRerender = false;

                SetupRefs();
            }

            returnButton.transform.scale = new Vector3(1, 1, 1);
            root.Q<VisualElement>("ItemList").style.display = DisplayStyle.Flex;
            itemListKeyHints.style.display = DisplayStyle.None;
            itemListGamepadHints.style.display = DisplayStyle.None;
        }

        private void OnDisable()
        {
            root.Q<VisualElement>("ItemList").style.display = DisplayStyle.None;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnUseItem()
        {
            /*
            if (isActiveAndEnabled && focusedItem != null && focusedItem is Consumable c)
            {
                playerManager.playerInventory.PrepareItemForConsuming(c);
            }*/
        }

        public void SetupRefs()
        {
            root = uIDocument.rootVisualElement;
            menuLabel = root.Q<Label>("MenuLabel");

            returnButton = root.Q<Button>("ReturnButton");
            UIUtils.SetupButton(returnButton, () =>
            {
                ReturnToEquipmentSlots();
            }, soundbank);

            itemsScrollView = root.Q<ScrollView>(SCROLL_ITEMS_LIST);
            itemListKeyHints = root.Q<VisualElement>("ItemListKeyboardHints");
            itemListGamepadHints = root.Q<VisualElement>("ItemListGamepadHints");
            equipItemKeyHint = itemListKeyHints.Q<VisualElement>("EquipItemKeyHint");
            equipItemButtonHint = itemListGamepadHints.Q<VisualElement>("EquipItemButtonHint");
            useItemKeyHint = itemListKeyHints.Q<VisualElement>("UseItemKeyHint");
            useItemButtonHint = itemListGamepadHints.Q<VisualElement>("UseItemButtonHint");
        }

        public void ReturnToEquipmentSlots()
        {
            itemListKeyHints.style.display = DisplayStyle.None;
            itemListGamepadHints.style.display = DisplayStyle.None;

            equipmentSlots.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }

        void ShowEquipmentHint()
        {
            if (Gamepad.current == null) equipItemKeyHint.style.display = DisplayStyle.Flex;
            else equipItemButtonHint.style.display = DisplayStyle.Flex;
        }

        void ShowUseItemHint()
        {
            if (Gamepad.current == null) useItemKeyHint.style.display = DisplayStyle.Flex;
            else useItemButtonHint.style.display = DisplayStyle.Flex;
        }

        public void DrawUI(EquipmentType equipmentType, int slotIndex)
        {
            menuLabel.style.display = DisplayStyle.None;
            equipItemKeyHint.style.display = DisplayStyle.None;
            equipItemButtonHint.style.display = DisplayStyle.None;
            useItemKeyHint.style.display = DisplayStyle.None;
            useItemButtonHint.style.display = DisplayStyle.None;

            if (Gamepad.current == null)
            {
                itemListKeyHints.style.display = DisplayStyle.Flex;
            }
            else
            {
                itemListGamepadHints.style.display = DisplayStyle.Flex;
            }

            if (equipmentType == EquipmentType.WEAPON)
            {
                ShowEquipmentHint();
                PopulateScrollView<Weapon>(false, slotIndex);
            }
            else if (equipmentType == EquipmentType.SHIELD)
            {
                ShowEquipmentHint();

                PopulateScrollView<Shield>(false, slotIndex);
            }
            else if (equipmentType == EquipmentType.ARROW)
            {
                ShowEquipmentHint();

                PopulateScrollView<Arrow>(false, slotIndex);
            }
            else if (equipmentType == EquipmentType.SPELL)
            {
                ShowEquipmentHint();

                PopulateScrollView<Spell>(false, slotIndex);
            }
            else if (equipmentType == EquipmentType.HELMET)
            {
                ShowEquipmentHint();

                PopulateScrollView<Helmet>(false, slotIndex);
            }
            else if (equipmentType == EquipmentType.ARMOR)
            {
                ShowEquipmentHint();

                PopulateScrollView<Armor>(false, slotIndex);
            }
            else if (equipmentType == EquipmentType.GAUNTLET)
            {
                ShowEquipmentHint();

                PopulateScrollView<Gauntlet>(false, slotIndex);
            }
            else if (equipmentType == EquipmentType.BOOTS)
            {
                ShowEquipmentHint();

                PopulateScrollView<Legwear>(false, slotIndex);
            }
            else if (equipmentType == EquipmentType.ACCESSORIES)
            {
                ShowEquipmentHint();

                PopulateScrollView<Accessory>(false, slotIndex);
            }
            else if (equipmentType == EquipmentType.CONSUMABLES)
            {
                ShowEquipmentHint();

                ShowUseItemHint();

                PopulateScrollView<Consumable>(false, slotIndex);
            }
            else if (equipmentType == EquipmentType.OTHER_ITEMS)
            {
                ShowUseItemHint();

                PopulateScrollView<Item>(true, slotIndex);
            }

            // Delay the focus until the next frame, required as a hack for now
            Invoke(nameof(GiveFocus), 0f);
        }

        bool IsItemEquipped(Item item, int slotIndex)
        {
            if (item is Weapon)
            {
                return equipmentDatabase.weapons[slotIndex] == item;
            }
            else if (item is Shield)
            {
                return equipmentDatabase.shields[slotIndex] == item;
            }
            else if (item is Arrow)
            {
                return equipmentDatabase.arrows[slotIndex] == item;
            }
            else if (item is Spell)
            {
                return equipmentDatabase.spells[slotIndex] == item;
            }
            else if (item is Accessory)
            {
                return equipmentDatabase.accessories[slotIndex] == item;
            }
            else if (item is Consumable)
            {
                return equipmentDatabase.consumables[slotIndex] == item;
            }
            else if (item is Helmet)
            {
                return equipmentDatabase.helmet == item;
            }
            else if (item is Armor)
            {
                return equipmentDatabase.armor == item;
            }
            else if (item is Gauntlet)
            {
                return equipmentDatabase.gauntlet == item;
            }
            else if (item is Legwear)
            {
                return equipmentDatabase.legwear == item;
            }

            return false;
        }

        public bool IsKeyItem(Item item)
        {
            return !(item is Weapon || item is Shield || item is Helmet || item is Armor || item is Gauntlet || item is Legwear
                        || item is Accessory || item is Consumable || item is Spell || item is Arrow);
        }

        public bool ShouldShowItem<T>(KeyValuePair<Item, ItemAmount> item, int slotIndexToEquip, bool showOnlyKeyItems)
        {
            if (item.Key is not T)
            {
                return false;
            }

            if (showOnlyKeyItems && !IsKeyItem(item.Key))
            {
                return false;
            }

            int equippedSlotIndex = -1;

            if (item.Key is Weapon weapon)
            {
                equippedSlotIndex = equipmentDatabase.GetEquippedWeaponSlot(weapon);

            }
            else if (item.Key is Shield shield)
            {
                equippedSlotIndex = equipmentDatabase.GetEquippedShieldSlot(shield);
            }
            else if (item.Key is Arrow arrow)
            {
                equippedSlotIndex = equipmentDatabase.GetEquippedArrowsSlot(arrow);
            }
            else if (item.Key is Spell spell)
            {
                equippedSlotIndex = equipmentDatabase.GetEquippedSpellSlot(spell);
            }
            else if (item.Key is Accessory accessory)
            {
                equippedSlotIndex = equipmentDatabase.GetEquippedAccessoriesSlot(accessory);
            }
            else if (item.Key is Consumable consumable)
            {
                equippedSlotIndex = equipmentDatabase.GetEquippedConsumablesSlot(consumable);
            }

            if (equippedSlotIndex >= 0 && equippedSlotIndex != slotIndexToEquip)
            {
                return false;
            }

            return true;
        }

        void PopulateScrollView<T>(bool showOnlyKeyItems, int slotIndex) where T : Item
        {
            this.itemsScrollView.Clear();

            Dictionary<Item, ItemAmount> filteredItems = inventoryDatabase.ownedItems.Where(item =>
            {
                return ShouldShowItem<T>(item, slotIndex, showOnlyKeyItems);
            }).ToDictionary(item => item.Key, item => item.Value);


            for (int i = 0; i < filteredItems.Count; i++)
            {
                var item = filteredItems.ElementAt(i);

                bool isEquipped = IsItemEquipped(item.Key, slotIndex);

                var instance = itemButtonPrefab.CloneTree();
                instance.Q<VisualElement>("Sprite").style.backgroundImage = new StyleBackground(item.Key.sprite);
                var itemName = instance.Q<Label>("ItemName");
                itemName.text = item.Key.GetName();

                if (item.Key is Consumable || item.Key is Arrow || showOnlyKeyItems)
                {
                    itemName.text += $" ({item.Value.amount})";
                }

                if (isEquipped)
                {
                    itemName.text += " " + LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "(Equipped)");
                }

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

                int index = i;
                btn.clicked += () =>
                {
                    lastScrollElementIndex = index;

                    soundbank.PlaySound(soundbank.uiEquip);

                    bool ignoreRerender = false;

                    if (item.Key is Weapon weapon)
                    {
                        if (!isEquipped)
                        {
                            if (!weapon.AreRequirementsMet(playerManager.statsBonusController))
                            {
                                notificationManager.ShowNotification(LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Can't equip weapon. Requirements are not met."), notificationManager.systemError);
                                ignoreRerender = true;
                            }
                            else
                            {
                                playerManager.playerWeaponsManager.EquipWeapon(weapon, slotIndex);
                            }
                        }
                        else
                        {
                            playerManager.playerWeaponsManager.UnequipWeapon(slotIndex);
                        }
                    }
                    else if (item.Key is Shield shield)
                    {
                        if (!isEquipped)
                        {
                            playerManager.playerWeaponsManager.EquipShield(shield, slotIndex);
                        }
                        else
                        {
                            playerManager.playerWeaponsManager.UnequipShield(slotIndex);
                        }
                    }
                    else if (item.Key is Helmet helmet)
                    {
                        if (!isEquipped)
                        {
                            playerManager.equipmentGraphicsHandler.EquipHelmet(helmet);
                        }
                        else
                        {
                            playerManager.equipmentGraphicsHandler.UnequipHelmet();
                        }
                    }
                    else if (item.Key is Armor armor)
                    {
                        if (!isEquipped)
                        {
                            playerManager.equipmentGraphicsHandler.EquipArmor(armor);
                        }
                        else
                        {
                            playerManager.equipmentGraphicsHandler.UnequipArmor();
                        }
                    }
                    else if (item.Key is Gauntlet gauntlet)
                    {
                        if (!isEquipped)
                        {
                            playerManager.equipmentGraphicsHandler.EquipGauntlet(gauntlet);
                        }
                        else
                        {
                            playerManager.equipmentGraphicsHandler.UnequipGauntlet();
                        }
                    }
                    else if (item.Key is Legwear legwear)
                    {
                        if (!isEquipped)
                        {
                            playerManager.equipmentGraphicsHandler.EquipLegwear(legwear);
                        }
                        else
                        {
                            playerManager.equipmentGraphicsHandler.UnequipLegwear();
                        }
                    }
                    else if (item.Key is Accessory accessory)
                    {
                        if (!isEquipped)
                        {
                            playerManager.equipmentGraphicsHandler.EquipAccessory(accessory, slotIndex);
                        }
                        else
                        {
                            playerManager.equipmentGraphicsHandler.UnequipAccessory(slotIndex);
                        }
                    }
                    else if (item.Key is Arrow)
                    {
                        if (!isEquipped)
                        {
                            equipmentDatabase.EquipArrow(item.Key as Arrow, slotIndex);
                        }
                        else
                        {
                            equipmentDatabase.UnequipArrow(slotIndex);
                        }
                    }
                    else if (item.Key is Consumable)
                    {
                        if (!isEquipped)
                        {
                            equipmentDatabase.EquipConsumable(item.Key as Consumable, slotIndex);
                        }
                        else
                        {
                            equipmentDatabase.UnequipConsumable(slotIndex);
                        }
                    }
                    else if (item.Key is Spell spell)
                    {
                        if (!isEquipped)
                        {
                            if (!spell.AreRequirementsMet(playerManager.statsBonusController))
                            {
                                notificationManager.ShowNotification(LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Can not equip spell. Requirements not met!"), notificationManager.systemError);
                                ignoreRerender = true;
                            }
                            else
                            {
                                equipmentDatabase.EquipSpell(item.Key as Spell, slotIndex);
                            }
                        }
                        else
                        {
                            equipmentDatabase.UnequipSpell(slotIndex);
                        }
                    }

                    if (!ignoreRerender)
                    {
                        ReturnToEquipmentSlots();
                    }

                    //PopulateScrollView<T>(showOnlyKeyItems, slotIndex);
                };

                void ShowTooltipAndStats(Item item)
                {
                    itemTooltip.gameObject.SetActive(true);
                    itemTooltip.PrepareTooltipForItem(item);
                    itemTooltip.DisplayTooltip(btn);

                    playerStatsAndAttributesUI.DrawStats(item);
                }

                void HideTooltipAndClearStats()
                {
                    itemTooltip.gameObject.SetActive(false);
                    playerStatsAndAttributesUI.DrawStats(null);
                }

                instance.RegisterCallback<MouseEnterEvent>(ev =>
                {
                    itemsScrollView.ScrollTo(instance);
                    ShowTooltipAndStats(item.Key);
                });
                instance.RegisterCallback<FocusInEvent>(ev =>
                {
                    itemsScrollView.ScrollTo(instance);

                    ShowTooltipAndStats(item.Key);
                });
                instance.RegisterCallback<MouseOutEvent>(ev =>
                {
                    HideTooltipAndClearStats();
                });
                instance.RegisterCallback<FocusOutEvent>(ev =>
                {
                    HideTooltipAndClearStats();
                });

                instance.RegisterCallback<KeyDownEvent>(ev =>
                {
                    if (inputs.jump && item.Key is Consumable)
                    {
                        inputs.jump = false;

                        if (playerStatsDatabase.currentHealth <= 0)
                        {
                            return;
                        }

                        playerManager.playerInventory.PrepareItemForConsuming(item.Key as Consumable);
                        menuManager.CloseMenu();
                    }
                });

                this.itemsScrollView.Add(instance);
            }



            Invoke(nameof(GiveFocus), 0f);
        }

        void GiveFocus()
        {
            if (lastScrollElementIndex == -1)
            {
                returnButton.Focus();
            }
            else
            {
                UIUtils.ScrollToLastPosition(
                    lastScrollElementIndex,
                    itemsScrollView,
                    () =>
                    {
                        lastScrollElementIndex = -1;
                    }
                );
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
