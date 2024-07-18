using UnityEngine;

namespace AF.Health
{

    public class CharacterHealthUI : MonoBehaviour
    {
        public UnityEngine.UI.Slider healthSlider;
        public CharacterHealth characterHealth;

        private void Start()
        {
            InitializeHUD();

            if (characterHealth != null)
            {
                characterHealth.onHealthSettingsChanged.AddListener(UpdateUIDueToSettings);
            }
        }

        public void InitializeHUD()
        {
            if (characterHealth.characterManager.characterBossController.IsBoss())
            {
                healthSlider.gameObject.SetActive(false);
                return;
            }

            if (healthSlider != null)
            {
                healthSlider.maxValue = characterHealth.GetMaxHealth() * 0.01f;
                healthSlider.value = characterHealth.GetCurrentHealth() * 0.01f;
                healthSlider.gameObject.SetActive(false);
            }
        }

        public void UpdateUI()
        {
            if (characterHealth.characterManager.characterBossController.IsBoss())
            {
                healthSlider.gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(characterHealth.GetCurrentHealth() > 0);

            healthSlider.value = characterHealth.GetCurrentHealth() * 0.01f;
            healthSlider.maxValue = characterHealth.GetMaxHealth() * 0.01f;
        }

        void UpdateUIDueToSettings()
        {
            healthSlider.value = characterHealth.GetCurrentHealth() * 0.01f;
            healthSlider.maxValue = characterHealth.GetMaxHealth() * 0.01f;
        }
    }
}
