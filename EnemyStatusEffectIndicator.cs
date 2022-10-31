using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AF
{

    public class EnemyStatusEffectIndicator : MonoBehaviour
    {
        public Image background;
        public Image fill;

        private void OnEnable()
        {
            background.gameObject.SetActive(true);
            fill.fillAmount = 0;
        }

        public void UpdateUI(float fillAmount, bool hasReachedTotalAmount)
        {
            background.enabled = (!hasReachedTotalAmount);

            fill.fillAmount = fillAmount;
        }


    }

}