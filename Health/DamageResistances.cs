using System;
using AF.Health;
using UnityEngine;

namespace AF
{
    [Serializable]
    public class DamageResistances : MonoBehaviour
    {
        [Serializable]
        public class WeaponTypeResistance
        {
            public WeaponAttackType weaponAttackType;
            [Range(0.1f, 1f)] public float damageResistance = 1;
            [Range(1f, 2.5f)] public float damageBonus = 1;
        }

        [Header("Resistant To Weapons")]
        public WeaponTypeResistance[] weaponTypeResistances;

        [Header("Elemental Damages")]
        [Range(0.1f, 1f)] public float fireDamageFilter = 1;
        [Range(1, 5f)] public float fireDamageBonus = 1;

        [Range(0.1f, 1f)] public float frostDamageFilter = 1;
        [Range(1, 5f)] public float frostDamageBonus = 1;

        [Range(0.1f, 1f)] public float magicDamageFilter = 1;
        [Range(1, 5f)] public float magicDamageBonus = 1;

        [Range(0.1f, 1f)] public float lightningDamageFilter = 1;
        [Range(1, 5f)] public float lightningDamageBonus = 1;

        [Range(0.1f, 1f)] public float darknessDamageFilter = 1;
        [Range(1, 5f)] public float darknessDamageBonus = 1;

        public virtual Damage FilterIncomingDamage(Damage incomingDamage)
        {
            Damage filteredDamage = new()
            {
                physical = incomingDamage.physical,
                fire = incomingDamage.fire,
                frost = incomingDamage.frost,
                magic = incomingDamage.magic,
                lightning = incomingDamage.lightning,
                darkness = incomingDamage.darkness,
                postureDamage = incomingDamage.postureDamage,
                poiseDamage = incomingDamage.poiseDamage,
                pushForce = incomingDamage.pushForce,
                weaponAttackType = incomingDamage.weaponAttackType,
                statusEffects = incomingDamage.statusEffects,
                damageType = incomingDamage.damageType
            };

            filteredDamage.physical = (int)(filteredDamage.physical
                * GetDamageMultiplier(incomingDamage.weaponAttackType, weaponTypeResistances, r => r.damageResistance)
                * GetDamageMultiplier(incomingDamage.weaponAttackType, weaponTypeResistances, r => r.damageBonus));
            filteredDamage.fire = ApplyElementalDamageBonus(filteredDamage.fire, fireDamageBonus, fireDamageFilter);
            filteredDamage.frost = ApplyElementalDamageBonus(filteredDamage.frost, frostDamageBonus, frostDamageFilter);
            filteredDamage.magic = ApplyElementalDamageBonus(filteredDamage.magic, magicDamageBonus, magicDamageFilter);
            filteredDamage.lightning = ApplyElementalDamageBonus(filteredDamage.lightning, lightningDamageBonus, lightningDamageFilter);
            filteredDamage.darkness = ApplyElementalDamageBonus(filteredDamage.darkness, darknessDamageBonus, darknessDamageFilter);

            return filteredDamage;
        }

        float GetDamageMultiplier(WeaponAttackType attackType, WeaponTypeResistance[] typeResistances, Func<WeaponTypeResistance, float> selector)
        {
            if (typeResistances == null)
            {
                return 1f;
            }

            var match = Array.Find(typeResistances, r => r.weaponAttackType == attackType);
            if (match == null)
            {
                return 1f;
            }

            return selector(match);
        }

        int ApplyElementalDamageBonus(int damage, float bonus, float filter)
        {
            return (int)(damage * bonus * filter);
        }
    }
}
