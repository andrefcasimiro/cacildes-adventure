using System.Linq;
using AF.Health;
using UnityEngine;

namespace AF
{

    [System.Serializable]
    public class StatusEffectBlockResistance
    {
        public StatusEffect statusEffect;
        [Range(0, 1f)] public float absorption = 1f;
    }

    [CreateAssetMenu(menuName = "Items / Shield / New Shield")]
    public class Shield : Item
    {
        [Header("Stamina Costs")]
        public float blockStaminaCost = 30f;

        // Defense Absorption
        [Range(0, 1f)] public float physicalAbsorption = 1f;
        [Range(0, 1f)] public float fireAbsorption = 1f;
        [Range(0, 1f)] public float frostAbsorption = 1f;
        [Range(0, 1f)] public float lightiningAbsorption = 1f;
        [Range(0, 1f)] public float magicAbsorption = 1f;
        [Range(0, 1f)] public float darknessAbsorption = 1f;


        [Header("Status Effect Resistances")]
        public StatusEffectBlockResistance[] statusEffectBlockResistances;

        [Header("Stats Bonuses")]
        public int vitalityBonus = 0;
        public int enduranceBonus = 0;
        public int intelligenceBonus = 0;

        [Header("Regen Options")]
        public float staminaRegenBonus = 1f;

        [Header("Additional Stats")]
        public int postureBonus = 0;
        public int poiseBonus = 0;

        public float speedPenalty = 0f;

        [Header("Damage Enemies On Block")]
        public bool canDamageEnemiesOnShieldAttack = false;
        public Damage damageDealtToEnemiesUponBlocking;

        [Header("Parry Bonus")]
        public float parryWindowBonus = 0f;
        public int parryPostureDamageBonus = 0;

        public Damage FilterDamage(Damage incomingDamage)
        {
            if (physicalAbsorption != 1)
            {
                incomingDamage.physical = (int)(incomingDamage.physical * physicalAbsorption);
            }
            if (fireAbsorption != 1)
            {
                incomingDamage.fire = (int)(incomingDamage.fire * fireAbsorption);
            }
            if (frostAbsorption != 1)
            {
                incomingDamage.frost = (int)(incomingDamage.frost * frostAbsorption);
            }
            if (lightiningAbsorption != 1)
            {
                incomingDamage.lightning = (int)(incomingDamage.lightning * lightiningAbsorption);
            }
            if (magicAbsorption != 1)
            {
                incomingDamage.magic = (int)(incomingDamage.magic * magicAbsorption);
            }

            return incomingDamage;
        }

        public Damage FilterPassiveDamage(Damage incomingDamage)
        {
            if (statusEffectBlockResistances != null && statusEffectBlockResistances.Length > 0 && incomingDamage.statusEffects != null && incomingDamage.statusEffects.Length > 0)
            {
                foreach (var statusEffectBlockResistance in statusEffectBlockResistances)
                {
                    int idx = System.Array.FindIndex(incomingDamage.statusEffects, x => x.statusEffect == statusEffectBlockResistance.statusEffect);
                    if (idx != -1)
                    {
                        incomingDamage.statusEffects[idx].amountPerHit *= statusEffectBlockResistance.absorption;
                    }
                }
            }

            return incomingDamage;
        }

        public string GetFormattedStatusResistances()
        {
            string result = "";

            foreach (var resistance in statusEffectBlockResistances)
            {
                if (resistance != null)
                {
                    result += $"%{100 - (resistance.absorption * 100)} {resistance.statusEffect.name} Absorption\n";
                }
            }

            return result.TrimEnd();
        }

        public string GetFormattedStatusAttacks()
        {
            string result = "";

            foreach (var resistance in damageDealtToEnemiesUponBlocking.statusEffects)
            {
                if (resistance != null)
                {
                    result += $"+{resistance.amountPerHit} {resistance.statusEffect.name} inflicted on enemy attacking shield\n";
                }
            }

            return result.TrimEnd();
        }

        public void AttackShieldAttacker(CharacterManager enemy)
        {
            if (!canDamageEnemiesOnShieldAttack)
            {
                return;
            }
            enemy.damageReceiver.TakeDamage(damageDealtToEnemiesUponBlocking);
        }

    }

}
