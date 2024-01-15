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
        }

        public void InitializeHUD()
        {
            if (healthSlider != null)
            {
                healthSlider.maxValue = characterHealth.GetMaxHealth() * 0.01f;
                healthSlider.value = characterHealth.GetCurrentHealth() * 0.01f;
                healthSlider.gameObject.SetActive(false);
            }
        }

        public void UpdateUI()
        {
            gameObject.SetActive(characterHealth.GetCurrentHealth() > 0);

            healthSlider.value = characterHealth.GetCurrentHealth() * 0.01f;
            healthSlider.maxValue = characterHealth.GetMaxHealth() * 0.01f;
        }

    }

}
