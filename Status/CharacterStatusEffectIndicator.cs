using AF.StatusEffects;
using UnityEngine;
using UnityEngine.UI;

namespace AF
{

    public class CharacterStatusEffectIndicator : MonoBehaviour
    {
        public Image background;
        public Image fill;

        private void OnEnable()
        {
            background.gameObject.SetActive(true);
            fill.fillAmount = 0;
        }

        public void UpdateUI(AppliedStatusEffect statusEffect, float currentMaximumResistanceToStatusEffect)
        {
            background.enabled = !statusEffect.hasReachedTotalAmount;

            fill.fillAmount = currentMaximumResistanceToStatusEffect;
        }
    }

}
