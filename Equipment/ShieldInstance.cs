using UnityEngine;

namespace AF.Equipment
{

    public class ShieldInstance : MonoBehaviour
    {
        [Header("Shield")]
        public Shield shield;

        public GameObject shieldInTheBack;
        public bool shouldHide = true;

        private bool isUsingShield = true;

        private void Awake()
        {
            if (!shouldHide)
            {
                return;
            }

            shieldInTheBack.SetActive(false);
        }

        public void ResetStates()
        {
            if (isUsingShield)
            {
                ShowShield();
            }
        }

        public void HideShield()
        {
            if (!shouldHide)
            {
                return;
            }

            if (shieldInTheBack != null)
            {
                shieldInTheBack.SetActive(true);
            }

            gameObject.SetActive(false);
        }

        public void ShowShield()
        {
            gameObject.SetActive(true);

            if (shieldInTheBack != null)
            {
                shieldInTheBack.SetActive(false);
            }
        }

        public void SetIsUsingShield(bool isUsingShield)
        {
            this.isUsingShield = isUsingShield;
        }
    }
}
