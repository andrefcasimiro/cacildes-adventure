using AF.Events;
using AF.Inventory;
using AF.Stats;
using TigerForge;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace AF
{

    public class UIDocumentPlayerHUDV2 : MonoBehaviour
    {

        UIDocument uIDocument => GetComponent<UIDocument>();
        public VisualElement root;

        VisualElement healthContainer;
        VisualElement healthFill;
        VisualElement staminaContainer;
        VisualElement staminaFill;
        VisualElement manaContainer;
        VisualElement manaFill;

        [Header("Graphic Settings")]
        public float healthContainerBaseWidth = 180;
        public float staminaContainerBaseWidth = 150;
        public float manaContainerBaseWidth = 150;
        float _containerMultiplierPerLevel = 10f;

        Label quickItemName, arrowsLabel;
        IMGUIContainer shieldBlockedIcon;


        [Header("Components")]
        public StatsBonusController playerStatsBonusController;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;
        public InventoryDatabase inventoryDatabase;
        public PlayerStatsDatabase playerStatsDatabase;
        public QuestsDatabase questsDatabase;
        public GameSettings gameSettings;

        [Header("Unequipped Textures")]
        public Texture2D unequippedSpellSlot;
        public Texture2D unequippedWeaponSlot;
        public Texture2D unequippedConsumableSlot;
        public Texture2D unequippedShieldSlot;
        public Texture2D unequippedArrowSlot;

        [Header("Components")]
        public PlayerManager playerManager;
        public EquipmentGraphicsHandler equipmentGraphicsHandler;

        IMGUIContainer spellSlotContainer, consumableSlotContainer, weaponSlotContainer, shieldSlotContainer;

        [Header("Animations")]
        public Vector3 popEffectWhenSwitchingSlots = new Vector3(0.8f, 0.8f, 0.8f);

        VisualElement leftGamepad, alpha1, upGamepad, alpha2, rightGamepad, alpha3, downGamepad, alpha4;
        VisualElement useKeyboard, useGamepad;
        VisualElement equipmentContainer;

        VisualElement KeyboardActions, GamepadActions;

        Label currentObjectiveLabel, currentObjectiveValue, combatStanceIndicatorLabel;

        public StarterAssetsInputs starterAssetsInputs;

        [Header("Localization")]
        public LocalizedString oneHandIndicator_LocalizedString;
        public LocalizedString twoHandIndicator_LocalizedString;

        public LocalizedString dodgeLabel; // Dodge
        public LocalizedString jumpLabel; // Jump
        public LocalizedString toggle1Or2Handing; // Toggle 1/2 Handing
        public LocalizedString heavyAttack; // Heavy Attack
        public LocalizedString sprint; // Sprint


        private void Awake()
        {
            EventManager.StartListening(
                EventMessages.ON_EQUIPMENT_CHANGED,
                UpdateEquipment);

            EventManager.StartListening(
                EventMessages.ON_QUEST_TRACKED,
                UpdateQuestTracking);

            EventManager.StartListening(
                EventMessages.ON_QUESTS_PROGRESS_CHANGED,
                UpdateQuestTracking);

            EventManager.StartListening(EventMessages.ON_USE_CUSTOM_INPUT_CHANGED, UpdateInputsHUD);
        }

        private void OnEnable()
        {
            this.root = this.uIDocument.rootVisualElement;
            healthContainer = root.Q<VisualElement>("Health");
            healthFill = root.Q<VisualElement>("HealthFill");
            staminaContainer = root.Q<VisualElement>("Stamina");
            staminaFill = root.Q<VisualElement>("StaminaFill");
            manaContainer = root.Q<VisualElement>("Mana");
            manaFill = root.Q<VisualElement>("ManaFill");

            quickItemName = root.Q<Label>("QuickItemName");
            arrowsLabel = root.Q<Label>("ArrowsLabel");

            spellSlotContainer = root.Q<IMGUIContainer>("SpellSlot");
            consumableSlotContainer = root.Q<IMGUIContainer>("ConsumableSlot");
            weaponSlotContainer = root.Q<IMGUIContainer>("WeaponSlot");
            shieldSlotContainer = root.Q<IMGUIContainer>("ShieldSlot");

            shieldBlockedIcon = shieldSlotContainer.Q<IMGUIContainer>("Blocked");

            leftGamepad = shieldSlotContainer.Q<VisualElement>("Gamepad");
            alpha1 = shieldSlotContainer.Q<VisualElement>("Keyboard");

            upGamepad = spellSlotContainer.Q<VisualElement>("Gamepad");
            alpha2 = spellSlotContainer.Q<VisualElement>("Keyboard");

            rightGamepad = weaponSlotContainer.Q<VisualElement>("Gamepad");
            alpha3 = weaponSlotContainer.Q<VisualElement>("Keyboard");

            downGamepad = consumableSlotContainer.Q<VisualElement>("Gamepad");
            alpha4 = consumableSlotContainer.Q<VisualElement>("Keyboard");

            useKeyboard = consumableSlotContainer.Q<VisualElement>("UseKeyboard");
            useGamepad = consumableSlotContainer.Q<VisualElement>("UseGamepad");

            equipmentContainer = root.Q<VisualElement>("EquipmentContainer");

            currentObjectiveLabel = root.Q<Label>("CurrentObjectiveLabel");
            currentObjectiveValue = root.Q<Label>("CurrentObjectiveValue");
            currentObjectiveLabel.style.display = DisplayStyle.None;
            currentObjectiveValue.text = "";

            KeyboardActions = root.Q<VisualElement>("KeyboardActions");
            GamepadActions = root.Q<VisualElement>("GamepadActions");

            combatStanceIndicatorLabel = root.Q<Label>("CombatStanceIndicator");
            UpdateCombatStanceIndicator();
            EventManager.StartListening(EventMessages.ON_TWO_HANDING_CHANGED, UpdateCombatStanceIndicator);

            UpdateEquipment();

            UpdateInputsHUD();
            InputSystem.onDeviceChange += HandleDeviceChangeCallback;

            UpdateQuestTracking();
        }

        void UpdateCombatStanceIndicator()
        {
            if (equipmentDatabase.isTwoHanding)
            {
                combatStanceIndicatorLabel.text = twoHandIndicator_LocalizedString.GetLocalizedString();
            }
            else
            {
                combatStanceIndicatorLabel.text = oneHandIndicator_LocalizedString.GetLocalizedString();
            }
        }

        private void OnDisable()
        {
            InputSystem.onDeviceChange -= HandleDeviceChangeCallback;
        }

        void HandleDeviceChangeCallback(InputDevice device, InputDeviceChange change)
        {
            HandleDeviceChange();
        }

        void HandleDeviceChange()
        {
            KeyboardActions.style.display = Gamepad.current == null ? DisplayStyle.Flex : DisplayStyle.None;
            GamepadActions.style.display = Gamepad.current != null ? DisplayStyle.Flex : DisplayStyle.None;
            UpdateEquipment();
        }

        void UpdateInputsHUD()
        {
            if (gameSettings.UseCustomInputs() && !string.IsNullOrEmpty(gameSettings.GetDodgeOverrideBindingPayload()))
            {
                root.Q<Label>("DodgeKeyLabel").text = dodgeLabel.GetLocalizedString() + ": " + starterAssetsInputs.GetCurrentKeyBindingForAction("Dodge");
                root.Q<VisualElement>("DodgeKeyIcon").style.display = DisplayStyle.None;
            }
            else
            {
                root.Q<Label>("DodgeKeyLabel").text = dodgeLabel.GetLocalizedString();
                root.Q<VisualElement>("DodgeKeyIcon").style.display = DisplayStyle.Flex;
            }

            if (gameSettings.UseCustomInputs() && !string.IsNullOrEmpty(gameSettings.GetJumpOverrideBindingPayload()))
            {
                root.Q<Label>("JumpKeyLabel").text = jumpLabel.GetLocalizedString() + ": " + starterAssetsInputs.GetCurrentKeyBindingForAction("Jump");
                root.Q<VisualElement>("JumpKeyIcon").style.display = DisplayStyle.None;
            }
            else
            {
                root.Q<Label>("JumpKeyLabel").text = jumpLabel.GetLocalizedString();
                root.Q<VisualElement>("JumpKeyIcon").style.display = DisplayStyle.Flex;
            }

            if (gameSettings.UseCustomInputs() && !string.IsNullOrEmpty(gameSettings.GetTwoHandModeOverrideBindingPayload()))
            {
                root.Q<Label>("ToggleHandsKeyLabel").text = toggle1Or2Handing.GetLocalizedString() + ": " + starterAssetsInputs.GetCurrentKeyBindingForAction("Tab");
                root.Q<VisualElement>("ToggleHandsKeyIcon").style.display = DisplayStyle.None;
            }
            else
            {
                root.Q<Label>("ToggleHandsKeyLabel").text = toggle1Or2Handing.GetLocalizedString();
                root.Q<VisualElement>("ToggleHandsKeyIcon").style.display = DisplayStyle.Flex;
            }

            if (gameSettings.UseCustomInputs() && !string.IsNullOrEmpty(gameSettings.GetHeavyAttackOverrideBindingPayload()))
            {
                root.Q<Label>("HeavyAttackKeyLabel").text = heavyAttack.GetLocalizedString() + ": " + starterAssetsInputs.GetCurrentKeyBindingForAction("HeavyAttack");
                root.Q<VisualElement>("HeavyAttackKeyIcon").style.display = DisplayStyle.None;
            }
            else
            {
                root.Q<Label>("HeavyAttackKeyLabel").text = heavyAttack.GetLocalizedString();
                root.Q<VisualElement>("HeavyAttackKeyIcon").style.display = DisplayStyle.Flex;
            }

            if (gameSettings.UseCustomInputs() && !string.IsNullOrEmpty(gameSettings.GetSprintOverrideBindingPayload()))
            {
                root.Q<Label>("SprintKeyLabel").text = sprint.GetLocalizedString() + ": " + starterAssetsInputs.GetCurrentKeyBindingForAction("Sprint");
                root.Q<VisualElement>("SprintKeyIcon").style.display = DisplayStyle.None;
            }
            else
            {
                root.Q<Label>("SprintKeyLabel").text = sprint.GetLocalizedString();
                root.Q<VisualElement>("SprintKeyIcon").style.display = DisplayStyle.Flex;
            }


            HandleDeviceChange();
        }

        public void HideHUD()
        {
            root.visible = false;
        }
        public void ShowHUD()
        {
            root.visible = true;
            UpdateInputsHUD();
        }

        private void Update()
        {
            healthContainer.style.width = healthContainerBaseWidth +
                ((playerStatsDatabase.vitality + playerStatsBonusController.vitalityBonus) * _containerMultiplierPerLevel);

            staminaContainer.style.width = staminaContainerBaseWidth + ((
                playerStatsDatabase.endurance + playerStatsBonusController.enduranceBonus) * _containerMultiplierPerLevel);

            manaContainer.style.width = manaContainerBaseWidth + ((
                playerStatsDatabase.intelligence + playerStatsBonusController.intelligenceBonus) * _containerMultiplierPerLevel);

            this.healthFill.style.width = new Length(playerManager.health.GetCurrentHealthPercentage(), LengthUnit.Percent);
            this.staminaFill.style.width = new Length(playerManager.staminaStatManager.GetCurrentStaminaPercentage(), LengthUnit.Percent);
            this.manaFill.style.width = new Length(playerManager.manaManager.GetCurrentManaPercentage(), LengthUnit.Percent);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ShowEquipment()
        {
            equipmentContainer.visible = true;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void HideEquipment()
        {
            equipmentContainer.visible = false;
        }

        public void UpdateEquipment()
        {
            if (!this.isActiveAndEnabled)
            {
                return;
            }

            quickItemName.text = "";
            arrowsLabel.text = "";


            alpha1.style.display = Gamepad.current == null ? DisplayStyle.Flex : DisplayStyle.None;
            alpha2.style.display = Gamepad.current == null ? DisplayStyle.Flex : DisplayStyle.None;
            alpha3.style.display = Gamepad.current == null ? DisplayStyle.Flex : DisplayStyle.None;
            alpha4.style.display = Gamepad.current == null ? DisplayStyle.Flex : DisplayStyle.None;

            leftGamepad.style.display = Gamepad.current != null ? DisplayStyle.Flex : DisplayStyle.None;
            upGamepad.style.display = Gamepad.current != null ? DisplayStyle.Flex : DisplayStyle.None;
            rightGamepad.style.display = Gamepad.current != null ? DisplayStyle.Flex : DisplayStyle.None;
            downGamepad.style.display = Gamepad.current != null ? DisplayStyle.Flex : DisplayStyle.None;

            useGamepad.style.display = equipmentDatabase.GetCurrentConsumable() != null && Gamepad.current != null ? DisplayStyle.Flex : DisplayStyle.None;
            useKeyboard.style.display = equipmentDatabase.GetCurrentConsumable() != null && Gamepad.current == null ? DisplayStyle.Flex : DisplayStyle.None;

            if (equipmentDatabase.IsBowEquipped())
            {
                arrowsLabel.text = equipmentDatabase.GetCurrentArrow() != null
                    ? equipmentDatabase.GetCurrentArrow().GetName() + " (" + inventoryDatabase.GetItemAmount(equipmentDatabase.GetCurrentArrow()) + ")"
                    : "";

                spellSlotContainer.style.backgroundImage = equipmentDatabase.GetCurrentArrow() != null
                    ? new StyleBackground(equipmentDatabase.GetCurrentArrow().sprite)
                    : new StyleBackground(unequippedArrowSlot);
            }
            else
            {
                spellSlotContainer.style.backgroundImage = equipmentDatabase.GetCurrentSpell() != null
                    ? new StyleBackground(equipmentDatabase.GetCurrentSpell().sprite)
                    : new StyleBackground(unequippedSpellSlot);
            }

            shieldSlotContainer.style.backgroundImage = equipmentDatabase.GetCurrentShield() != null
                ? new StyleBackground(equipmentDatabase.GetCurrentShield().sprite)
                : new StyleBackground(unequippedShieldSlot);

            shieldBlockedIcon.style.display = equipmentDatabase.IsBowEquipped() || equipmentDatabase.IsStaffEquipped()
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            weaponSlotContainer.style.backgroundImage = equipmentDatabase.GetCurrentWeapon() != null
                ? new StyleBackground(equipmentDatabase.GetCurrentWeapon().sprite)
                : new StyleBackground(unequippedWeaponSlot);

            quickItemName.text = equipmentDatabase.GetCurrentConsumable() != null ?
                equipmentDatabase.GetCurrentConsumable().GetName() + $" ({inventoryDatabase.GetItemAmount(equipmentDatabase.GetCurrentConsumable())})"
                : "";

            consumableSlotContainer.style.backgroundImage = equipmentDatabase.GetCurrentConsumable() != null
                ? new StyleBackground(equipmentDatabase.GetCurrentConsumable().sprite)
                : new StyleBackground(unequippedConsumableSlot);
        }

        public void OnSwitchWeapon()
        {
            UIUtils.PlayPopAnimation(weaponSlotContainer, popEffectWhenSwitchingSlots);
            UpdateEquipment();
        }
        public void OnSwitchShield()
        {
            UIUtils.PlayPopAnimation(shieldSlotContainer, popEffectWhenSwitchingSlots);
            UpdateEquipment();
        }
        public void OnSwitchConsumable()
        {
            UIUtils.PlayPopAnimation(consumableSlotContainer, popEffectWhenSwitchingSlots);
            UpdateEquipment();
        }
        public void OnSwitchSpell()
        {
            UIUtils.PlayPopAnimation(spellSlotContainer, popEffectWhenSwitchingSlots);
            UpdateEquipment();
        }

        public bool IsEquipmentDisplayed()
        {
            if (!root.visible)
            {
                return false;
            }

            return equipmentContainer.visible;
        }

        void UpdateQuestTracking()
        {
            currentObjectiveLabel.style.display = DisplayStyle.None;
            currentObjectiveValue.text = "";

            string currentQuestObjective = questsDatabase.GetCurrentTrackedQuestObjective();

            if (!string.IsNullOrEmpty(currentQuestObjective))
            {
                currentObjectiveValue.text = currentQuestObjective;
                currentObjectiveLabel.style.display = DisplayStyle.Flex;
            }
        }
    }
}
