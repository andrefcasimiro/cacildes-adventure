using UnityEngine;

namespace AF.Equipment
{

    public class ShieldInstance : MonoBehaviour
    {
        [Header("Shield")]
        public Shield shield;

        public GameObject shieldInTheBack;

        private void Awake()
        {
            shieldInTheBack.SetActive(false);
        }

        public void HideShield()
        {
            gameObject.SetActive(false);
            shieldInTheBack.SetActive(true);
        }

        public void ShowShield()
        {
            gameObject.SetActive(true);
            shieldInTheBack.SetActive(false);
        }
    }
}
