using UnityEngine;
using UnityEngine.InputSystem;
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

        [Header("Graphic Settings")]
        public float healthContainerBaseWidth = 185;
        public float staminaContainerBaseWidth = 150;
        public float containerMultiplierPerLevel = 1.5f;

        HealthStatManager healthStatManager;
        StaminaStatManager staminaStatManager;
        EquipmentGraphicsHandler equipmentGraphicsHandler;

        IMGUIContainer itemIcon;
        Label quickItemName;
        Label consumeActionLabel;
        Label switchItemActionLabel;
        Label aimActionLabel;
        VisualElement consumeGamepadIcon;
        VisualElement switchItemGamepadIcon;
        VisualElement aimGamepadIcon;

        private void OnEnable()
        {
            healthStatManager = FindObjectOfType<HealthStatManager>(true);
            staminaStatManager = FindObjectOfType<StaminaStatManager>(true);
            equipmentGraphicsHandler = FindObjectOfType<EquipmentGraphicsHandler>(true);

            this.root = this.uIDocument.rootVisualElement;
            healthContainer = root.Q<VisualElement>("Health");
            healthFill = root.Q<VisualElement>("HealthFill");
            staminaContainer = root.Q<VisualElement>("Stamina");
            staminaFill = root.Q<VisualElement>("StaminaFill");

            itemIcon = root.Q<IMGUIContainer>("ItemIcon");
            quickItemName = root.Q<Label>("QuickItemName");

            consumeActionLabel = root.Q<Label>("ConsumeAction");
            switchItemActionLabel = root.Q<Label>("SwitchAction");
            aimActionLabel = root.Q<Label>("AimAction");

            consumeGamepadIcon = root.Q<VisualElement>("GamepadTriangle");
            switchItemGamepadIcon = root.Q<VisualElement>("GamepadSwitchItem");
            aimGamepadIcon = root.Q<VisualElement>("GamepadAim");

            aimActionLabel.text = "";
            aimGamepadIcon.style.display = DisplayStyle.None;

            UpdateFavoriteItems();
        }

        public void HideHUD()
        {
            root.style.opacity = 0;
        }
        public void ShowHUD()
        {
            root.style.opacity = 1;
        }

        private void Update()
        {
            healthContainer.style.width = healthContainerBaseWidth + ((Player.instance.vitality + equipmentGraphicsHandler.vitalityBonus) * containerMultiplierPerLevel);
            staminaContainer.style.width = staminaContainerBaseWidth + ((Player.instance.endurance + equipmentGraphicsHandler.enduranceBonus) * containerMultiplierPerLevel);

            float healthPercentage = Mathf.Clamp(
                (Player.instance.currentHealth * 100) / healthStatManager.GetMaxHealth(),
                0,
                100
            );

            Length formattedHp = new Length(healthPercentage, LengthUnit.Percent);
            this.healthFill.style.width = formattedHp;


            float staminaPercentage = Mathf.Clamp(
                (Player.instance.currentStamina * 100) / staminaStatManager.GetMaxStamina(),
                0,
                100
            );

            Length formattedStamina = new Length(staminaPercentage, LengthUnit.Percent);
            this.staminaFill.style.width = formattedStamina;
        }

        public void UpdateFavoriteItems()
        {
            if (!this.isActiveAndEnabled)
            {
                return;
            }

            itemIcon.style.backgroundImage = null;

            quickItemName.text = "";


            consumeGamepadIcon.style.display = DisplayStyle.None;
            switchItemGamepadIcon.style.display = DisplayStyle.None;

            consumeActionLabel.text = "";
            switchItemActionLabel.text = "";

            if (Player.instance == null || Player.instance.favoriteItems == null || Player.instance.favoriteItems.Count <= 0) { return; }

            itemIcon.style.backgroundImage = new StyleBackground(Player.instance.favoriteItems[0].sprite);

            quickItemName.text = Player.instance.favoriteItems[0].name.GetText();

            var favItem = Player.instance.ownedItems.Find(x => x.item.name.GetEnglishText() == Player.instance.favoriteItems[0].name.GetEnglishText());

            if (favItem != null)
            {
                if (favItem.item is Consumable c && c.notStackable == false)
                {
                    quickItemName.text += " (" + favItem.amount.ToString() + ")";
                }
                else if (favItem.item is Spell s)
                {
                    var spellName = favItem.item.name.GetEnglishText();

                    quickItemName.text += " (" + (favItem.amount).ToString() + ")";
                }
            }

            var isEnglish = GamePreferences.instance.IsEnglish();

            if (Gamepad.current != null)
            {
                consumeGamepadIcon.style.display = DisplayStyle.Flex;
                switchItemGamepadIcon.style.display = DisplayStyle.Flex;

                /*if (favItem.item is ConsumableProjectile)
                {
                    switchItemGamepadIcon.style.display = DisplayStyle.Flex;
                }*/
            }
            else
            {
                consumeActionLabel.text = "[R] ";
                switchItemActionLabel.text = isEnglish ? "[Scrollwheel] " : "[Scrollwheel] ";

                /*if (favItem.item is ConsumableProjectile)
                {
                    aimActionLabel.text = "[TAB] ";
                }*/
            }   


            consumeActionLabel.text += isEnglish ? "Use" : "Usar";
            switchItemActionLabel.text += isEnglish ? "Switch" : "Trocar";

            /*
            if (favItem.item is ConsumableProjectile)
            {
                switchItemActionLabel.text += isEnglish ? "Aim" : "Mirar";
            }*/

        }

    }

}
