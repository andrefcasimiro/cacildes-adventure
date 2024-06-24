using System.Linq;
using AF.Health;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace AF
{
    public enum ArmorSlot
    {
        Head,
        Chest,
        Arms,
        Legs,
    }

    public class ArmorBase : Item
    {
        [System.Serializable]
        public class StatusEffectResistance
        {
            public StatusEffect statusEffect;
            public float resistanceBonus;
        }

        [Header("Stats")]
        public float physicalDefense;

        [Header("Elemental")]
        public float fireDefense;
        public float frostDefense;
        public float lightningDefense;
        public float magicDefense = 0;
        public float darkDefense = 0;

        [Header("Negative Status Resistances")]
        public StatusEffectResistance[] statusEffectResistances;

        [Header("Graphics")]
        public string graphicNameToShow;
        public string[] graphicNamesToHide;

        [Header("Attribute Bonus")]
        public int vitalityBonus = 0;
        public int enduranceBonus = 0;
        public int strengthBonus = 0;
        public int dexterityBonus = 0;
        public int intelligenceBonus = 0;

        [Header("Poise")]
        public int poiseBonus = 0;

        [Header("Posture")]
        public int postureBonus = 0;

        [Header("Stamina")]
        public float staminaRegenBonus = 0f;

        [Header("Speed Penalties")]
        public float speedPenalty = 0;
        public int movementSpeedBonus = 0;

        [Header("Coins")]
        [Range(0, 100f)]
        public float additionalCoinPercentage = 0f;

        [Header("Reputation")]
        public int reputationBonus = 0;

        [Header("Discounts")]
        [Range(0, 1f)] public float discountPercentage = 0f;

        [Header("Damage Type Filters")]

        [Range(0, 1f)] public float pierceDamageAbsorption = 1f;

        [Range(0, 1f)] public float bluntDamageAbsorption = 1f;

        [Range(0, 1f)] public float slashDamageAbsorption = 1f;

        [Header("Damage On Enemies")]
        public bool canDamageEnemiesUponAttack = false;
        public Damage damageDealtToEnemiesUponAttacked;

        [Header("Projectile Options")]
        public float projectileMultiplierBonus = 0f;

        [Header("Rage Mode")]
        public bool canRage = false;

        public string GetFormattedStatusResistances()
        {
            string result = "";

            var resistenceAgainstLabel = LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "resistence against");

            foreach (var resistance in statusEffectResistances)
            {

                if (resistance != null)
                {
                    result += $"+{resistance.resistanceBonus} {resistenceAgainstLabel} {resistance.statusEffect.GetName()}\n";
                }
            }

            return result.TrimEnd();
        }

        public string GetFormattedDamageDealtToEnemiesUpponAttacked()
        {
            string result = "";

            foreach (var resistance in damageDealtToEnemiesUponAttacked.statusEffects)
            {
                if (resistance != null)
                {
                    result += $"+{resistance.amountPerHit} {resistance.statusEffect.name} inflicted on attacking enemies\n";
                }
            }

            return result.TrimEnd();
        }

        public void AttackEnemy(CharacterManager enemy)
        {
            if (!canDamageEnemiesUponAttack)
            {
                return;
            }
            enemy.damageReceiver.TakeDamage(damageDealtToEnemiesUponAttacked);
        }
    }
}
