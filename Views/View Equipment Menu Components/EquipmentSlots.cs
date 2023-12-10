using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF.UI.EquipmentMenu
{
    public class EquipmentSlots : MonoBehaviour
    {
        [Header("Components")]
        public Soundbank soundbank;

        [Header("UI Documents")]
        public UIDocument uIDocument;
        public VisualElement root;

        public ItemList itemList;
        public ItemTooltip itemTooltip;

        Button weaponButtonSlot1;
        Button weaponButtonSlot2;
        Button weaponButtonSlot3;

        Button secondaryWeaponButtonSlot1;
        Button secondaryWeaponButtonSlot2;
        Button secondaryWeaponButtonSlot3;

        Button arrowsButtonSlot1;
        Button arrowsButtonSlot2;

        Button spellsButtonSlot1;
        Button spellsButtonSlot2;
        Button spellsButtonSlot3;
        Button spellsButtonSlot4;
        Button spellsButtonSlot5;

        Button helmetButtonSlot;
        Button armorButtonSlot;
        Button gauntletsButtonSlot;
        Button bootsButtonSlot;

        Button accessoryButtonSlot1;
        Button accessoryButtonSlot2;

        Button consumableButtonSlot1;
        Button consumableButtonSlot2;
        Button consumableButtonSlot3;
        Button consumableButtonSlot4;
        Button consumableButtonSlot5;

        Button otherItemsButton;

        Label menuLabel;

        [Header("Sprites")]
        public Texture2D txt_UnequipedWeapon, txt_UnequipedShield, txt_UnequipedArrow,
            txt_UnequipedSpell, txt_UnequippedHelmet, txt_UnequippedArmor, txt_UnequippedLegwear, txt_UnequippedGauntlets,
            txt_UnequippedAccessory, txt_UnequippedConsumable, txt_OtherItems;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;

        [HideInInspector] public bool shouldRerender = true;

        VisualElement keyboardHints;

        Button activeButton;

        private void OnEnable()
        {
            if (shouldRerender)
            {
                shouldRerender = false;

                SetupRefs();
            }

            DrawUI();
            root.Q<VisualElement>("EquipmentSlots").style.display = DisplayStyle.Flex;
        }

        private void OnDisable()
        {
            keyboardHints.style.display = DisplayStyle.None;

            root.Q<VisualElement>("EquipmentSlots").style.display = DisplayStyle.None;
        }

        public void SetupRefs()
        {
            activeButton = null;

            root = uIDocument.rootVisualElement;

            weaponButtonSlot1 = root.Q<Button>("WeaponButton_Slot1");
            weaponButtonSlot2 = root.Q<Button>("WeaponButton_Slot2");
            weaponButtonSlot3 = root.Q<Button>("WeaponButton_Slot3");

            secondaryWeaponButtonSlot1 = root.Q<Button>("SecondaryWeaponButton_Slot1");
            secondaryWeaponButtonSlot2 = root.Q<Button>("SecondaryWeaponButton_Slot2");
            secondaryWeaponButtonSlot3 = root.Q<Button>("SecondaryWeaponButton_Slot3");

            arrowsButtonSlot1 = root.Q<Button>("ArrowsButton_Slot1");
            arrowsButtonSlot2 = root.Q<Button>("ArrowsButton_Slot2");

            spellsButtonSlot1 = root.Q<Button>("SpellsButton_Slot1");
            spellsButtonSlot2 = root.Q<Button>("SpellsButton_Slot2");
            spellsButtonSlot3 = root.Q<Button>("SpellsButton_Slot3");
            spellsButtonSlot4 = root.Q<Button>("SpellsButton_Slot4");
            spellsButtonSlot5 = root.Q<Button>("SpellsButton_Slot5");

            helmetButtonSlot = root.Q<Button>("HelmetButton");
            armorButtonSlot = root.Q<Button>("ArmorButton");
            gauntletsButtonSlot = root.Q<Button>("GauntletsButton");
            bootsButtonSlot = root.Q<Button>("BootsButton");

            accessoryButtonSlot1 = root.Q<Button>("AccessoriesButton_Slot1");
            accessoryButtonSlot2 = root.Q<Button>("AccessoriesButton_Slot2");

            consumableButtonSlot1 = root.Q<Button>("ConsumablesButton_Slot1");
            consumableButtonSlot2 = root.Q<Button>("ConsumablesButton_Slot2");
            consumableButtonSlot3 = root.Q<Button>("ConsumablesButton_Slot3");
            consumableButtonSlot4 = root.Q<Button>("ConsumablesButton_Slot4");
            consumableButtonSlot5 = root.Q<Button>("ConsumablesButton_Slot5");

            otherItemsButton = root.Q<Button>("OtherItemsButton");

            keyboardHints = root.Q<VisualElement>("EquipmentSlotsKeyboardHints");
            keyboardHints.style.display = DisplayStyle.None;

            AssignWeaponButtonCallbacks();
            AssignShieldButtonCallbacks();
            AssignArrowButtonCallbacks();
            AssignSpellButtonCallbacks();
            AssignArmorButtonCallbacks();
            AssignAccessoryButtonCallbacks();
            AssignConsumableButtonCallbacks();
            AssignOtherItemsButtonCallbacks();
        }

        void AssignWeaponButtonCallbacks()
        {
            Dictionary<Button, Func<Weapon>> buttonDictionary = new()
            {
                { weaponButtonSlot1, () => equipmentDatabase.weapons[0] },
                { weaponButtonSlot2, () => equipmentDatabase.weapons[1] },
                { weaponButtonSlot3, () => equipmentDatabase.weapons[2] },
            };

            int slotIndex = 0;
            foreach (var entry in buttonDictionary)
            {
                int localSlotIndex = slotIndex;  // Create a local variable to capture the correct value

                UIUtils.SetupButton(entry.Key,
                    () =>
                    {
                        activeButton = entry.Key;

                        SetupEquipmentButton(ItemList.EquipmentType.WEAPON, localSlotIndex, "Weapons");
                    },
                    () =>
                    {
                        OnSlotFocus("Weapons", entry.Value());
                    },
                    () =>
                    {
                        OnSlotFocusOut();
                    },
                    false,
                    soundbank);

                slotIndex++;
            }
        }

        void AssignShieldButtonCallbacks()
        {
            Dictionary<Button, Func<Shield>> buttonDictionary = new()
            {
                { secondaryWeaponButtonSlot1, () => equipmentDatabase.shields[0] },
                { secondaryWeaponButtonSlot2, () => equipmentDatabase.shields[1] },
                { secondaryWeaponButtonSlot3, () => equipmentDatabase.shields[2] },
            };

            int slotIndex = 0;
            foreach (var entry in buttonDictionary)
            {
                int localSlotIndex = slotIndex;  // Create a local variable to capture the correct value

                UIUtils.SetupButton(entry.Key,
                    () =>
                    {
                        activeButton = entry.Key;

                        SetupEquipmentButton(ItemList.EquipmentType.SHIELD, localSlotIndex, "Shields");
                    },
                    () =>
                    {
                        OnSlotFocus("Shields", entry.Value());
                    },
                    () =>
                    {
                        OnSlotFocusOut();
                    },
                    false,
                    soundbank);

                slotIndex++;
            }
        }

        void AssignArrowButtonCallbacks()
        {
            Dictionary<Button, Func<Arrow>> buttonDictionary = new()
            {
                { arrowsButtonSlot1, () => equipmentDatabase.arrows[0] },
                { arrowsButtonSlot2, () => equipmentDatabase.arrows[1] },
            };

            int slotIndex = 0;
            foreach (var entry in buttonDictionary)
            {
                int localSlotIndex = slotIndex;  // Create a local variable to capture the correct value

                UIUtils.SetupButton(entry.Key,
                    () =>
                    {
                        activeButton = entry.Key;

                        SetupEquipmentButton(ItemList.EquipmentType.ARROW, localSlotIndex, "Arrow");
                    },
                    () =>
                    {
                        OnSlotFocus("Arrows", entry.Value());
                    },
                    () =>
                    {
                        OnSlotFocusOut();
                    },
                    false,
                    soundbank);

                slotIndex++;
            }
        }

        void AssignSpellButtonCallbacks()
        {
            Dictionary<Button, Func<Spell>> buttonDictionary = new()
            {
                { spellsButtonSlot1, () => equipmentDatabase.spells[0] },
                { spellsButtonSlot2, () => equipmentDatabase.spells[1] },
                { spellsButtonSlot3, () => equipmentDatabase.spells[2] },
                { spellsButtonSlot4, () => equipmentDatabase.spells[3] },
                { spellsButtonSlot5, () => equipmentDatabase.spells[4] }
            };

            int slotIndex = 0;
            foreach (var entry in buttonDictionary)
            {
                int localSlotIndex = slotIndex;  // Create a local variable to capture the correct value

                UIUtils.SetupButton(entry.Key,
                    () =>
                    {
                        activeButton = entry.Key;
                        SetupEquipmentButton(ItemList.EquipmentType.SPELL, localSlotIndex, "Spell");
                    },
                    () =>
                    {
                        OnSlotFocus("Spells", entry.Value());
                    },
                    () =>
                    {
                        OnSlotFocusOut();
                    },
                    false,
                    soundbank);

                slotIndex++;
            }
        }

        void AssignArmorButtonCallbacks()
        {
            AssignHelmetButtonCallback();
            AssignArmorButtonCallback();
            AssignGauntletsButtonCallback();
            AssignLegwearButtonCallback();
        }


        void AssignHelmetButtonCallback()
        {
            Item Get() { return equipmentDatabase.helmet; }

            UIUtils.SetupButton(helmetButtonSlot,
            () =>
            {
                activeButton = helmetButtonSlot;

                SetupEquipmentButton(ItemList.EquipmentType.HELMET, 0, "Helmet");
            },
            () => { OnSlotFocus("Helmet", Get()); },
            OnSlotFocusOut,
            false,
            soundbank);
        }
        void AssignArmorButtonCallback()
        {
            Item Get() { return equipmentDatabase.armor; }

            UIUtils.SetupButton(armorButtonSlot,
            () =>
            {
                activeButton = armorButtonSlot;

                SetupEquipmentButton(ItemList.EquipmentType.ARMOR, 0, "Armor");
            },
            () => { OnSlotFocus("Armor", Get()); },
            OnSlotFocusOut,
            false,
            soundbank);
        }
        void AssignGauntletsButtonCallback()
        {
            Item Get() { return equipmentDatabase.gauntlet; }

            UIUtils.SetupButton(gauntletsButtonSlot,
            () =>
            {
                activeButton = gauntletsButtonSlot;

                SetupEquipmentButton(ItemList.EquipmentType.GAUNTLET, 0, "Gauntlets");
            },
            () => { OnSlotFocus("Gauntlets", Get()); },
            OnSlotFocusOut,
            false,
            soundbank);
        }
        void AssignLegwearButtonCallback()
        {
            Item Get() { return equipmentDatabase.legwear; }

            UIUtils.SetupButton(bootsButtonSlot,
            () =>
            {
                activeButton = bootsButtonSlot;

                SetupEquipmentButton(ItemList.EquipmentType.BOOTS, 0, "Boots");
            },
            () => { OnSlotFocus("Boots", Get()); },
            OnSlotFocusOut,
            false,
            soundbank);
        }

        void AssignAccessoryButtonCallbacks()
        {
            Dictionary<Button, Func<Accessory>> buttonDictionary = new()
            {
                { accessoryButtonSlot1, () => equipmentDatabase.accessories[0] },
                { accessoryButtonSlot2, () => equipmentDatabase.accessories[1] }
            };

            int slotIndex = 0;
            foreach (var entry in buttonDictionary)
            {
                int localSlotIndex = slotIndex;  // Create a local variable to capture the correct value

                UIUtils.SetupButton(entry.Key,
                    () =>
                    {
                        activeButton = entry.Key;

                        SetupEquipmentButton(ItemList.EquipmentType.ACCESSORIES, localSlotIndex, "Accessories");
                    },
                    () =>
                    {
                        OnSlotFocus("Accessories", entry.Value());
                    },
                    () =>
                    {
                        OnSlotFocusOut();
                    },
                    false,
                    soundbank);

                slotIndex++;
            }
        }

        void AssignConsumableButtonCallbacks()
        {
            Dictionary<Button, Func<Consumable>> buttonDictionary = new()
            {
                { consumableButtonSlot1, () => equipmentDatabase.consumables[0] },
                { consumableButtonSlot2, () => equipmentDatabase.consumables[1] },
                { consumableButtonSlot3, () => equipmentDatabase.consumables[2] },
                { consumableButtonSlot4, () => equipmentDatabase.consumables[3] },
                { consumableButtonSlot5, () => equipmentDatabase.consumables[4] }
            };

            int slotIndex = 0;
            foreach (var entry in buttonDictionary)
            {
                int localSlotIndex = slotIndex;  // Create a local variable to capture the correct value

                UIUtils.SetupButton(entry.Key,
                    () =>
                    {
                        activeButton = entry.Key;

                        SetupEquipmentButton(ItemList.EquipmentType.CONSUMABLES, localSlotIndex, "Consumables");
                    },
                    () =>
                    {
                        OnSlotFocus("Consumables", entry.Value());
                    },
                    () =>
                    {
                        OnSlotFocusOut();
                    },
                    false,
                    soundbank);

                slotIndex++;
            }
        }

        void AssignOtherItemsButtonCallbacks()
        {
            UIUtils.SetupButton(otherItemsButton,
            () =>
            {
                activeButton = otherItemsButton;
                SetupEquipmentButton(ItemList.EquipmentType.OTHER_ITEMS, 0, "All Items");
            },
            () =>
            {
                OnSlotFocus("All Items", null);
            },
            () =>
            {
                OnSlotFocusOut();
            },
            false,
            soundbank);
        }

        void DrawUI()
        {
            keyboardHints.style.display = DisplayStyle.Flex;

            DrawSlotSprites();

            menuLabel = root.Q<Label>("MenuLabel");
            menuLabel.text = "Equipment";
            menuLabel.style.display = DisplayStyle.Flex;

            // Delay the focus until the next frame, required as a hack for now
            Invoke(nameof(GiveFocus), 0f);
        }

        void GiveFocus()
        {
            if (activeButton != null)
            {
                activeButton.Focus();
            }
            else
            {
                weaponButtonSlot1.Focus();
            }
        }

        void OnSlotFocus(string activeSlotMenuLabel, Item item)
        {
            menuLabel.text = activeSlotMenuLabel;

            if (item != null)
            {
                menuLabel.text = item.name.GetEnglishText();

                itemTooltip.gameObject.SetActive(true);
                itemTooltip.PrepareTooltipForItem(item);
            }
        }

        void OnSlotFocusOut()
        {
            menuLabel.text = "Equipment";

            itemTooltip.gameObject.SetActive(false);
        }

        void SetupEquipmentButton(ItemList.EquipmentType equipmentType, int slotIndex, string label)
        {
            itemList.gameObject.SetActive(true);
            itemList.DrawUI(equipmentType, slotIndex);

            root.Q<Label>("MenuLabel").text = label;

            this.gameObject.SetActive(false);
        }
        void DrawSlotSprites()
        {
            SetBackgroundImage(weaponButtonSlot1, equipmentDatabase.weapons, 0, txt_UnequipedWeapon);
            SetBackgroundImage(weaponButtonSlot2, equipmentDatabase.weapons, 1, txt_UnequipedWeapon);
            SetBackgroundImage(weaponButtonSlot3, equipmentDatabase.weapons, 2, txt_UnequipedWeapon);

            SetBackgroundImage(secondaryWeaponButtonSlot1, equipmentDatabase.shields, 0, txt_UnequipedShield);
            SetBackgroundImage(secondaryWeaponButtonSlot2, equipmentDatabase.shields, 1, txt_UnequipedShield);
            SetBackgroundImage(secondaryWeaponButtonSlot3, equipmentDatabase.shields, 2, txt_UnequipedShield);

            SetBackgroundImage(arrowsButtonSlot1, equipmentDatabase.arrows, 0, txt_UnequipedArrow);
            SetBackgroundImage(arrowsButtonSlot2, equipmentDatabase.arrows, 1, txt_UnequipedArrow);

            SetBackgroundImage(spellsButtonSlot1, equipmentDatabase.spells, 0, txt_UnequipedSpell);
            SetBackgroundImage(spellsButtonSlot2, equipmentDatabase.spells, 1, txt_UnequipedSpell);
            SetBackgroundImage(spellsButtonSlot3, equipmentDatabase.spells, 2, txt_UnequipedSpell);
            SetBackgroundImage(spellsButtonSlot4, equipmentDatabase.spells, 3, txt_UnequipedSpell);
            SetBackgroundImage(spellsButtonSlot5, equipmentDatabase.spells, 4, txt_UnequipedSpell);

            SetBackgroundImage(helmetButtonSlot, new Item[] { equipmentDatabase.helmet }, 0, txt_UnequippedHelmet);
            SetBackgroundImage(armorButtonSlot, new Item[] { equipmentDatabase.armor }, 0, txt_UnequippedArmor);
            SetBackgroundImage(bootsButtonSlot, new Item[] { equipmentDatabase.legwear }, 0, txt_UnequippedLegwear);
            SetBackgroundImage(gauntletsButtonSlot, new Item[] { equipmentDatabase.gauntlet }, 0, txt_UnequippedGauntlets);

            SetBackgroundImage(accessoryButtonSlot1, equipmentDatabase.accessories, 0, txt_UnequippedAccessory);
            SetBackgroundImage(accessoryButtonSlot2, equipmentDatabase.accessories, 1, txt_UnequippedAccessory);

            SetBackgroundImage(consumableButtonSlot1, equipmentDatabase.consumables, 0, txt_UnequippedConsumable);
            SetBackgroundImage(consumableButtonSlot2, equipmentDatabase.consumables, 1, txt_UnequippedConsumable);
            SetBackgroundImage(consumableButtonSlot3, equipmentDatabase.consumables, 2, txt_UnequippedConsumable);
            SetBackgroundImage(consumableButtonSlot4, equipmentDatabase.consumables, 3, txt_UnequippedConsumable);
            SetBackgroundImage(consumableButtonSlot5, equipmentDatabase.consumables, 4, txt_UnequippedConsumable);
        }

        void SetBackgroundImage(VisualElement button, Item[] items, int index, Texture2D unequippedTexture)
        {
            if (index < items.Length)
            {
                if (items[index] != null)
                {
                    button.style.backgroundImage = new StyleBackground(items[index].sprite);
                }
                else
                {
                    button.style.backgroundImage = new StyleBackground(unequippedTexture);
                }
            }
        }
    }
}
