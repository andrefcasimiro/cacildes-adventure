using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentEquipmentMenuV2 : MonoBehaviour
    {
        MenuManager menuManager;

        AttackStatManager attackStatManager;
        DefenseStatManager defenseStatManager;

        [Header("Settings")]
        public Sprite emptyWeaponSprite;
        public Sprite emptyShieldSprite;
        public Sprite emptyHeadSprite;
        public Sprite emptyChestSprite;
        public Sprite emptyGauntletsSprite;
        public Sprite emptyLegwearSprite;
        public Sprite emptyAccessorySprite;

        VisualElement root;

        private void Awake()
        {
            attackStatManager = FindObjectOfType<AttackStatManager>(true);
            defenseStatManager = FindObjectOfType<DefenseStatManager>(true);
            menuManager = FindObjectOfType<MenuManager>(true);

        }

        private void OnEnable()
        {
            this.root = GetComponent<UIDocument>().rootVisualElement;

            menuManager.SetupNavMenu(this.root);

            menuManager.SetActiveMenu(this.root, "ButtonEquipment");

            var player = Player.instance;

            var playerEquippedWeaponAttack = player.equippedWeapon != null ? attackStatManager.GetWeaponAttack(player.equippedWeapon) : attackStatManager.GetCurrentPhysicalAttack();

            var weaponBonus = Mathf.Abs(playerEquippedWeaponAttack - attackStatManager.GetCurrentPhysicalAttack());
            var defenseBonus = Mathf.Abs(defenseStatManager.GetDefenseAbsorption() - defenseStatManager.GetCurrentPhysicalDefense());
            this.root.Q<VisualElement>("EquipmentStats").Q<VisualElement>("Attack").Q<Label>("Value").text = 
                "/ " + ((player.equippedWeapon != null ? attackStatManager.GetWeaponAttack(player.equippedWeapon) : attackStatManager.GetCurrentPhysicalAttack())) + "";
            this.root.Q<VisualElement>("EquipmentStats").Q<VisualElement>("Attack").Q<Label>("Explanation").text = 
                "" + attackStatManager.GetCurrentPhysicalAttack() + " (Base) + " + weaponBonus.ToString() + " (Weapon)";
            this.root.Q<VisualElement>("EquipmentStats").Q<VisualElement>("Defense").Q<Label>("Value").text = 
                "/ " + (defenseBonus + defenseStatManager.GetCurrentPhysicalDefense()).ToString() + "";
            this.root.Q<VisualElement>("EquipmentStats").Q<VisualElement>("Defense").Q<Label>("Explanation").text = 
                "" + defenseStatManager.GetCurrentPhysicalDefense() + " (Base) + " + defenseBonus.ToString() + " (Gear)";

            this.root.Q<Label>("Attributes").text = "Vitality " + player.vitality + " / Endurance " + player.endurance + " / Strength "
                    + player.strength + " / Dexterity " + player.dexterity;

            var healthStatManager = FindObjectOfType<HealthStatManager>(true);
            var staminaStatManager = FindObjectOfType<StaminaStatManager>(true);
            var playerPoiseController = FindObjectOfType<PlayerPoiseController>(true);
            this.root.Q<Label>("Stats").text = "HP " + healthStatManager.GetMaxHealth() + " / Stamina " + staminaStatManager.GetMaxStamina() + " / Poise " + playerPoiseController.maxPoiseHits
                    + " / Reputation " + player.currentReputation + " / Gold " + player.currentGold;

            // Weapon
            var weaponButtonElement = this.root.Q<VisualElement>("Weapon").Q<VisualElement>("Root").Q<Button>();
            weaponButtonElement.Q<Label>("Label").text = player.equippedWeapon != null ? player.equippedWeapon.name : "Weapon";
            weaponButtonElement.Q<Label>("Value").text = player.equippedWeapon != null
                ? "+ " + (attackStatManager.GetWeaponAttack(player.equippedWeapon) - attackStatManager.GetCurrentPhysicalAttack()) + " ATK"
                : "+ 0 ATK";

            if (player.equippedWeapon != null)
            {
                weaponButtonElement.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(player.equippedWeapon.sprite);
                weaponButtonElement.Q<IMGUIContainer>("Icon").style.opacity =1f;
            }
            else
            {
                weaponButtonElement.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(emptyWeaponSprite);
            }
            menuManager.SetupButton(weaponButtonElement, () =>
            {
                menuManager.OpenEquipmentSelectionScreen(UIDocumentEquipmentSelectionMenuV2.EquipmentType.Weapon);
            });

            // Shield
            var shieldButtonElement = this.root.Q<VisualElement>("Shield").Q<VisualElement>("Root").Q<Button>();
            shieldButtonElement.Q<Label>("Label").text = player.equippedShield != null ? player.equippedShield.name : "Shield";
            shieldButtonElement.Q<Label>("Value").text = player.equippedShield != null
                ? "" + player.equippedShield.defenseAbsorption + "% DEF Absorption"
                : "0% DEF Absorption";

            if (player.equippedShield != null)
            {
                shieldButtonElement.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(player.equippedShield.sprite);
                shieldButtonElement.Q<IMGUIContainer>("Icon").style.opacity = 1f;
            }
            else
            {
                shieldButtonElement.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(emptyShieldSprite);
            }
            menuManager.SetupButton(shieldButtonElement, () =>
            {
                menuManager.OpenEquipmentSelectionScreen(UIDocumentEquipmentSelectionMenuV2.EquipmentType.Shield);
            });

            CreateArmorEquipmentButton(this.root, "Head", UIDocumentEquipmentSelectionMenuV2.EquipmentType.Helmet, Player.instance.equippedHelmet, emptyHeadSprite);
            CreateArmorEquipmentButton(this.root, "Chest", UIDocumentEquipmentSelectionMenuV2.EquipmentType.Armor, Player.instance.equippedArmor, emptyChestSprite);
            CreateArmorEquipmentButton(this.root, "Hands", UIDocumentEquipmentSelectionMenuV2.EquipmentType.Gauntlets, Player.instance.equippedGauntlets, emptyGauntletsSprite);
            CreateArmorEquipmentButton(this.root, "Legs", UIDocumentEquipmentSelectionMenuV2.EquipmentType.Legwear, Player.instance.equippedLegwear, emptyLegwearSprite);

            // Accessory
            var accessoryElement = this.root.Q<VisualElement>("Accessory").Q<VisualElement>("Root").Q<Button>();
            accessoryElement.Q<Label>("Label").text = player.equippedAccessory != null ? player.equippedAccessory.name : "Accessory";
            accessoryElement.Q<Label>("Value").text = player.equippedAccessory != null
                ? player.equippedAccessory.smallEffectDescription
                : "";

            if (player.equippedAccessory != null)
            {
                accessoryElement.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(player.equippedAccessory.sprite);
                accessoryElement.Q<IMGUIContainer>("Icon").style.opacity = 1f;
            }
            else
            {
                accessoryElement.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(emptyAccessorySprite);
            }
            accessoryElement.RegisterCallback<ClickEvent>(ev =>
            {
                menuManager.OpenEquipmentSelectionScreen(UIDocumentEquipmentSelectionMenuV2.EquipmentType.Accessories);
            });
        }

        void CreateArmorEquipmentButton(
            VisualElement root,
            string name,
            UIDocumentEquipmentSelectionMenuV2.EquipmentType equipmentType,
            ArmorBase equippedItem,
            Sprite emptySprite)
        {
            var btnElement = root.Q<VisualElement>(name).Q<VisualElement>("Root").Q<Button>();

            btnElement.Q<Label>("Label").text = equippedItem != null ? equippedItem.name : name;
            btnElement.Q<Label>("Value").text = equippedItem != null
                ? "+ " + equippedItem.physicalDefense + " DEF"
                : "+ 0 DEF";

            if (equippedItem != null)
            {
                btnElement.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(equippedItem.sprite);
                btnElement.Q<IMGUIContainer>("Icon").style.opacity = 1f;
            }
            else
            {
                btnElement.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(emptySprite);
            }

            menuManager.SetupButton(btnElement, () =>
            {
                menuManager.OpenEquipmentSelectionScreen(equipmentType);
            });

        }
    }
}
