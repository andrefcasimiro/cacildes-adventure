using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace AF
{
    public class UIDocumentLevelUpScreenUI : UIDocumentBase
    {
        [Header("Attributes")]
        public Button decreaseVitalityBtn;
        public Button increaseVitalityBtn;
        public Label currentVitality;
        int desiredVitality;

        public Button decreaseEnduranceBtn;
        public Button increaseEnduranceBtn;
        public Label currentEndurance;
        int desiredEndurance;

        public Button decreaseIntelligenceBtn;
        public Button increaseIntelligenceBtn;
        public Label currentIntelligence;
        int desiredIntelligence;

        public Button decreaseStrengthBtn;
        public Button increaseStrengthBtn;
        public Label currentStrength;
        int desiredStrength;

        public Button decreaseDexterityBtn;
        public Button increaseDexterityBtn;
        public Label currentDexterity;
        int desiredDexterity;

        public Button decreaseArcaneBtn;
        public Button increaseArcaneBtn;
        public Label currentArcane;
        int desiredArcane;

        public Button decreaseCharismaBtn;
        public Button increaseCharismaBtn;
        public Label currentCharisma;
        int desiredCharisma;

        public Button confirmButton;
        public Button cancelButton;

        [Header("Character Stats Labels")]
        public Label level;
        public Label currentExperience;
        public Label hp;
        public Label mp;
        public Label stamina;
        public Label physicalAttack;
        public Label physicalDefense;

        bool hasFinished = false;

        public float virtualCurrentExperience;

        protected override void Start()
        {
            base.Start();

            this.increaseVitalityBtn = root.Q<VisualElement>("Vitality").Q<Button>("btnIncrease");
            this.decreaseVitalityBtn = root.Q<VisualElement>("Vitality").Q<Button>("btnDecrease");
            SetupButtonClick(this.increaseVitalityBtn, () =>
            {

                desiredVitality++;

                virtualCurrentExperience -= PlayerStatsManager.instance.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount() - 1);

                this.RefreshUI();
            });
            SetupButtonClick(this.decreaseVitalityBtn, () =>
            {
                desiredVitality--;

                virtualCurrentExperience += PlayerStatsManager.instance.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount());

                this.RefreshUI();
            });
            this.currentVitality = root.Q<VisualElement>("Vitality").Q<Label>("Value");

            this.increaseEnduranceBtn = root.Q<VisualElement>("Endurance").Q<Button>("btnIncrease");
            this.decreaseEnduranceBtn = root.Q<VisualElement>("Endurance").Q<Button>("btnDecrease");
            SetupButtonClick(this.increaseEnduranceBtn, () =>
            {
                desiredEndurance++;

                virtualCurrentExperience -= PlayerStatsManager.instance.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount() -1);
                this.RefreshUI();
            });
            SetupButtonClick(this.decreaseEnduranceBtn, () =>
            {
                desiredEndurance--;

                virtualCurrentExperience += PlayerStatsManager.instance.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount());
                this.RefreshUI();
            });
            this.currentEndurance = root.Q<VisualElement>("Endurance").Q<Label>("Value");

            this.increaseIntelligenceBtn = root.Q<VisualElement>("Intelligence").Q<Button>("btnIncrease");
            this.decreaseIntelligenceBtn = root.Q<VisualElement>("Intelligence").Q<Button>("btnDecrease");
            SetupButtonClick(this.increaseIntelligenceBtn, () =>
            {
                desiredIntelligence++;

                virtualCurrentExperience -= PlayerStatsManager.instance.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount() - 1);
                this.RefreshUI();
            });
            SetupButtonClick(this.decreaseIntelligenceBtn, () =>
            {
                desiredIntelligence--;

                virtualCurrentExperience += PlayerStatsManager.instance.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount());
                this.RefreshUI();
            });
            this.currentIntelligence = root.Q<VisualElement>("Intelligence").Q<Label>("Value");

            this.increaseStrengthBtn = root.Q<VisualElement>("Strength").Q<Button>("btnIncrease");
            this.decreaseStrengthBtn = root.Q<VisualElement>("Strength").Q<Button>("btnDecrease");
            SetupButtonClick(this.increaseStrengthBtn, () =>
            {
                desiredStrength++;

                virtualCurrentExperience -= PlayerStatsManager.instance.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount() - 1);
                this.RefreshUI();
            });
            SetupButtonClick(this.decreaseStrengthBtn, () =>
            {
                desiredStrength--;

                virtualCurrentExperience += PlayerStatsManager.instance.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount());
                this.RefreshUI();
            });
            this.currentStrength = root.Q<VisualElement>("Strength").Q<Label>("Value");

            this.increaseDexterityBtn = root.Q<VisualElement>("Dexterity").Q<Button>("btnIncrease");
            this.decreaseDexterityBtn = root.Q<VisualElement>("Dexterity").Q<Button>("btnDecrease");
            SetupButtonClick(this.increaseDexterityBtn, () =>
            {
                desiredDexterity++;

                virtualCurrentExperience -= PlayerStatsManager.instance.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount() -1);
                this.RefreshUI();
            });
            SetupButtonClick(this.decreaseDexterityBtn, () =>
            {
                desiredDexterity--;

                virtualCurrentExperience += PlayerStatsManager.instance.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount());
                this.RefreshUI();
            });
            this.currentDexterity = root.Q<VisualElement>("Dexterity").Q<Label>("Value");

            this.increaseArcaneBtn = root.Q<VisualElement>("Arcane").Q<Button>("btnIncrease");
            this.decreaseArcaneBtn = root.Q<VisualElement>("Arcane").Q<Button>("btnDecrease");
            SetupButtonClick(this.increaseArcaneBtn, () =>
            {
                desiredArcane++;

                virtualCurrentExperience -= PlayerStatsManager.instance.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount() -1);
                this.RefreshUI();
            });
            SetupButtonClick(this.decreaseArcaneBtn, () =>
            {
                desiredArcane--;

                virtualCurrentExperience += PlayerStatsManager.instance.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount());
                this.RefreshUI();
            });
            this.currentArcane = root.Q<VisualElement>("Arcane").Q<Label>("Value");

            this.increaseCharismaBtn = root.Q<VisualElement>("Charisma").Q<Button>("btnIncrease");
            this.decreaseCharismaBtn = root.Q<VisualElement>("Charisma").Q<Button>("btnDecrease");
            SetupButtonClick(this.increaseCharismaBtn, () =>
            {
                desiredCharisma++;

                virtualCurrentExperience -= PlayerStatsManager.instance.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount() - 1);
                this.RefreshUI();
            });
            SetupButtonClick(this.decreaseCharismaBtn, () =>
            {
                desiredCharisma--;

                virtualCurrentExperience += PlayerStatsManager.instance.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount());
                this.RefreshUI();
            }); 
            this.currentCharisma = root.Q<VisualElement>("Charisma").Q<Label>("Value");

            this.confirmButton = root.Q<Button>("ConfirmButton");
            SetupButtonClick(confirmButton, () =>
            {
                PlayerStatsManager.instance.vitality = desiredVitality;
                PlayerStatsManager.instance.intelligence = desiredIntelligence;
                PlayerStatsManager.instance.endurance = desiredEndurance;
                PlayerStatsManager.instance.strength = desiredStrength;
                PlayerStatsManager.instance.dexterity = desiredDexterity;
                PlayerStatsManager.instance.arcane = desiredArcane;
                PlayerStatsManager.instance.charisma = desiredCharisma;

                PlayerStatsManager.instance.currentExperience = virtualCurrentExperience;

                hasFinished = true;
            });
            this.cancelButton = root.Q<Button>("CancelButton");
            SetupButtonClick(cancelButton, () =>
            {
                hasFinished = true;
            });

            this.level = root.Q<VisualElement>("Level").Q<Label>("Value");
            this.currentExperience = root.Q<VisualElement>("CurrentExperience").Q<Label>("Value");
            this.hp = root.Q<VisualElement>("CurrentHP").Q<Label>("Value");
            this.mp = root.Q<VisualElement>("CurrentMP").Q<Label>("Value");
            this.stamina = root.Q<VisualElement>("CurrentStamina").Q<Label>("Value");
            this.physicalAttack = root.Q<VisualElement>("AttackPower").Q<Label>("Value");
            this.physicalDefense = root.Q<VisualElement>("DefenseAbsorption").Q<Label>("Value");

            base.Disable();
        }

        public IEnumerator OpenLevelUpScreen()
        {
            hasFinished = false;
            virtualCurrentExperience = PlayerStatsManager.instance.currentExperience;

            desiredVitality = PlayerStatsManager.instance.vitality;
            desiredEndurance = PlayerStatsManager.instance.endurance;
            desiredIntelligence = PlayerStatsManager.instance.intelligence;
            desiredStrength = PlayerStatsManager.instance.strength;
            desiredDexterity = PlayerStatsManager.instance.dexterity;
            desiredArcane = PlayerStatsManager.instance.arcane;
            desiredCharisma = PlayerStatsManager.instance.charisma;

            Enable();

            yield return new WaitUntil(() => hasFinished == true);

            Disable();
        }

        private void ShowStats()
        {
            this.level.text = GetDesiredLevelsAmount().ToString();

            this.currentExperience.text = virtualCurrentExperience + "/" + PlayerStatsManager.instance.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount());
            
            if (virtualCurrentExperience < PlayerStatsManager.instance.GetRequiredExperienceForGivenLevel(GetDesiredLevelsAmount()))
            {
                this.currentExperience.style.color = Color.red;
            }
            else
            {
                this.currentExperience.style.color = Color.white;
            }

            this.hp.text = PlayerStatsManager.instance.GetHealthPointsForGivenVitality(desiredVitality).ToString() + " Points";

            this.mp.text = PlayerStatsManager.instance.GetMagicPointsForGivenIntelligence(desiredIntelligence).ToString() + " Points";
            this.stamina.text = PlayerStatsManager.instance.GetStaminaPointsForGivenEndurance(desiredEndurance).ToString() + " Points";

            this.physicalAttack.text = PlayerStatsManager.instance.GetPhysicalAttackForDesiredStrengthAndDexterity(desiredStrength, desiredDexterity).ToString();
            this.physicalDefense.text = PlayerStatsManager.instance.GetPhysicalDefenseForGivenEndurance(desiredEndurance).ToString();

            this.currentVitality.text = desiredVitality.ToString();
            this.decreaseVitalityBtn.SetEnabled(desiredVitality > PlayerStatsManager.instance.vitality);
            this.increaseVitalityBtn.SetEnabled(PlayerStatsManager.instance.HasEnoughExperienceForLevelling(virtualCurrentExperience, GetDesiredLevelsAmount() + 1));

            this.currentIntelligence.text = desiredIntelligence.ToString();
            this.decreaseIntelligenceBtn.SetEnabled(desiredIntelligence > PlayerStatsManager.instance.intelligence);
            this.increaseIntelligenceBtn.SetEnabled(PlayerStatsManager.instance.HasEnoughExperienceForLevelling(virtualCurrentExperience, GetDesiredLevelsAmount() + 1));

            this.currentEndurance.text = desiredEndurance.ToString();
            this.decreaseEnduranceBtn.SetEnabled(desiredEndurance > PlayerStatsManager.instance.endurance);
            this.increaseEnduranceBtn.SetEnabled(PlayerStatsManager.instance.HasEnoughExperienceForLevelling(virtualCurrentExperience, GetDesiredLevelsAmount() + 1));

            this.currentStrength.text = desiredStrength.ToString();
            this.decreaseStrengthBtn.SetEnabled(desiredStrength > PlayerStatsManager.instance.strength);
            this.increaseStrengthBtn.SetEnabled(PlayerStatsManager.instance.HasEnoughExperienceForLevelling(virtualCurrentExperience, GetDesiredLevelsAmount() + 1));

            this.currentDexterity.text = desiredDexterity.ToString();
            this.decreaseDexterityBtn.SetEnabled(desiredDexterity > PlayerStatsManager.instance.dexterity);
            this.increaseDexterityBtn.SetEnabled(PlayerStatsManager.instance.HasEnoughExperienceForLevelling(virtualCurrentExperience, GetDesiredLevelsAmount() + 1));

            this.currentArcane.text = desiredArcane.ToString();
            this.decreaseArcaneBtn.SetEnabled(desiredArcane > PlayerStatsManager.instance.charisma);
            this.increaseArcaneBtn.SetEnabled(PlayerStatsManager.instance.HasEnoughExperienceForLevelling(virtualCurrentExperience, GetDesiredLevelsAmount() + 1));

            this.currentCharisma.text = desiredCharisma.ToString();
            this.decreaseCharismaBtn.SetEnabled(desiredCharisma > PlayerStatsManager.instance.charisma);
            this.increaseCharismaBtn.SetEnabled(PlayerStatsManager.instance.HasEnoughExperienceForLevelling(virtualCurrentExperience, GetDesiredLevelsAmount() + 1));
        }

        int GetDesiredLevelsAmount()
        {
            int vitalityLevels = Mathf.Abs(PlayerStatsManager.instance.vitality - desiredVitality);
            int enduranceLevels = Mathf.Abs(PlayerStatsManager.instance.endurance - desiredEndurance);
            int intelligenceLevels = Mathf.Abs(PlayerStatsManager.instance.intelligence - desiredIntelligence);
            int strengthLevels = Mathf.Abs(PlayerStatsManager.instance.strength - desiredStrength);
            int dexterityLevels = Mathf.Abs(PlayerStatsManager.instance.dexterity - desiredDexterity);
            int arcaneLevels = Mathf.Abs(PlayerStatsManager.instance.arcane - desiredArcane);
            int charismaLevels = Mathf.Abs(PlayerStatsManager.instance.charisma - desiredCharisma);

            return PlayerStatsManager.instance.GetCurrentLevel() + (
                vitalityLevels + enduranceLevels + intelligenceLevels + strengthLevels + dexterityLevels + arcaneLevels + charismaLevels);
        }

        public void RefreshUI()
        {

            ShowStats();

        }

        public override void Enable()
        {
            base.Enable();

            RefreshUI();

            this.confirmButton.Focus();
        }

        public override void Disable()
        {
            base.Disable();
        }
    }
}
