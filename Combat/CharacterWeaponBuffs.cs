using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;
namespace AF
{

    public class CharacterWeaponBuffs : MonoBehaviour
    {

        [System.Serializable]
        public class WeaponBuff
        {
            public GameObject container;
            public int damageBonus;
            public StatusEffect statusEffect;
            public int statusEffectAmountToApply;
        }

        public enum WeaponBuffName
        {
            NONE,
            PHYSICAL,
            FIRE,
            FROST,
            MAGIC,
            LIGHTNING,
            DARKNESS,
            POISON,
            BLOOD,
            SHARPNESS,
        }


        [Header("Enchantments and Buffs")]
        public WeaponBuffName appliedBuff = WeaponBuffName.NONE;
        public float buffDuration = 60f;
        public SerializedDictionary<WeaponBuffName, WeaponBuff> weaponBuffs = new();

        private void Awake()
        {
            DisableAllBuffContainers();
        }

        void DisableAllBuffContainers()
        {
            foreach (var weaponBuff in weaponBuffs)
            {
                if (weaponBuff.Value != null && weaponBuff.Value.container != null)
                {
                    weaponBuff.Value.container.SetActive(false);
                }
            }
        }


        public void ApplyBuff(WeaponBuffName weaponBuffName, float customDuration)
        {
            HandleApplyBuff(weaponBuffName, customDuration);
        }

        public void ApplyBuff(WeaponBuffName weaponBuffName)
        {
            HandleApplyBuff(weaponBuffName, this.buffDuration);
        }

        void HandleApplyBuff(WeaponBuffName weaponBuffName, float duration)
        {
            if (!CanUseBuff(weaponBuffName))
            {
                return;
            }

            appliedBuff = weaponBuffName;

            weaponBuffs[weaponBuffName].container.SetActive(true);

            Invoke(nameof(DisableBuff), duration);
        }

        void DisableBuff()
        {
            DisableAllBuffContainers();

            appliedBuff = WeaponBuffName.NONE;
        }

        bool CanUseBuff(WeaponBuffName weaponBuffName)
        {
            if (HasOnGoingBuff())
            {
                return false;
            }

            if (!weaponBuffs.ContainsKey(weaponBuffName))
            {
                return false;
            }

            return true;
        }

        public bool HasOnGoingBuff()
        {
            return appliedBuff != WeaponBuffName.NONE;
        }

        public int GetCurrentBuffDamage()
        {
            if (!HasOnGoingBuff())
            {
                return 0;
            }

            return weaponBuffs[appliedBuff].damageBonus;
        }
    }
}
