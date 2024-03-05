using UnityEngine;
namespace AF
{

    public class CharacterWeaponBuffs : MonoBehaviour
    {


        [Header("Enchantments and Buffs")]
        public bool hasBuff = false;
        public float buffDuration = 60f;
        public int fireBuffBonus = 50;
        public GameObject fireBuffContainer;

        private void Awake()
        {
            if (fireBuffContainer != null)
            {
                fireBuffContainer.gameObject.SetActive(false);
            }
        }

        public void ApplyFireBuff()
        {
            if (!CanUseBuff())
            {
                return;
            }

            hasBuff = true;

            if (fireBuffContainer != null)
            {
                fireBuffContainer.gameObject.SetActive(true);
            }

            Invoke(nameof(DisableBuff), buffDuration);
        }

        void DisableBuff()
        {
            Debug.Log("has finished");
            if (fireBuffContainer != null)
            {
                fireBuffContainer.gameObject.SetActive(false);
            }

            hasBuff = false;
        }

        bool CanUseBuff()
        {
            if (hasBuff)
            {
                return false;
            }

            return true;
        }
    }
}
