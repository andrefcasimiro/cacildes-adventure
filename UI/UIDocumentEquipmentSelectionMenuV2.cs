using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;

namespace AF
{
    public class UIDocumentEquipmentSelectionMenuV2 : MonoBehaviour
    {
        public enum EquipmentType
        {
            Weapon,
            Shield,
            Helmet,
            Armor,
            Gauntlets,
            Legwear,
            Accessories
        }

        public EquipmentType selectedEquipmentType = EquipmentType.Weapon;

        public VisualTreeAsset equipmentSelectionItem;

        private MenuManager menuManager;
        private EquipmentGraphicsHandler equipmentGraphicsHandler;
        private AttackStatManager attackStatManager;
        private PlayerInventory playerInventory;

        private Dictionary<EquipmentType, System.Action> unequipActions = new Dictionary<EquipmentType, System.Action>();

        VisualElement root;

        private void Awake()
        {
            menuManager = FindObjectOfType<MenuManager>(true);
            equipmentGraphicsHandler = FindObjectOfType<EquipmentGraphicsHandler>(true);
            playerInventory = FindObjectOfType<PlayerInventory>(true);
            attackStatManager = FindObjectOfType<AttackStatManager>(true);
        }

        private void OnEnable()
        {
            unequipActions.Clear();

            // Unequip
            unequipActions.Add(EquipmentType.Weapon, equipmentGraphicsHandler.UnequipWeapon);
            unequipActions.Add(EquipmentType.Shield, equipmentGraphicsHandler.UnequipShield);
            unequipActions.Add(EquipmentType.Helmet, equipmentGraphicsHandler.UnequipHelmet);
            unequipActions.Add(EquipmentType.Armor, equipmentGraphicsHandler.UnequipArmor);
            unequipActions.Add(EquipmentType.Gauntlets, equipmentGraphicsHandler.UnequipGauntlet);
            unequipActions.Add(EquipmentType.Legwear, equipmentGraphicsHandler.UnequipLegwear);
            unequipActions.Add(EquipmentType.Accessories, OnUnequipAccessory);

            this.root = GetComponent<UIDocument>().rootVisualElement;
            menuManager.SetupNavMenu(root);

            // Hide Item Preview
            HideItemInfo();

            menuManager.SetupButton(this.root.Q<Button>("GoBackButton"), () =>
            {
                menuManager.OpenEquipmentListMenu();
            });

            // Unequip Logic
            menuManager.SetupButton(this.root.Q<Button>("UnequipButton"), () =>
            {
                unequipActions[selectedEquipmentType].Invoke();

                menuManager.PlayClick();

                menuManager.OpenEquipmentListMenu();
            });

            this.root.Q<ScrollView>().Clear();

            if (selectedEquipmentType == EquipmentType.Weapon)
            {
                DrawWeaponsList();
            }
            else if (selectedEquipmentType == EquipmentType.Shield)
            {
                DrawShieldsList();
            }
            else if (selectedEquipmentType == EquipmentType.Helmet)
            {
                DrawArmorItemsList(EquipmentType.Helmet, Player.instance.equippedHelmet);
            }
            else if (selectedEquipmentType == EquipmentType.Armor)
            {
                DrawArmorItemsList(EquipmentType.Armor, Player.instance.equippedArmor);
            }
            else if (selectedEquipmentType == EquipmentType.Gauntlets)
            {
                DrawArmorItemsList(EquipmentType.Gauntlets, Player.instance.equippedGauntlets);
            }
            else if (selectedEquipmentType == EquipmentType.Legwear)
            {
                DrawArmorItemsList(EquipmentType.Legwear, Player.instance.equippedLegwear);
            }
            else if (selectedEquipmentType == EquipmentType.Accessories)
            {
                DrawAccessoriesList();
            }
        }

        #region Weapons
        void DrawWeaponsList()
        {
            var playerWeapons = playerInventory.GetWeapons();
                    if (playerWeapons.Count > 0)
            {
                foreach (var weapon in playerInventory.GetWeapons())
                {
                    VisualElement weaponButton = equipmentSelectionItem.CloneTree();

                    if (weapon == Player.instance.equippedWeapon)
                    {
                        weaponButton.Q<VisualElement>("Item").style.opacity = 0.25f;
                    }
                    else
                    {
                        weaponButton.Q<VisualElement>("Item").style.opacity = 1f;
                    }

                    Button wpBtn = weaponButton.Q<Button>();
                    menuManager.SetupButton(wpBtn, () =>
                    {
                        equipmentGraphicsHandler.EquipWeapon(weapon);
                        menuManager.PlayClick();
                        menuManager.OpenEquipmentListMenu();
                    });

                    wpBtn.RegisterCallback<FocusInEvent>(ev =>
                    {
                        ShowWeaponItemPreview(weapon);

                        root.Q<ScrollView>().ScrollTo(wpBtn);
                    });
                    wpBtn.RegisterCallback<FocusOutEvent>(ev =>
                    {
                        HideItemInfo();
                    });

                    wpBtn.RegisterCallback<MouseOverEvent>(ev =>
                    {
                        ShowWeaponItemPreview(weapon);

                        root.Q<ScrollView>().ScrollTo(wpBtn);
                    });
                    wpBtn.RegisterCallback<MouseOutEvent>(ev =>
                    {
                        HideItemInfo();
                    });

                    weaponButton.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(weapon.sprite);
                    weaponButton.Q<Label>("Name").text = weapon.name.GetText();

                    var valueLabelElement = weaponButton.Q<Label>("Value");

                    var thisWeaponAttack = attackStatManager.GetWeaponAttack(weapon) - attackStatManager.GetCurrentPhysicalAttack();
                    var equippedWeaponAttack = Player.instance.equippedWeapon != null ? attackStatManager.GetWeaponAttack(Player.instance.equippedWeapon) - attackStatManager.GetCurrentPhysicalAttack() : 0f;

                    bool isBetterWeapon = thisWeaponAttack - equippedWeaponAttack > 0;

                    if (isBetterWeapon)
                    {
                        valueLabelElement.text = "+ " + Mathf.Abs(equippedWeaponAttack - thisWeaponAttack) + " ATK";
                        valueLabelElement.RemoveFromClassList("same-value-equipment");
                        valueLabelElement.RemoveFromClassList("lower-value-equipment");
                        valueLabelElement.AddToClassList("higher-value-equipment");
                    }
                    else
                    {
                        bool isLowerWeapon = thisWeaponAttack - equippedWeaponAttack < 0;

                        if (isLowerWeapon)
                        {
                            valueLabelElement.text = "- " + Mathf.Abs(attackStatManager.GetWeaponAttack(weapon) - equippedWeaponAttack) + " ATK";
                            valueLabelElement.RemoveFromClassList("same-value-equipment");
                            valueLabelElement.AddToClassList("lower-value-equipment");
                            valueLabelElement.RemoveFromClassList("higher-value-equipment");
                        }
                        else
                        {
                            if (weapon == Player.instance.equippedWeapon)
                            {
                                valueLabelElement.text = "(Equipped)";
                            }
                            else
                            {
                                valueLabelElement.text = "+ 0 ATK";
                            }
                            valueLabelElement.AddToClassList("same-value-equipment");
                            valueLabelElement.RemoveFromClassList("lower-value-equipment");
                            valueLabelElement.RemoveFromClassList("higher-value-equipment");
                        }
                    }

                    this.root.Q<ScrollView>().Add(weaponButton);
                }
            }
        }

        void ShowWeaponItemPreview(Weapon weapon)
        {
            var itemPreviewElement = root.Q<VisualElement>("ItemPreview");
            var itemStats = root.Q<VisualElement>("ItemStats");

            itemPreviewElement.Q<Label>("Title").text = weapon.name.GetText();
            itemPreviewElement.Q<Label>("Description").text = weapon.description.GetText();
            itemPreviewElement.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(weapon.sprite);

            itemStats.Q<Label>("Damage").text = weapon.physicalAttack + " (Base Damage) + "
                    + attackStatManager.GetStrengthBonusFromWeapon(weapon) + " (" + weapon.strengthScaling + " Strength) + "
                    + attackStatManager.GetDexterityBonusFromWeapon(weapon) + " (" + weapon.dexterityScaling + " Dexterity)";

            var bonusLabel = itemStats.Q<Label>("Bonus");
            bonusLabel.text = "";
            if (weapon.fireAttack != 0)
            {
                if (bonusLabel.text.Length > 0)
                {
                    bonusLabel.text += " / ";
                }

                bonusLabel.text += "Fire Bonus + " + weapon.fireAttack;
            }
            if (weapon.frostAttack != 0)
            {
                if (bonusLabel.text.Length > 0)
                {
                    bonusLabel.text += " / ";
                }

                bonusLabel.text += "Frost Bonus + " + weapon.frostAttack;
            }

            if (weapon.statusEffects.Length > 0)
            {
                foreach (var weaponStatusEffect in weapon.statusEffects)
                {
                    if (bonusLabel.text.Length > 0)
                    {
                        bonusLabel.text += " / ";
                    }

                    bonusLabel.text += weaponStatusEffect.statusEffect.name + " Buildup + " + weaponStatusEffect.amountPerHit;
                }
            }

            if (bonusLabel.text.Length <= 0)
            {
                bonusLabel.style.display = DisplayStyle.None;
            }
            else
            {
                bonusLabel.style.display = DisplayStyle.Flex;
            }

            itemStats.Q<Label>("Value").text = "Value " + weapon.value + " / Speed Loss " + (weapon.speedPenalty != 0 ? "-" + weapon.speedPenalty.ToString().Replace(",", ".") : "0"); 

            itemPreviewElement.style.visibility = Visibility.Visible;
            itemStats.style.visibility = Visibility.Visible;
        }
        #endregion

        #region Shields
        void DrawShieldsList()
        {
            var playerShields = playerInventory.GetShields();
            if (playerShields.Count > 0)
            {
                foreach (var shield in playerShields)
                {
                    VisualElement shieldButton = equipmentSelectionItem.CloneTree();

                    if (shield == Player.instance.equippedShield)
                    {
                        shieldButton.Q<VisualElement>("Item").style.opacity = 0.25f;
                    }
                    else
                    {
                        shieldButton.Q<VisualElement>("Item").style.opacity = 1f;
                    }

                    Button shieldBtn = shieldButton.Q<Button>();
                    menuManager.SetupButton(shieldBtn, () =>
                    {
                        equipmentGraphicsHandler.EquipShield(shield);
                        menuManager.PlayClick();
                        menuManager.OpenEquipmentListMenu();
                    });

                    shieldBtn.RegisterCallback<FocusInEvent>(ev =>
                    {
                        ShowShieldItemPreview(shield);

                        root.Q<ScrollView>().ScrollTo(shieldBtn);
                    });
                    shieldBtn.RegisterCallback<FocusOutEvent>(ev =>
                    {
                        HideItemInfo();
                    });

                    shieldBtn.RegisterCallback<MouseOverEvent>(ev =>
                    {
                        ShowShieldItemPreview(shield);

                        root.Q<ScrollView>().ScrollTo(shieldBtn);
                    });
                    shieldBtn.RegisterCallback<MouseOutEvent>(ev =>
                    {
                        HideItemInfo();
                    });

                    shieldButton.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(shield.sprite);
                    shieldButton.Q<Label>("Name").text = shield.name.GetText();

                    var valueLabelElement = shieldButton.Q<Label>("Value");

                    var equippedShieldAbsorption = Player.instance.equippedShield != null ? Player.instance.equippedShield.defenseAbsorption : 0f;

                    bool isBetterShield = shield.defenseAbsorption - equippedShieldAbsorption > 0;

                    if (isBetterShield)
                    {
                        valueLabelElement.text = "+ " + Mathf.Abs(equippedShieldAbsorption - shield.defenseAbsorption) + "% ATK Absorption";
                        valueLabelElement.RemoveFromClassList("same-value-equipment");
                        valueLabelElement.RemoveFromClassList("lower-value-equipment");
                        valueLabelElement.AddToClassList("higher-value-equipment");
                    }
                    else
                    {
                        bool isWorseShield = shield.defenseAbsorption - equippedShieldAbsorption < 0;

                        if (isWorseShield)
                        {
                            valueLabelElement.text = "- " + Mathf.Abs(equippedShieldAbsorption - shield.defenseAbsorption) + "% ATK Absorption";
                            valueLabelElement.RemoveFromClassList("same-value-equipment");
                            valueLabelElement.AddToClassList("lower-value-equipment");
                            valueLabelElement.RemoveFromClassList("higher-value-equipment");
                        }
                        else
                        {
                            if (shield == Player.instance.equippedShield)
                            {
                                valueLabelElement.text = "(Equipped)";
                            }
                            else
                            {
                                valueLabelElement.text = "+ 0% ATK Absorption";
                            }
                            valueLabelElement.AddToClassList("same-value-equipment");
                            valueLabelElement.RemoveFromClassList("lower-value-equipment");
                            valueLabelElement.RemoveFromClassList("higher-value-equipment");
                        }
                    }

                    this.root.Q<ScrollView>().Add(shieldButton);
                }
            }
        }

        void ShowShieldItemPreview(Shield shield)
        {
            var itemPreviewElement = root.Q<VisualElement>("ItemPreview");
            var itemStats = root.Q<VisualElement>("ItemStats");

            itemPreviewElement.Q<Label>("Title").text = shield.name.GetText();
            itemPreviewElement.Q<Label>("Description").text = shield.description.GetText();
            itemPreviewElement.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(shield.sprite);

            itemStats.Q<Label>("Damage").text = shield.defenseAbsorption + "% (Enemy ATK Absorption) / "
                    + shield.blockStaminaCost + " (Stamina Cost Per Block)";

            var bonusLabel = itemStats.Q<Label>("Bonus");
            bonusLabel.text = "";
            if (shield.fireDefense != 0)
            {
                if (bonusLabel.text.Length > 0)
                {
                    bonusLabel.text += " / ";
                }

                bonusLabel.text += "Fire DEF Bonus + " + shield.fireDefense;
            }

            if (shield.frostDefense != 0)
            {
                if (bonusLabel.text.Length > 0)
                {
                    bonusLabel.text += " / ";
                }

                bonusLabel.text += "Frost DEF Bonus + " + shield.frostDefense;
            }

            if (bonusLabel.text.Length <= 0)
            {
                bonusLabel.style.display = DisplayStyle.None;
            }
            else
            {
                bonusLabel.style.display = DisplayStyle.Flex;
            }

            itemStats.Q<Label>("Value").text = "Value " + shield.value;

            itemPreviewElement.style.visibility = Visibility.Visible;
            itemStats.style.visibility = Visibility.Visible;
        }

        #endregion

        #region Armors
        void DrawArmorItemsList(EquipmentType equipmentType, ArmorBase currentEquippedItem)
        {
            List<ArmorBase> items = new List<ArmorBase>();

            if (equipmentType == EquipmentType.Helmet)
            {
                foreach (var i in playerInventory.GetHelmets()) { items.Add(i); }
            }
            if (equipmentType == EquipmentType.Armor)
            {
                foreach (var i in playerInventory.GetArmors()) { items.Add(i); }
            }
            if (equipmentType == EquipmentType.Gauntlets)
            {
                foreach (var i in playerInventory.GetGauntlets()) { items.Add(i); }
            }
            if (equipmentType == EquipmentType.Legwear)
            {
                foreach (var i in playerInventory.GetLegwears()) { items.Add(i); }
            }

            if (items.Count > 0)
            {
                foreach (var item in items)
                {
                    VisualElement itemButton = equipmentSelectionItem.CloneTree();

                    if (item == currentEquippedItem)
                    {
                        itemButton.Q<VisualElement>("Item").style.opacity = 0.25f;
                    }
                    else
                    {
                        itemButton.Q<VisualElement>("Item").style.opacity = 1f;
                    }

                    var itemBtn = itemButton.Q<Button>();
                    menuManager.SetupButton(itemBtn, () =>
                    {
                        if (equipmentType == EquipmentType.Helmet) { equipmentGraphicsHandler.EquipHelmet(item as Helmet); }
                        if (equipmentType == EquipmentType.Armor) { equipmentGraphicsHandler.EquipArmor(item as ArmorBase); }
                        if (equipmentType == EquipmentType.Gauntlets) { equipmentGraphicsHandler.EquipGauntlet(item as Gauntlet); }
                        if (equipmentType == EquipmentType.Legwear) { equipmentGraphicsHandler.EquipLegwear(item as Legwear); }

                        menuManager.PlayClick();
                        menuManager.OpenEquipmentListMenu();
                    });

                    itemBtn.RegisterCallback<FocusInEvent>(ev =>
                    {
                        ShowArmorItemPreview(item as ArmorBase);
                        root.Q<ScrollView>().ScrollTo(itemBtn);
                    });
                    itemBtn.RegisterCallback<FocusOutEvent>(ev =>
                    {
                        HideItemInfo();
                    });

                    itemBtn.RegisterCallback<PointerEnterEvent>(ev =>
                    {
                        ShowArmorItemPreview(item as ArmorBase);
                        root.Q<ScrollView>().ScrollTo(itemBtn);
                    });
                    itemBtn.RegisterCallback<PointerLeaveEvent>(ev =>
                    {
                        HideItemInfo();
                    });

                    itemButton.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(item.sprite);
                    itemButton.Q<Label>("Name").text = item.name.GetText();

                    var valueLabelElement = itemButton.Q<Label>("Value");

                    var thisItemDefense = item.physicalDefense;
                    var equippedItemDefense = currentEquippedItem != null ? currentEquippedItem.physicalDefense : 0f;

                    bool isBetter = thisItemDefense - equippedItemDefense > 0;

                    if (isBetter)
                    {
                        valueLabelElement.text = "+ " + Mathf.Abs(thisItemDefense - equippedItemDefense) + " DEF";
                        valueLabelElement.RemoveFromClassList("same-value-equipment");
                        valueLabelElement.RemoveFromClassList("lower-value-equipment");
                        valueLabelElement.AddToClassList("higher-value-equipment");
                    }
                    else
                    {
                        bool isWorse = thisItemDefense - equippedItemDefense < 0;

                        if (isWorse)
                        {
                            valueLabelElement.text = "- " + Mathf.Abs(equippedItemDefense - thisItemDefense) + " DEF";
                            valueLabelElement.RemoveFromClassList("same-value-equipment");
                            valueLabelElement.AddToClassList("lower-value-equipment");
                            valueLabelElement.RemoveFromClassList("higher-value-equipment");
                        }
                        else
                        {
                            if (item == currentEquippedItem)
                            {
                                valueLabelElement.text = "(Equipped)";
                            }
                            else
                            {
                                valueLabelElement.text = "+ 0 DEF";
                            }
                            valueLabelElement.AddToClassList("same-value-equipment");
                            valueLabelElement.RemoveFromClassList("lower-value-equipment");
                            valueLabelElement.RemoveFromClassList("higher-value-equipment");
                        }
                    }

                    this.root.Q<ScrollView>().Add(itemButton);
                }
            }
        }

        void ShowArmorItemPreview(ArmorBase armor)
        {
            var itemPreviewElement = root.Q<VisualElement>("ItemPreview");
            var itemStats = root.Q<VisualElement>("ItemStats");

            itemPreviewElement.Q<Label>("Title").text = armor.name.GetText();
            itemPreviewElement.Q<Label>("Description").text = armor.description.GetText();
            itemPreviewElement.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(armor.sprite);

            itemStats.Q<Label>("Damage").text = "Defense + " + armor.physicalDefense;
            if (armor.fireDefense != 0)
            {
                itemStats.Q<Label>("Damage").text += " / Fire Defense + " + armor.fireDefense;
            }
            if (armor.frostDefense != 0)
            {
                itemStats.Q<Label>("Damage").text += " / Frost Defense + " + armor.frostDefense;
            }

            foreach (var statusEffectResistance in armor.statusEffectResistances)
            {
                itemStats.Q<Label>("Damage").text += " / " + statusEffectResistance.statusEffect.name + " Resistance + " + statusEffectResistance.resistanceBonus;
            }

            if (itemStats.Q<Label>("Damage").text.Length <= 0)
            {
                itemStats.Q<Label>("Damage").style.display = DisplayStyle.None;
            }
            else
            {
                itemStats.Q<Label>("Damage").style.display = DisplayStyle.Flex;
            }

            itemStats.Q<Label>("Bonus").text = "";

            itemStats.Q<Label>("Bonus").text += "Value " + armor.value;

            if (armor.poiseBonus != 0)
            {
                itemStats.Q<Label>("Bonus").text += " / Poise Bonus +" + armor.poiseBonus;
            }
            if (armor.speedPenalty != 0)
            {
                itemStats.Q<Label>("Bonus").text += " / Speed Penalty -" + armor.speedPenalty.ToString().Replace(",", ".");
            }

            if (itemStats.Q<Label>("Bonus").text.Length <= 0)
            {
                itemStats.Q<Label>("Bonus").style.display = DisplayStyle.None;
            }
            else
            {
                itemStats.Q<Label>("Bonus").style.display = DisplayStyle.Flex;
            }

            itemStats.Q<Label>("Value").style.visibility = Visibility.Hidden;

            itemPreviewElement.style.visibility = Visibility.Visible;
            itemStats.style.visibility = Visibility.Visible;
        }
        #endregion

        #region Accessories
        void DrawAccessoriesList()
        {
            var playerAccessories = playerInventory.GetAccessories();
            if (playerAccessories.Count > 0)
            {
                foreach (var accessory in playerAccessories)
                {
                    VisualElement accessoryButton = equipmentSelectionItem.CloneTree();

                    if (accessory == Player.instance.equippedAccessory)
                    {
                        accessoryButton.Q<VisualElement>("Item").style.opacity = 0.25f;
                    }
                    else
                    {
                        accessoryButton.Q<VisualElement>("Item").style.opacity = 1f;
                    }

                    Button accessoryBtn = accessoryButton.Q<Button>();
                    menuManager.SetupButton(accessoryBtn, () =>
                    {
                        CheckIfAccessoryShouldBeDestroyed(Player.instance.equippedAccessory);

                        equipmentGraphicsHandler.EquipAccessory(accessory);
                        menuManager.PlayClick();
                        menuManager.OpenEquipmentListMenu();
                    });

                    accessoryBtn.RegisterCallback<FocusInEvent>(ev =>
                    {
                        ShowAccessoryItemPreview(accessory);

                        root.Q<ScrollView>().ScrollTo(accessoryBtn);
                    });
                    accessoryBtn.RegisterCallback<FocusOutEvent>(ev =>
                    {
                        HideItemInfo();
                    });

                    accessoryBtn.RegisterCallback<MouseOverEvent>(ev =>
                    {
                        ShowAccessoryItemPreview(accessory);

                        root.Q<ScrollView>().ScrollTo(accessoryBtn);
                    });
                    accessoryBtn.RegisterCallback<MouseOutEvent>(ev =>
                    {
                        HideItemInfo();
                    });

                    accessoryButton.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(accessory.sprite);
                    accessoryButton.Q<Label>("Name").text = accessory.name.GetText();

                    var valueLabelElement = accessoryButton.Q<Label>("Value");
                    valueLabelElement.text = accessory.smallEffectDescription.GetText();
                    valueLabelElement.style.fontSize = 11;
                    valueLabelElement.style.paddingRight = 5;

                    valueLabelElement.RemoveFromClassList("same-value-equipment");
                    valueLabelElement.RemoveFromClassList("lower-value-equipment");
                    valueLabelElement.RemoveFromClassList("higher-value-equipment");

                    this.root.Q<ScrollView>().Add(accessoryButton);
                }
            }
        }

        void ShowAccessoryItemPreview(Accessory accessory)
        {
            var itemPreviewElement = root.Q<VisualElement>("ItemPreview");
            var itemStats = root.Q<VisualElement>("ItemStats");

            itemPreviewElement.Q<Label>("Title").text = accessory.name.GetText();
            itemPreviewElement.Q<Label>("Description").text = accessory.description.GetText();
            itemPreviewElement.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(accessory.sprite);

            itemStats.Q<Label>("Damage").text = " ";

            var bonusLabel = itemStats.Q<Label>("Bonus");
            bonusLabel.text = "";

            if (accessory.fireDefense != 0)
            {
                if (bonusLabel.text.Length > 0)
                {
                    bonusLabel.text += " / ";
                }

                bonusLabel.text += "Fire DEF Bonus + " + accessory.fireDefense;
            }

            if (accessory.frostDefense != 0)
            {
                if (bonusLabel.text.Length > 0)
                {
                    bonusLabel.text += " / ";
                }

                bonusLabel.text += "Frost DEF Bonus + " + accessory.frostDefense;
            }


            if (bonusLabel.text.Length <= 0)
            {
                bonusLabel.style.display = DisplayStyle.None;
            }
            else
            {
                bonusLabel.style.display = DisplayStyle.Flex;
            }

            foreach (var statusEffectResistance in accessory.statusEffectResistances)
            {
                itemStats.Q<Label>("Damage").text += " / " + statusEffectResistance.statusEffect.name + " Resistance + " + statusEffectResistance.resistanceBonus;
            }

            itemStats.Q<Label>("Value").text = "";

            itemStats.Q<Label>("Value").text += "Value " + accessory.value;

            if (accessory.poiseBonus != 0)
            {
                itemStats.Q<Label>("Value").text += " / Poise Bonus +" + accessory.poiseBonus;
            }

            if (accessory.speedPenalty < 0)
            {
                itemStats.Q<Label>("Value").text += " / Speed Penalty +" + Mathf.Abs(accessory.speedPenalty).ToString().Replace(",", ".");
            }
            else if (accessory.speedPenalty > 0)
            {
                itemStats.Q<Label>("Value").text += " / Speed Penalty -" + accessory.speedPenalty.ToString().Replace(",", ".");
            }


            if (itemStats.Q<Label>("Value").text.Length <= 0)
            {
                itemStats.Q<Label>("Value").style.display = DisplayStyle.None;
            }
            else
            {
                itemStats.Q<Label>("Value").style.display = DisplayStyle.Flex;
            }

            itemPreviewElement.style.visibility = Visibility.Visible;
            itemStats.style.visibility = Visibility.Visible;
        }

        #endregion

        void HideItemInfo()
        {
            root.Q<VisualElement>("ItemPreview").style.visibility = Visibility.Hidden;
            root.Q<VisualElement>("ItemStats").style.visibility = Visibility.Hidden;
        }

        void OnUnequipAccessory()
        {
            var unequipedAccessory = Player.instance.equippedAccessory;

            equipmentGraphicsHandler.UnequipAccessory();

            CheckIfAccessoryShouldBeDestroyed(unequipedAccessory);
        }

        void CheckIfAccessoryShouldBeDestroyed(Accessory unequipedAccessory)
        {
            if (unequipedAccessory != null && unequipedAccessory.destroyOnUnequip)
            {
                FindObjectOfType<PlayerInventory>(true).RemoveItem(unequipedAccessory, 1);

                var notificationManager = FindObjectOfType<NotificationManager>(true);
                if (notificationManager != null)
                {
                    notificationManager.ShowNotification(unequipedAccessory.name + " was destroyed by unequiping it.", notificationManager.systemError);

                    if (unequipedAccessory.onUnequipDestroySoundclip != null)
                    {
                        BGMManager.instance.PlaySound(unequipedAccessory.onUnequipDestroySoundclip, null);
                    }
                }

                SaveSystem.instance.SaveGameData(unequipedAccessory.name + " destroyed");
            }
        }

    }
}
