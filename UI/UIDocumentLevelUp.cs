using AF.Music;
using AF.Stats;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentLevelUp : MonoBehaviour
    {
        [Header("UI Documents")]
        public UIDocument uIDocument;
        public UIDocumentBonfireMenu uIDocumentBonfireMenu;

        VisualElement root;

        [Header("Components")]
        public PlayerManager playerManager;
        public NotificationManager notificationManager;
        public BGMManager bgmManager;
        public Soundbank soundbank;

        [Header("SFX")]
        public AudioClip levelUpSound;

        [Header("Components")]
        public StatsBonusController playerStatsBonusController;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;

        // Internal
        int desiredVitality, desiredEndurance, desiredStrength, desiredDexterity, desiredIntelligence;
        int virtualGold;

        Focusable focusedElement;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnClose()
        {
            if (this.isActiveAndEnabled)
            {
                Close();
            }
        }

        void Close()
        {
            uIDocumentBonfireMenu.gameObject.SetActive(true);
            this.gameObject.SetActive(false);
        }

        void SetupAttributeButtonCallbacks(VisualElement attributeRoot, UnityEngine.Events.UnityAction decreaseAction, UnityEngine.Events.UnityAction increaseAction)
        {
            UIUtils.SetupButton(attributeRoot.Q<Button>("DecreaseBtn"), decreaseAction, soundbank);
            UIUtils.SetupButton(attributeRoot.Q<Button>("IncreaseBtn"), increaseAction, soundbank);
        }

        private void OnEnable()
        {
            virtualGold = playerStatsDatabase.gold;

            desiredVitality = playerStatsDatabase.vitality;
            desiredStrength = playerStatsDatabase.strength;
            desiredDexterity = playerStatsDatabase.dexterity;
            desiredEndurance = playerStatsDatabase.endurance;
            desiredIntelligence = playerStatsDatabase.intelligence;

            root = GetComponent<UIDocument>().rootVisualElement;
            Button buttonExit = root.Q<Button>("ButtonExit");
            UIUtils.SetupButton(buttonExit, () => { Close(); }, soundbank);

            SetupAttributeButtonCallbacks(
                root.Q<VisualElement>("Vitality"),
                () =>
                {
                    desiredVitality--;
                    virtualGold += playerManager.playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount());
                    DrawUI(root);
                },
                () =>
                {
                    desiredVitality++;
                    virtualGold -= playerManager.playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount() - 1);
                    DrawUI(root);
                }
            );

            SetupAttributeButtonCallbacks(
                root.Q<VisualElement>("Endurance"),
                () =>
                {
                    desiredEndurance--;
                    virtualGold += playerManager.playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount());
                    DrawUI(root);
                },
                () =>
                {
                    desiredEndurance++;
                    virtualGold -= playerManager.playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount() - 1);
                    DrawUI(root);
                }
            );

            SetupAttributeButtonCallbacks(
                root.Q<VisualElement>("Intelligence"),
                () =>
                {
                    desiredIntelligence--;
                    virtualGold += playerManager.playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount());
                    DrawUI(root);
                },
                () =>
                {
                    desiredIntelligence++;
                    virtualGold -= playerManager.playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount() - 1);
                    DrawUI(root);
                }
            );

            SetupAttributeButtonCallbacks(
                root.Q<VisualElement>("Strength"),
                () =>
                {
                    desiredStrength--;
                    virtualGold += playerManager.playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount());
                    DrawUI(root);
                },
                () =>
                {
                    desiredStrength++;
                    virtualGold -= playerManager.playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount() - 1);
                    DrawUI(root);
                }
            );

            SetupAttributeButtonCallbacks(
                root.Q<VisualElement>("Dexterity"),
                () =>
                {
                    desiredDexterity--;
                    virtualGold += playerManager.playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount());
                    DrawUI(root);
                },
                () =>
                {
                    desiredDexterity++;
                    virtualGold -= playerManager.playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount() - 1);
                    DrawUI(root);
                }
            );

            UIUtils.SetupButton(root.Q<Button>("ConfirmButton"), () =>
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
                    bgmManager.PlaySound(levelUpSound, null);
                    notificationManager.ShowNotification("Cacildes leveled up!", notificationManager.levelUp);
                }

                DrawUI(root);
            }, soundbank);

            root.Q<Button>("ConfirmButton").SetEnabled(HasEnoughExperienceForLevelling(virtualGold, GetDesiredLevelsAmount() + 1));

            if (focusedElement == null)
            {
                focusedElement = buttonExit;
                focusedElement.Focus();
            }

            DrawUI(root);
        }

        void DrawUI(VisualElement root)
        {
            if (focusedElement != null)
            {
                focusedElement.Focus();
            }
            else
            {
                focusedElement = root.focusController.focusedElement;
            }

            root.Q<VisualElement>("Level").Q<Label>("Value").text = GetDesiredLevelsAmount() + "";
            root.Q<VisualElement>("Gold").Q<Label>("Value").text = virtualGold + "/" + playerManager.playerLevelManager.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount());

            if (HasEnoughExperienceForLevelling(virtualGold, GetDesiredLevelsAmount() + 1))
            {
                root.Q<VisualElement>("Gold").Q<Label>("Value").style.color = Color.green;
            }
            else
            {
                root.Q<VisualElement>("Gold").Q<Label>("Value").style.color = Color.red;
            }

            PlayerHealth playerHealth = playerManager.health as PlayerHealth;

            root.Q<VisualElement>("MaximumHealth").Q<Label>("Value").text = playerHealth.GetHealthPointsForGivenVitality(desiredVitality) + "";
            root.Q<VisualElement>("MaximumStamina").Q<Label>("Value").text = playerManager.staminaStatManager.GetStaminaPointsForGivenEndurance(desiredEndurance) + "";
            root.Q<VisualElement>("MaximumMana").Q<Label>("Value").text = playerManager.manaManager.GetManaPointsForGivenIntelligence(desiredIntelligence) + "";
            root.Q<VisualElement>("PhysicalAttackPower").Q<Label>("Value").text =
                playerManager.attackStatManager.GetCurrentPhysicalAttackForGivenStrengthAndDexterity(desiredStrength, desiredDexterity) + "";
            root.Q<VisualElement>("DefenseAbsorption").Q<Label>("Value").text = playerManager.defenseStatManager.GetCurrentPhysicalDefenseForGivenEndurance(desiredEndurance) + "";

            // Buttons
            root.Q<VisualElement>("Vitality").Q<Button>("DecreaseBtn").SetEnabled(desiredVitality > playerStatsDatabase.vitality);
            root.Q<VisualElement>("Vitality").Q<Button>("IncreaseBtn").SetEnabled(HasEnoughExperienceForLevelling(virtualGold, GetDesiredLevelsAmount() + 1));
            root.Q<VisualElement>("Vitality").Q<Label>("Value").text = desiredVitality + "";

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
            if (experience >= playerManager.playerLevelManager.GetRequiredExperienceForGivenLevel(levelDesired - 1))
            {
                return true;
            }

            return (experience - playerManager.playerLevelManager.GetRequiredExperienceForGivenLevel(levelDesired)) >= 0;
        }

        int GetDesiredLevelsAmount()
        {
            int vitalityLevels = Mathf.Abs(playerStatsDatabase.vitality - desiredVitality);
            int enduranceLevels = Mathf.Abs(playerStatsDatabase.endurance - desiredEndurance);
            int strengthLevels = Mathf.Abs(playerStatsDatabase.strength - desiredStrength);
            int dexterityLevels = Mathf.Abs(playerStatsDatabase.dexterity - desiredDexterity);
            int intelligenceLevels = Mathf.Abs(playerStatsDatabase.intelligence - desiredIntelligence);

            return playerManager.playerLevelManager.GetCurrentLevel() + (
                vitalityLevels + enduranceLevels + strengthLevels + dexterityLevels + intelligenceLevels);
        }
    }

}
