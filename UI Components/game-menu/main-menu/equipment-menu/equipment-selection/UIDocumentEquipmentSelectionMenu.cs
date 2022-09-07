using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UnityEngine.Events;

namespace AF
{

    public class UIDocumentEquipmentSelectionMenu : UIDocumentScrollerBase
    {
        EquipmentGraphicsHandler equipmentGraphicsHandler;

        Label menuTitle;
        Label currentItemName;
        Label currentItemStatValue;
        Label itemToEquipName;
        Label itemToEquipStatValue;
        Label increasedValueLabel;
        Label decreasedValueLabel;
        ScrollView equipmentListScrollView;

        private void Awake()
        {
            equipmentGraphicsHandler = FindObjectOfType<EquipmentGraphicsHandler>(true);
        }

        protected override void Start()
        {
            base.Start();

            // EQUIPMENT SELECTION MENU SCREEN
            this.menuTitle = this.root.Q<Label>("SelectionLabel");
            this.currentItemName = this.root.Q<VisualElement>("EquippedItem").Q<Label>("ItemName");
            this.currentItemStatValue = this.root.Q<VisualElement>("EquippedItem").Q<Label>("CurrentValue");
            this.itemToEquipName = this.root.Q<VisualElement>("WeaponToEquip").Q<Label>("ItemName");
            this.itemToEquipStatValue = this.root.Q<VisualElement>("WeaponToEquip").Q<Label>("CurrentValue");
            this.increasedValueLabel = this.root.Q<VisualElement>("ValueDifferences").Q<Label>("IncreasedValue");
            this.decreasedValueLabel = this.root.Q<VisualElement>("ValueDifferences").Q<Label>("DecreasedValue");
            this.equipmentListScrollView = this.root.Q<ScrollView>("ScrollView");

            this.Disable();
        }


        void UpdateEquipmentSelection()
        {
            // Clean up scroll view contents
            this.equipmentListScrollView.Clear();

            var equipmentSelectionMode = FindObjectOfType<UIDocumentEquipmentMenu>().equipmentSelectionMode;

            if (equipmentSelectionMode == EquipmentSelectionMode.WEAPON)
            {
                SetupWeaponSelectionScreen();
            }
            else if (equipmentSelectionMode == EquipmentSelectionMode.SHIELD)
            {
                SetupShieldSelection();
            }
            else if (equipmentSelectionMode == EquipmentSelectionMode.HEAD)
            {
                SetupArmorSelection(ArmorSlot.Head, false);
            }
            else if (equipmentSelectionMode == EquipmentSelectionMode.CHEST)
            {
                SetupArmorSelection(ArmorSlot.Chest, false);
            }
            else if (equipmentSelectionMode == EquipmentSelectionMode.ARMS)
            {
                SetupArmorSelection(ArmorSlot.Arms, false);
            }
            else if (equipmentSelectionMode == EquipmentSelectionMode.LEGS)
            {
                SetupArmorSelection(ArmorSlot.Legs, false);
            }
            else if (equipmentSelectionMode == EquipmentSelectionMode.ACCESSORY_1)
            {
                SetupAccessory(0);
            }
            else if (equipmentSelectionMode == EquipmentSelectionMode.ACCESSORY_2)
            {
                SetupAccessory(1);
            }

        }

        void SetupWeaponSelectionScreen()
        {
            this.menuTitle.text = "Weapon equipment";

            var equippedWeapon = PlayerInventoryManager.instance.currentWeapon;
            this.currentItemName.text = "Equipped: " + (equippedWeapon != null ? equippedWeapon.name : "None");
            this.currentItemStatValue.text = "Attack Power: " + (equippedWeapon != null ? equippedWeapon.physicalAttack.ToString() : "0");
            
            HideIncreaseAndDecreaseLabels();

            var goBackBtn = CreateButton(
                "Go Back",
                () => { ShowCurrentEquipmentScreen(); },
                () => {
                    HideItemToEquipInfo();
                });
            goBackBtn.Focus();

            CreateButton(
                "None (Unarmed)",
                () => {
                    equipmentGraphicsHandler.UnequipWeapon();
                    ShowCurrentEquipmentScreen();
                },
                () => {
                    var unarmedWeapon = PlayerInventoryManager.instance.defaultUnarmedWeapon;
                    this.itemToEquipName.text = "To equip: " + unarmedWeapon.name;
                    this.itemToEquipStatValue.text = "Attack Power: " + unarmedWeapon.physicalAttack;
                });

            foreach (Weapon weapon in PlayerInventoryManager.instance.GetWeapons())
            {
                if (weapon.name != "Unarmed")
                {
                    CreateButton(
                        weapon.name,
                        // Click Callback
                        () =>
                        {
                            equipmentGraphicsHandler.Equip(weapon);
                            ShowCurrentEquipmentScreen();
                        },
                        // Focus Callback
                        () =>
                        {
                            this.itemToEquipName.text = "Weapon to equip: " + weapon.name;
                            this.itemToEquipStatValue.text = "Attack Power: " + weapon.physicalAttack.ToString();

                            float difference = weapon.physicalAttack - PlayerInventoryManager.instance.currentWeapon.physicalAttack;
                            UpdateIncreaseAndDecreasedLabelsBasedOnDifference(difference);
                        });
                }
            }
        }

        void SetupShieldSelection()
        {
            this.menuTitle.text = "Shield equipment";

            var equippedShield = PlayerInventoryManager.instance.currentShield;
            this.currentItemName.text = "Equipped: " + (equippedShield != null ? equippedShield.name : "None");
            this.currentItemStatValue.text = "Block stamina cost: " + (
                equippedShield != null ? equippedShield.blockStaminaCost.ToString() : "0");
            
            HideIncreaseAndDecreaseLabels();


            var goBackBtn = CreateButton(
                "Go Back",
                () => { ShowCurrentEquipmentScreen(); },
                () => {
                    HideItemToEquipInfo();
                });
            goBackBtn.Focus();

            CreateButton(
                "None",
                () => {
                    equipmentGraphicsHandler.UnequipShield();
                    ShowCurrentEquipmentScreen();
                },
                () => {
                    HideItemToEquipInfo();
                });


            foreach (Shield shield in PlayerInventoryManager.instance.GetShields())
            {
                CreateButton(
                    shield.name,
                    // Click Callback
                    () =>
                    {
                        equipmentGraphicsHandler.Equip(shield);
                        ShowCurrentEquipmentScreen();
                    },
                    // Focus Callback
                    () =>
                    {
                        this.itemToEquipName.text = "To equip: " + shield.name;
                        this.itemToEquipStatValue.text = "Block stamina cost: " + shield.blockStaminaCost.ToString();
                    });
            }
        }

        void SetupArmorSelection(ArmorSlot armorSlot, bool focusOnCurrentEquippedButton)
        {
            // Clean up scroll view contents
            this.equipmentListScrollView.Clear();

            float currentDefenseAbsorption = PlayerStatsManager.instance.GetDefenseAbsorption();

            string menuLabel = "";
            string currentEquipmentName = "None";
            float currentEquipmentPhysicalDefense = 0;

            if (armorSlot == ArmorSlot.Head)
            {
                menuLabel = "Select head equipment";

                var equippedHelmet = PlayerInventoryManager.instance.currentHelmet;
                if (equippedHelmet != null)
                {
                    currentEquipmentName = equippedHelmet.name;
                    currentEquipmentPhysicalDefense = equippedHelmet.physicalDefense;
                }
            }
            else if (armorSlot == ArmorSlot.Chest)
            {
                menuLabel = "Select chest equipment";

                var equippedChest = PlayerInventoryManager.instance.currentChest;
                if (equippedChest != null)
                {
                    currentEquipmentName = equippedChest.name;
                    currentEquipmentPhysicalDefense = equippedChest.physicalDefense;
                }
            }
            else if (armorSlot == ArmorSlot.Arms)
            {
                menuLabel = "Select arms equipment";
                
                var equippedGauntlets = PlayerInventoryManager.instance.currentGauntlets;
                if (equippedGauntlets != null)
                {
                    currentEquipmentName = equippedGauntlets.name;
                    currentEquipmentPhysicalDefense = equippedGauntlets.physicalDefense;
                }
            }
            else if (armorSlot == ArmorSlot.Legs)
            {
                menuLabel = "Select legs equipment";

                var equippedLegwear = PlayerInventoryManager.instance.currentLegwear;
                if (equippedLegwear != null)
                {
                    currentEquipmentName = equippedLegwear.name;
                    currentEquipmentPhysicalDefense = equippedLegwear.physicalDefense;
                }
            }

            this.menuTitle.text = menuLabel;
            this.currentItemName.text = "Equipped: " + currentEquipmentName;
            this.currentItemStatValue.text = "Total Physical Defense: " + currentDefenseAbsorption + " (" + currentEquipmentName + ": +" + currentEquipmentPhysicalDefense + ")";

            HideIncreaseAndDecreaseLabels();

            // GO BACK BUTTON LOGIC
            var goBackBtn = CreateButton(
                "Go Back",
                () => { ShowCurrentEquipmentScreen(); },
                () => {
                    HideItemToEquipInfo();
                });

            if (focusOnCurrentEquippedButton == false)
            {
                goBackBtn.Focus();
            }

            // NONE BUTTON LOGIC
            var noneBtn = CreateButton(
                "None",
                () => {
                    equipmentGraphicsHandler.UnequipArmorSlot(armorSlot);
                    // Refresh
                    SetupArmorSelection(armorSlot, true);
                },
                () => {
                    // Show difference if we unequip this item
                    float difference = currentEquipmentPhysicalDefense;

                    if (difference > 0)
                    {
                        this.decreasedValueLabel.text = "-" + difference.ToString();
                        this.increasedValueLabel.AddToClassList("hide");
                        this.decreasedValueLabel.RemoveFromClassList("hide");
                    } else if (difference < 0)
                    {
                        this.increasedValueLabel.text = "+" + difference.ToString();
                        this.increasedValueLabel.RemoveFromClassList("hide");
                        this.decreasedValueLabel.AddToClassList("hide");
                    }
                    else
                    {
                        HideIncreaseAndDecreaseLabels();
                    }

                    this.itemToEquipName.text = "To equip: None";
                    this.itemToEquipStatValue.text = "Physical defense bonus: 0";
                });

            if (currentEquipmentName == "None" && focusOnCurrentEquippedButton)
            {
                noneBtn.Focus();
            }

            // FILTERED EQUIPMENT LIST FOR GIVEN ARMOR SLOT
            List<Armor> items = new List<Armor>();
            if (armorSlot == ArmorSlot.Head)
            {
                items = PlayerInventoryManager.instance.GetHelmets();
            }
            else if (armorSlot == ArmorSlot.Chest)
            {
                items = PlayerInventoryManager.instance.GetChestpieces();
            }
            else if (armorSlot == ArmorSlot.Arms)
            {
                items = PlayerInventoryManager.instance.GetGauntlets();
            }
            else if (armorSlot == ArmorSlot.Legs)
            {
                items = PlayerInventoryManager.instance.GetLegwear();
            }

            foreach (Armor item in items)
            {
                var btn = CreateButton(
                    item.name,
                    // Click Callback
                    () =>
                    {
                        equipmentGraphicsHandler.Equip(item);

                        // Refresh
                        SetupArmorSelection(armorSlot, true);

                    },
                    // Focus Callback
                    () =>
                    {
                        if (item.name == currentEquipmentName)
                        {
                            this.itemToEquipName.text = "To equip: " + item.name;
                            this.itemToEquipStatValue.text = "(Item already equipped)";
                            HideIncreaseAndDecreaseLabels();

                            return;
                        }

                        this.itemToEquipName.text = "To equip: " + item.name;
                        this.itemToEquipStatValue.text = "Physical defense bonus: " + item.physicalDefense.ToString();

                        float difference = item.physicalDefense - currentEquipmentPhysicalDefense;
                        UpdateIncreaseAndDecreasedLabelsBasedOnDifference(difference);
                    });

                if (item.name == currentEquipmentName && focusOnCurrentEquippedButton)
                {
                    btn.Focus();
                }
            }
        }

        void SetupAccessory(int index)
        {
            this.menuTitle.text = "Accessory equipment";

            var equippedAccessory = index > 0
                ? PlayerInventoryManager.instance.currentAccessory2
                : PlayerInventoryManager.instance.currentAccessory1;

            this.currentItemName.text = "Equipped: " + (equippedAccessory != null ? equippedAccessory.name : "None");
            HideIncreaseAndDecreaseLabels();

            var goBackBtn = CreateButton(
                "Go Back",
                () => { ShowCurrentEquipmentScreen(); },
                () => {
                    HideItemToEquipInfo();
                });
            goBackBtn.Focus();

            CreateButton(
                "None",
                () => {
                    equipmentGraphicsHandler.UnequipAccessory(index);
                    ShowCurrentEquipmentScreen();
                },
                () => {
                    // Show difference if we unequip this item
                });


            foreach (Accessory accessory in PlayerInventoryManager.instance.GetAccessories())
            {
                CreateButton(
                    accessory.name,
                    // Click Callback
                    () =>
                    {
                        equipmentGraphicsHandler.EquipAccessory(accessory, index);
                        ShowCurrentEquipmentScreen();
                    },
                    // Focus Callback
                    () =>
                    {
                        this.itemToEquipName.text = "To equip: " + accessory.name;
                        this.itemToEquipStatValue.text = accessory.description;
                    });
            }
        }

        void HideItemToEquipInfo()
        {
            this.itemToEquipName.text = "";
            this.itemToEquipStatValue.text = "";

            HideIncreaseAndDecreaseLabels();
        }

        void HideIncreaseAndDecreaseLabels()
        {
            this.increasedValueLabel.AddToClassList("hide");
            this.decreasedValueLabel.AddToClassList("hide");
        }

        void UpdateIncreaseAndDecreasedLabelsBasedOnDifference(float difference)
        {
            if (difference > 0)
            {
                this.increasedValueLabel.text = "+" + difference.ToString();
                this.increasedValueLabel.RemoveFromClassList("hide");
                this.decreasedValueLabel.AddToClassList("hide");
            }
            else if (difference < 0)
            {
                this.decreasedValueLabel.text = "-" + Mathf.Abs(difference).ToString();
                this.decreasedValueLabel.RemoveFromClassList("hide");
                this.increasedValueLabel.AddToClassList("hide");
            }
            else
            {
                this.increasedValueLabel.AddToClassList("hide");
                this.decreasedValueLabel.AddToClassList("hide");
            }
        }

        Button CreateButton(string name, UnityAction clickCallback, UnityAction focusCallback)
        {
            Button btn = new Button();
            btn.text = name;
            btn.name = name.Replace(" ", "");
            btn.AddToClassList("game-button");
            btn.AddToClassList("scroll-view-equipment-button");

            this.SetupButtonClick(btn, clickCallback);

            btn.RegisterCallback<FocusEvent>(ev =>
            {
                focusCallback.Invoke();
            });

            btn.RegisterCallback<PointerEnterEvent>(ev =>
            {
                focusCallback.Invoke();
            });

            this.equipmentListScrollView.Add(btn);

            return btn;
        }

        void ShowCurrentEquipmentScreen()
        {
            FindObjectOfType<UIDocumentEquipmentMenu>(true).Enable();

            this.Disable();
        }

        public override void Enable()
        {
            base.Enable();

            UpdateEquipmentSelection();
        }

        public override void Disable()
        {
            base.Disable();
        }

    }
}
