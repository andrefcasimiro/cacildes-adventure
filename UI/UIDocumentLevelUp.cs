using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentLevelUp : MonoBehaviour
    {
        public UIDocumentBonfireMenu uIDocumentBonfireMenu;

        HealthStatManager healthStatManager;
        StaminaStatManager staminaStatManager;
        AttackStatManager attackStatManager;
        DefenseStatManager defenseStatManager;
        PlayerLevelManager playerLevelManager;

        int desiredVitality;
        int desiredEndurance;
        int desiredStrength;
        int desiredDexterity;

        int virtualGold;

        public AudioClip levelUpSound;

        NotificationManager notificationManager;

        [Header("Localization")]
        public LocalizedText backToBonfireText;
        public LocalizedText levelUpText;
        public LocalizedText currentLevelText;
        public LocalizedText currentAndRequiredGoldText;
        public LocalizedText maximumHealthText;
        public LocalizedText maximumStaminaText;
        public LocalizedText physicalAttackPowerText;
        public LocalizedText physicalDefenseAbsorption;
        public LocalizedText vigorText;
        public LocalizedText vigorExplanationText;
        public LocalizedText enduranceText;
        public LocalizedText enduranceExplanationText;
        public LocalizedText strengthText;
        public LocalizedText strengthExplanationText;
        public LocalizedText dexterityText;
        public LocalizedText dexterityExplanationText;
        public LocalizedText confirmText;

        VisualElement root;

        void TranslateRoot()
        {
            root.Q<Button>("ButtonExit").text = backToBonfireText.GetText();
            root.Q<Label>("Title").text = levelUpText.GetText();
            root.Q<Label>("CurrentLevel").text = currentLevelText.GetText();
            root.Q<Label>("CurrentGoldAndRequired").text = currentAndRequiredGoldText.GetText();
            root.Q<Label>("MaximumHealthText").text = maximumHealthText.GetText();
            root.Q<Label>("MaximumStaminaText").text = maximumStaminaText.GetText();
            root.Q<Label>("PhysicalAttackPowerLabel").text = physicalAttackPowerText.GetText();
            root.Q<Label>("PhysicalDefenseAbsorptionLabel").text = physicalDefenseAbsorption.GetText();
            root.Q<Label>("VigorText").text = vigorText.GetText();
            root.Q<Label>("VigorDescriptionText").text = vigorExplanationText.GetText();
            root.Q<Label>("EnduranceText").text = enduranceText.GetText();
            root.Q<Label>("EnduranceDescriptionText").text = enduranceExplanationText.GetText();
            root.Q<Label>("StrengthText").text = strengthText.GetText();
            root.Q<Label>("StrengthExplanationText").text = strengthExplanationText.GetText();
            root.Q<Label>("DexterityText").text = dexterityText.GetText();
            root.Q<Label>("DexterityExplanationText").text = dexterityExplanationText.GetText();
            root.Q<Button>("ConfirmButton").text = confirmText.GetText();
        }


        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void Start()
        {
            notificationManager = FindObjectOfType<NotificationManager>(true);
        }

        private void OnEnable()
        {
            GameObject player = GameObject.FindWithTag("Player");
            healthStatManager = player.GetComponent<HealthStatManager>();
            staminaStatManager = player.GetComponent<StaminaStatManager>();
            attackStatManager = player.GetComponent<AttackStatManager>();
            defenseStatManager = player.GetComponent<DefenseStatManager>();
            playerLevelManager = player.GetComponent<PlayerLevelManager>();

            virtualGold = Player.instance.currentGold;

            desiredVitality = Player.instance.vitality;
            desiredStrength = Player.instance.strength;
            desiredDexterity = Player.instance.dexterity;
            desiredEndurance = Player.instance.endurance;

            root = GetComponent<UIDocument>().rootVisualElement;
            TranslateRoot();

            root.Q<Button>("ButtonExit").RegisterCallback<ClickEvent>(ev =>
            {
                uIDocumentBonfireMenu.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            });

            // Add callbacks

            root.Q<VisualElement>("Vigor").Q<Button>("DecreaseBtn").RegisterCallback<ClickEvent>(ev =>
            {
                desiredVitality--;
                virtualGold += playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount());
                DrawUI(root);
            });

            root.Q<VisualElement>("Vigor").Q<Button>("IncreaseBtn").RegisterCallback<ClickEvent>(ev =>
            {
                desiredVitality++;
                virtualGold -= playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount() - 1);
                DrawUI(root);
            });

            root.Q<VisualElement>("Endurance").Q<Button>("DecreaseBtn").RegisterCallback<ClickEvent>(ev =>
            {
                desiredEndurance--;
                virtualGold += playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount());
                DrawUI(root);
            });
            root.Q<VisualElement>("Endurance").Q<Button>("IncreaseBtn").RegisterCallback<ClickEvent>(ev =>
            {
                desiredEndurance++;
                virtualGold -= playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount() - 1);
                DrawUI(root);
            });

            root.Q<VisualElement>("Strength").Q<Button>("DecreaseBtn").RegisterCallback<ClickEvent>(ev =>
            {
                desiredStrength--;
                virtualGold += playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount());
                DrawUI(root);
            });

            root.Q<VisualElement>("Strength").Q<Button>("IncreaseBtn").RegisterCallback<ClickEvent>(ev =>
            {
                desiredStrength++;
                virtualGold -= playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount() - 1);
                DrawUI(root);
            });

            root.Q<VisualElement>("Dexterity").Q<Button>("DecreaseBtn").RegisterCallback<ClickEvent>(ev =>
            {
                desiredDexterity--;
                virtualGold += playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount());
                DrawUI(root);
            });

            root.Q<VisualElement>("Dexterity").Q<Button>("IncreaseBtn").RegisterCallback<ClickEvent>(ev =>
            {
                desiredDexterity++;
                virtualGold -= playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount() - 1);
                DrawUI(root);
            });

            root.Q<Button>("ConfirmButton").RegisterCallback<ClickEvent>(ev =>
            {
                var oldLevel = playerLevelManager.GetCurrentLevel();
                Player.instance.vitality = desiredVitality;
                Player.instance.endurance = desiredEndurance;
                Player.instance.strength = desiredStrength;
                Player.instance.dexterity = desiredDexterity;

                Player.instance.currentGold = virtualGold;

                var newLevel = playerLevelManager.GetCurrentLevel();

                if (oldLevel != newLevel)
                {
                    BGMManager.instance.PlaySound(levelUpSound, null);
                    notificationManager.ShowNotification(LocalizedTerms.CacildesLeveledUp(), notificationManager.levelUp);
                }

                DrawUI(root);
            });

            DrawUI(root);

            FindObjectOfType<GamepadCursor>(true).gameObject.SetActive(true);
        }

        void DrawUI(VisualElement root)
        {
            root.Q<VisualElement>("Level").Q<Label>("Value").text = GetDesiredLevelsAmount() + "";
            root.Q<VisualElement>("Gold").Q<Label>("Value").text = virtualGold + "/" + playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount());

            root.Q<VisualElement>("MaximumHealth").Q<Label>("Value").text = healthStatManager.GetHealthPointsForGivenVitality(desiredVitality) + "";
            root.Q<VisualElement>("MaximumStamina").Q<Label>("Value").text = staminaStatManager.GetStaminaPointsForGivenEndurance(desiredEndurance) + "";
            root.Q<VisualElement>("PhysicalAttackPower").Q<Label>("Value").text = attackStatManager.GetCurrentPhysicalAttackForGivenStrengthAndDexterity(desiredStrength, desiredDexterity) + "";
            root.Q<VisualElement>("DefenseAbsorption").Q<Label>("Value").text = defenseStatManager.GetCurrentPhysicalDefenseForGivenEndurance(desiredEndurance) + "";

            // Buttons
            root.Q<VisualElement>("Vigor").Q<Button>("DecreaseBtn").SetEnabled(desiredVitality > Player.instance.vitality);
            root.Q<VisualElement>("Vigor").Q<Button>("IncreaseBtn").SetEnabled(HasEnoughExperienceForLevelling(virtualGold, GetDesiredLevelsAmount() + 1));
            root.Q<VisualElement>("Vigor").Q<Label>("Value").text = desiredVitality + "";

            root.Q<VisualElement>("Endurance").Q<Button>("DecreaseBtn").SetEnabled(desiredEndurance > Player.instance.endurance);
            root.Q<VisualElement>("Endurance").Q<Button>("IncreaseBtn").SetEnabled(HasEnoughExperienceForLevelling(virtualGold, GetDesiredLevelsAmount() + 1));
            root.Q<VisualElement>("Endurance").Q<Label>("Value").text = desiredEndurance + "";

            root.Q<VisualElement>("Strength").Q<Button>("DecreaseBtn").SetEnabled(desiredStrength > Player.instance.strength);
            root.Q<VisualElement>("Strength").Q<Button>("IncreaseBtn").SetEnabled(HasEnoughExperienceForLevelling(virtualGold, GetDesiredLevelsAmount() + 1));
            root.Q<VisualElement>("Strength").Q<Label>("Value").text = desiredStrength + "";

            root.Q<VisualElement>("Dexterity").Q<Button>("DecreaseBtn").SetEnabled(desiredDexterity > Player.instance.dexterity);
            root.Q<VisualElement>("Dexterity").Q<Button>("IncreaseBtn").SetEnabled(HasEnoughExperienceForLevelling(virtualGold, GetDesiredLevelsAmount() + 1));
            root.Q<VisualElement>("Dexterity").Q<Label>("Value").text = desiredDexterity + "";
        }

        public bool HasEnoughExperienceForLevelling(float experience, int levelDesired)
        {
            if (experience >= playerLevelManager.GetRequiredExperienceForGivenLevel(levelDesired - 1))
            {
                return true;
            }

            return (experience - playerLevelManager.GetRequiredExperienceForGivenLevel(levelDesired)) >= 0;
        }

        private void Update()
        {
            // UnityEngine.Cursor.lockState = CursorLockMode.None;
        }

        int GetDesiredLevelsAmount()
        {
            int vitalityLevels = Mathf.Abs(Player.instance.vitality - desiredVitality);
            int enduranceLevels = Mathf.Abs(Player.instance.endurance - desiredEndurance);
            int strengthLevels = Mathf.Abs(Player.instance.strength - desiredStrength);
            int dexterityLevels = Mathf.Abs(Player.instance.dexterity - desiredDexterity);

            return playerLevelManager.GetCurrentLevel() + (
                vitalityLevels + enduranceLevels + strengthLevels + dexterityLevels);
        }
    }

}
