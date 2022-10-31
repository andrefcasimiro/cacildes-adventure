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
            unequipActions.Add(EquipmentType.Accessories, equipmentGraphicsHandler.UnequipAccessory);

            this.root = GetComponent<UIDocument>().rootVisualElement;
            menuManager.SetupNavMenu(root);

            // Hide Item Preview
            HideItemInfo();

            this.root.Q<Button>("GoBackButton").RegisterCallback<ClickEvent>(ev =>
            {
                menuManager.OpenEquipmentListMenu();
            });

            // Unequip Logic
            this.root.Q<Button>("UnequipButton").RegisterCallback<ClickEvent>(ev =>
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


                    weaponButton.RegisterCallback<ClickEvent>(ev =>
                    {
                        equipmentGraphicsHandler.EquipWeapon(weapon);
                        menuManager.PlayClick();
                        menuManager.OpenEquipmentListMenu();
                    });

                    weaponButton.RegisterCallback<MouseEnterEvent>(ev =>
                    {
                        ShowWeaponItemPreview(weapon);
                    });
                    weaponButton.RegisterCallback<MouseLeaveEvent>(ev =>
                    {
                        HideItemInfo();
                    });

                    weaponButton.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(weapon.sprite);
                    weaponButton.Q<Label>("Name").text = weapon.name;

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
                            valueLabelElement.text = "- " + (attackStatManager.GetWeaponAttack(weapon) - attackStatManager.GetWeaponAttack(Player.instance.equippedWeapon)) + " ATK";
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

            itemPreviewElement.Q<Label>("Title").text = weapon.name;
            itemPreviewElement.Q<Label>("Description").text = weapon.description;
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

            itemStats.Q<Label>("Value").text = "Value " + weapon.value + " / Weight " + weapon.weight; 

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


                    itemButton.RegisterCallback<ClickEvent>(ev =>
                    {
                        if (equipmentType == EquipmentType.Helmet) { equipmentGraphicsHandler.EquipHelmet(item as Helmet); }
                        if (equipmentType == EquipmentType.Armor) { equipmentGraphicsHandler.EquipArmor(item as ArmorBase); }
                        if (equipmentType == EquipmentType.Gauntlets) { equipmentGraphicsHandler.EquipGauntlet(item as Gauntlet); }
                        if (equipmentType == EquipmentType.Legwear) { equipmentGraphicsHandler.EquipLegwear(item as Legwear); }

                        menuManager.PlayClick();
                        menuManager.OpenEquipmentListMenu();
                    });

                    itemButton.RegisterCallback<MouseEnterEvent>(ev =>
                    {
                        ShowArmorItemPreview(item as ArmorBase);
                    });
                    itemButton.RegisterCallback<MouseLeaveEvent>(ev =>
                    {
                        HideItemInfo();
                    });

                    itemButton.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(item.sprite);
                    itemButton.Q<Label>("Name").text = item.name;

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

            itemPreviewElement.Q<Label>("Title").text = armor.name;
            itemPreviewElement.Q<Label>("Description").text = armor.description;
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
            if (armor.poisonResistance != 0)
            {
                itemStats.Q<Label>("Damage").text += " / Poison Resistance + " + armor.poisonResistance;
            }
            if (armor.bleedResistance != 0)
            {
                itemStats.Q<Label>("Damage").text += " / Bleed Resistance + " + armor.bleedResistance;
            }

            itemStats.Q<Label>("Bonus").text = "Value " + armor.value + " / Weight " + armor.weight;

            itemStats.Q<Label>("Value").style.visibility = Visibility.Hidden;

            itemPreviewElement.style.visibility = Visibility.Visible;
            itemStats.style.visibility = Visibility.Visible;
        }
        #endregion

        void HideItemInfo()
        {
            root.Q<VisualElement>("ItemPreview").style.visibility = Visibility.Hidden;
            root.Q<VisualElement>("ItemStats").style.visibility = Visibility.Hidden;
        }


    }
}
