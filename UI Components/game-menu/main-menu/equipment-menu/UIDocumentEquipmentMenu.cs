using UnityEngine.UIElements;

namespace AF
{
    public enum EquipmentSelectionMode
    {
        WEAPON,
        SHIELD,
        HEAD,
        CHEST,
        ARMS,
        LEGS,
        ACCESSORY_1,
        ACCESSORY_2,
    }

    public class UIDocumentEquipmentMenu : UIDocumentBase
    {
        EquipmentGraphicsHandler equipmentGraphicsHandler;

        // Current Equipment Screen Refs
        Button equippedWeaponButton;
        Button equippedShieldButton;
        Button equippedBowButton;
        Button equippedArrowsButton;
        Button equippedHeadButton;
        Button equippedChestButton;
        Button equippedArmsButton;
        Button equippedLegsButton;
        Button equippedAccessory1Button;
        Button equippedAccessory2Button;

        public EquipmentSelectionMode equipmentSelectionMode;

        private void Awake()
        {
            equipmentGraphicsHandler = FindObjectOfType<EquipmentGraphicsHandler>();
        }

        protected override void Start()
        {
            base.Start();

            // CURRENT EQUIPMENT BUTTONS
            equippedWeaponButton = this.root.Q<Button>("WeaponButton");
            SetupButtonClick(equippedWeaponButton, () =>
            {
                equipmentSelectionMode = EquipmentSelectionMode.WEAPON;
                ShowEquipmentSelection();
            });

            equippedShieldButton = this.root.Q<Button>("ShieldButton");
            SetupButtonClick(equippedShieldButton, () =>
            {
                equipmentSelectionMode = EquipmentSelectionMode.SHIELD;
                ShowEquipmentSelection();
            });

            equippedHeadButton = this.root.Q<Button>("HeadButton");
            SetupButtonClick(equippedHeadButton, () =>
            {
                equipmentSelectionMode = EquipmentSelectionMode.HEAD;
                ShowEquipmentSelection();
            });

            equippedChestButton = this.root.Q<Button>("ChestButton");
            SetupButtonClick(equippedChestButton, () =>
            {
                equipmentSelectionMode = EquipmentSelectionMode.CHEST;
                ShowEquipmentSelection();
            });

            equippedArmsButton = this.root.Q<Button>("ArmsButton");
            SetupButtonClick(equippedArmsButton, () =>
            {
                equipmentSelectionMode = EquipmentSelectionMode.ARMS;
                ShowEquipmentSelection();
            });

            equippedLegsButton = this.root.Q<Button>("LegsButton");
            SetupButtonClick(equippedLegsButton, () =>
            {
                equipmentSelectionMode = EquipmentSelectionMode.LEGS;
                ShowEquipmentSelection();
            });

            equippedAccessory1Button = this.root.Q<Button>("Accessory1Button");
            SetupButtonClick(equippedAccessory1Button, () =>
            {
                equipmentSelectionMode = EquipmentSelectionMode.ACCESSORY_1;
                ShowEquipmentSelection();
            });

            /*equippedAccessory2Button = this.root.Q<Button>("Accessory2Button");
            SetupButtonClick(equippedAccessory2Button, () =>
            {
                equipmentSelectionMode = EquipmentSelectionMode.ACCESSORY_2;
                ShowEquipmentSelection();
            });*/

            this.Disable();
        }

        void ShowEquipmentSelection()
        {
            FindObjectOfType<UIDocumentEquipmentSelectionMenu>(true).Enable();

            this.Disable();
        }

        public override void Enable()
        {
            base.Enable();

            // Update button values with current player equipment
            UpdateCurrentEquipmentButtonsGUI();

            this.equippedWeaponButton.Focus();
        }

        void UpdateCurrentEquipmentButtonsGUI()
        {
            var weapon = PlayerInventoryManager.instance.currentWeapon;
            var shield = PlayerInventoryManager.instance.currentShield;
            var helmet = PlayerInventoryManager.instance.currentHelmet;
            var chest = PlayerInventoryManager.instance.currentChest;
            var gauntlets = PlayerInventoryManager.instance.currentGauntlets;
            var legwear = PlayerInventoryManager.instance.currentLegwear;
            var accessory1 = PlayerInventoryManager.instance.currentAccessory1;
            var accessory2 = PlayerInventoryManager.instance.currentAccessory2;

            equippedWeaponButton.Q<Label>("Value").text = weapon != null ? weapon.name : "None";
            equippedShieldButton.Q<Label>("Value").text = shield != null ? shield.name : "None";
            equippedHeadButton.Q<Label>("Value").text = helmet != null ? helmet.name : "None";
            equippedChestButton.Q<Label>("Value").text = chest != null ? chest.name : "None";
            equippedArmsButton.Q<Label>("Value").text = gauntlets != null ? gauntlets.name : "None";
            equippedLegsButton.Q<Label>("Value").text = legwear != null ? legwear.name : "None";
            equippedAccessory1Button.Q<Label>("Value").text = accessory1 != null ? accessory1.name : "None";
            // equippedAccessory2Button.Q<Label>("Value").text = accessory2 != null ? accessory2.name : "None";
        }

        public override void Disable()
        {
            base.Disable();
        }

    }
}
