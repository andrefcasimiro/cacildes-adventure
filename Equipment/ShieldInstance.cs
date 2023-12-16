using UnityEngine;

namespace AF.Equipment
{

    public class ShieldInstance : MonoBehaviour
    {
        [Header("Shield")]
        public Shield shield;

        public GameObject shieldInTheBack;
        public bool shouldHide = true;

        private void Awake()
        {
            if (!shouldHide)
            {
                return;
            }

            shieldInTheBack.SetActive(false);
        }

        public void HideShield()
        {
            if (!shouldHide)
            {
                return;
            }

            gameObject.SetActive(false);

            if (shieldInTheBack != null)
            {
                shieldInTheBack.SetActive(true);
            }
        }

        public void ShowShield()
        {
            gameObject.SetActive(true);

            if (shieldInTheBack != null)
            {
                shieldInTheBack.SetActive(false);
            }
        }
    }
}
