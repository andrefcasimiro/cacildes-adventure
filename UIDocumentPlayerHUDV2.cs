using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        IMGUIContainer itemIcon;
        Label itemCounter;

        private void OnEnable()
        {
            healthStatManager = FindObjectOfType<HealthStatManager>(true);
            staminaStatManager = FindObjectOfType<StaminaStatManager>(true);

            this.root = this.uIDocument.rootVisualElement;
            healthContainer = root.Q<VisualElement>("Health");
            healthFill = root.Q<VisualElement>("HealthFill");
            staminaContainer = root.Q<VisualElement>("Stamina");
            staminaFill = root.Q<VisualElement>("StaminaFill");

            itemIcon = root.Q<IMGUIContainer>("ItemIcon");
            itemCounter = root.Q<Label>("ItemCounterLabel");

            UpdateFavoriteItems();
        }

        private void Update()
        {
            healthContainer.style.width = healthContainerBaseWidth + (Player.instance.vitality * containerMultiplierPerLevel);
            staminaContainer.style.width = staminaContainerBaseWidth + (Player.instance.endurance * containerMultiplierPerLevel);

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
            itemIcon.style.backgroundImage = null;
            itemCounter.text = "";
            root.Q<VisualElement>("ItemCounter").style.display = DisplayStyle.None;
            if (Player.instance.favoriteItems.Count <= 0) { return; }

            itemIcon.style.backgroundImage = new StyleBackground(Player.instance.favoriteItems[0].sprite);
            itemCounter.text = Player.instance.ownedItems.Find(x => x.item == Player.instance.favoriteItems[0]).amount.ToString();
            root.Q<VisualElement>("ItemCounter").style.display = DisplayStyle.Flex;
        }

    }

}