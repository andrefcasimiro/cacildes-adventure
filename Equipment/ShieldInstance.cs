using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF.Equipment
{

    public class ShieldInstance : MonoBehaviour
    {
        [Header("Shield")]
        public Shield shield;

        public GameObject shieldInTheBack;
        public bool shouldHide = true;

        [Header("Equipment Database")]
        public EquipmentDatabase equipmentDatabase;

        private void Awake()
        {
            if (!shouldHide)
            {
                return;
            }

            EventManager.StartListening(EventMessages.ON_TWO_HANDING_CHANGED, OnTwoHandingChanged);
        }

        private void OnEnable()
        {
            ResetStates();
        }

        public void ResetStates()
        {
            if (equipmentDatabase != null && equipmentDatabase.isTwoHanding == false)
            {
                ShowShield();
            }
            else
            {
                HideShield();
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

        public void OnTwoHandingChanged()
        {
            if (equipmentDatabase.GetCurrentShield() != shield)
            {
                return;
            }

            if (!equipmentDatabase.isTwoHanding)
            {
                ShowShield();
            }
            else
            {
                HideShield();
            }
        }
    }
}
