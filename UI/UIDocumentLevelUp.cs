using System.Collections;
using System.Collections.Generic;
using AF.Stats;
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
        int desiredIntelligence;

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
        public LocalizedText intelligenceText;
        public LocalizedText intelligenceExplanationText;
        public LocalizedText confirmText;

        VisualElement root;

        [Header("Components")]
        public StatsBonusController playerStatsBonusController;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;

        void TranslateRoot()
        {
            root.Q<Button>("ButtonExit").text = backToBonfireText.GetText();
            root.Q<Label>("Title").text = levelUpText.GetText();

            var currentLevelLabel = root.Q<Label>("CurrentLevel");
            currentLevelLabel.text = currentLevelText.GetText();

            var currentGoldAndRequiredLabel = root.Q<Label>("CurrentGoldAndRequired");
            currentGoldAndRequiredLabel.text = currentAndRequiredGoldText.GetText();

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
            root.Q<Label>("IntelligenceText").text = intelligenceText.GetText();
            root.Q<Label>("IntelligenceExplanationText").text = intelligenceExplanationText.GetText();
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

        void Close()
        {
            uIDocumentBonfireMenu.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            GameObject player = GameObject.FindWithTag("Player");
            healthStatManager = player.GetComponent<HealthStatManager>();
            staminaStatManager = player.GetComponent<StaminaStatManager>();
            attackStatManager = player.GetComponent<AttackStatManager>();
            defenseStatManager = player.GetComponent<DefenseStatManager>();
            playerLevelManager = player.GetComponent<PlayerLevelManager>();

            virtualGold = playerStatsDatabase.gold;

            desiredVitality = playerStatsDatabase.vitality;
            desiredStrength = playerStatsDatabase.strength;
            desiredDexterity = playerStatsDatabase.dexterity;
            desiredEndurance = playerStatsDatabase.endurance;
            desiredIntelligence = playerStatsDatabase.intelligence;

            root = GetComponent<UIDocument>().rootVisualElement;
            TranslateRoot();

            root.Q<Button>("ButtonExit").RegisterCallback<ClickEvent>(ev =>
            {
                Close();
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


            root.Q<VisualElement>("Intelligence").Q<Button>("DecreaseBtn").RegisterCallback<ClickEvent>(ev =>
            {
                desiredIntelligence--;
                virtualGold += playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount());
                DrawUI(root);
            });

            root.Q<VisualElement>("Intelligence").Q<Button>("IncreaseBtn").RegisterCallback<ClickEvent>(ev =>
            {
                desiredIntelligence++;
                virtualGold -= playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount() - 1);
                DrawUI(root);
            });

            root.Q<Button>("ConfirmButton").RegisterCallback<ClickEvent>(ev =>
            {
                var oldLevel = playerStatsDatabase.GetCurrentLevel();
                playerStatsDatabase.vitality = desiredVitality;
                playerStatsDatabase.endurance = desiredEndurance;
                playerStatsDatabase.strength = desiredStrength;
                playerStatsDatabase.dexterity = desiredDexterity;
                playerStatsDatabase.intelligence = desiredIntelligence;

                playerStatsDatabase.gold = virtualGold;

                var newLevel = playerStatsDatabase.GetCurrentLevel();

                if (oldLevel != newLevel)
                {
                    BGMManager.instance.PlaySound(levelUpSound, null);
                    notificationManager.ShowNotification(LocalizedTerms.CacildesLeveledUp(), notificationManager.levelUp);
                }

                DrawUI(root);
            });

            root.Q<Button>("ConfirmButton").SetEnabled(HasEnoughExperienceForLevelling(virtualGold, GetDesiredLevelsAmount() + 1));

            DrawUI(root);

            FindObjectOfType<GamepadCursor>(true).gameObject.SetActive(true);
        }

        void DrawUI(VisualElement root)
        {

            root.Q<VisualElement>("Level").Q<Label>("Value").text = GetDesiredLevelsAmount() + "";
            root.Q<VisualElement>("Gold").Q<Label>("Value").text = virtualGold + "/" + playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount());


            if (HasEnoughExperienceForLevelling(virtualGold, GetDesiredLevelsAmount() + 1))
            {
                root.Q<VisualElement>("Gold").Q<Label>("Value").style.color = Color.green;
            }
            else
            {
                root.Q<VisualElement>("Gold").Q<Label>("Value").style.color = Color.red;
            }

            root.Q<VisualElement>("MaximumHealth").Q<Label>("Value").text = healthStatManager.GetHealthPointsForGivenVitality(desiredVitality) + "";
            root.Q<VisualElement>("MaximumStamina").Q<Label>("Value").text = staminaStatManager.GetStaminaPointsForGivenEndurance(desiredEndurance) + "";
            root.Q<VisualElement>("PhysicalAttackPower").Q<Label>("Value").text = attackStatManager.GetCurrentPhysicalAttackForGivenStrengthAndDexterity(desiredStrength, desiredDexterity) + "";
            root.Q<VisualElement>("DefenseAbsorption").Q<Label>("Value").text = defenseStatManager.GetCurrentPhysicalDefenseForGivenEndurance(desiredEndurance) + "";

            // Buttons
            root.Q<VisualElement>("Vigor").Q<Button>("DecreaseBtn").SetEnabled(desiredVitality > playerStatsDatabase.vitality);
            root.Q<VisualElement>("Vigor").Q<Button>("IncreaseBtn").SetEnabled(HasEnoughExperienceForLevelling(virtualGold, GetDesiredLevelsAmount() + 1));
            root.Q<VisualElement>("Vigor").Q<Label>("Value").text = desiredVitality + "";

            root.Q<VisualElement>("Endurance").Q<Button>("DecreaseBtn").SetEnabled(desiredEndurance > playerStatsDatabase.endurance);
            root.Q<VisualElement>("Endurance").Q<Button>("IncreaseBtn").SetEnabled(HasEnoughExperienceForLevelling(virtualGold, GetDesiredLevelsAmount() + 1));
            root.Q<VisualElement>("Endurance").Q<Label>("Value").text = desiredEndurance + "";

            root.Q<VisualElement>("Strength").Q<Button>("DecreaseBtn").SetEnabled(desiredStrength > playerStatsDatabase.strength);
            root.Q<VisualElement>("Strength").Q<Button>("IncreaseBtn").SetEnabled(HasEnoughExperienceForLevelling(virtualGold, GetDesiredLevelsAmount() + 1));
            root.Q<VisualElement>("Strength").Q<Label>("Value").text = desiredStrength + "";

            root.Q<VisualElement>("Dexterity").Q<Button>("DecreaseBtn").SetEnabled(desiredDexterity > playerStatsDatabase.dexterity);
            root.Q<VisualElement>("Dexterity").Q<Button>("IncreaseBtn").SetEnabled(HasEnoughExperienceForLevelling(virtualGold, GetDesiredLevelsAmount() + 1));
            root.Q<VisualElement>("Dexterity").Q<Label>("Value").text = desiredDexterity + "";

            root.Q<VisualElement>("Intelligence").Q<Button>("DecreaseBtn").SetEnabled(desiredIntelligence > playerStatsDatabase.intelligence);
            root.Q<VisualElement>("Intelligence").Q<Button>("IncreaseBtn").SetEnabled(HasEnoughExperienceForLevelling(virtualGold, GetDesiredLevelsAmount() + 1));
            root.Q<VisualElement>("Intelligence").Q<Label>("Value").text = desiredIntelligence + "";

        }

        public bool HasEnoughExperienceForLevelling(float experience, int levelDesired)
        {
            if (experience >= playerLevelManager.GetRequiredExperienceForGivenLevel(levelDesired - 1))
            {
                return true;
            }

            return (experience - playerLevelManager.GetRequiredExperienceForGivenLevel(levelDesired)) >= 0;
        }

        int GetDesiredLevelsAmount()
        {
            int vitalityLevels = Mathf.Abs(playerStatsDatabase.vitality - desiredVitality);
            int enduranceLevels = Mathf.Abs(playerStatsDatabase.endurance - desiredEndurance);
            int strengthLevels = Mathf.Abs(playerStatsDatabase.strength - desiredStrength);
            int dexterityLevels = Mathf.Abs(playerStatsDatabase.dexterity - desiredDexterity);
            int intelligenceLevels = Mathf.Abs(playerStatsDatabase.intelligence - desiredIntelligence);

            return playerLevelManager.GetCurrentLevel() + (
                vitalityLevels + enduranceLevels + strengthLevels + dexterityLevels + intelligenceLevels);
        }
    }

}
