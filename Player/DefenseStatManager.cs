using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    [System.Serializable]
    public class NegativeStatusResistance {
        public StatusEffect statusEffect;

        public float resistance = 100f;

        public float decreaseRateWithoutDamage = 5;

        public float decreaseRateWithDamage = 1;
    }

    public class DefenseStatManager : MonoBehaviour
    {
        [Header("Physical Defense")]
        public int basePhysicalDefense = 100;
        [Tooltip("Increases with endurance level")]
        public float levelMultiplier = 3.25f;

        [Header("Status defense bonus")]
        [Tooltip("Increased by buffs like potions, or equipment like accessories")]
        public float physicalDefenseBonus = 0f;

        [Header("Negative Status Resistances")]
        public List<NegativeStatusResistance> negativeStatusResistances = new List<NegativeStatusResistance>();

        EquipmentGraphicsHandler equipmentGraphicsHandler => GetComponent<EquipmentGraphicsHandler>();

        public float GetDefenseAbsorption()
        {
            return (int)(
                GetCurrentPhysicalDefense()
                + equipmentGraphicsHandler.equipmentPhysicalDefense // Equipment Bonus
                + physicalDefenseBonus
                + (equipmentGraphicsHandler.enduranceBonus * levelMultiplier)
            );
        }

        public int GetCurrentPhysicalDefense()
        {
            return (int)(this.basePhysicalDefense + Player.instance.endurance * levelMultiplier);
        }

        public int GetCurrentPhysicalDefenseForGivenEndurance(int endurance)
        {
            return (int)(this.basePhysicalDefense + (endurance * levelMultiplier));
        }

        public float GetMaximumStatusResistanceBeforeSufferingStatusEffect(StatusEffect statusEffect)
        {
            var target = this.negativeStatusResistances.Find(x => x.statusEffect == statusEffect);

            if (target == null)
            {
                // Fallback for unknown status
                return 100f;
            }

            var bonusFromEquipment = 0f;

            var idx = equipmentGraphicsHandler.statusEffectResistances.FindIndex(x => x.statusEffect == statusEffect);
            if (idx != -1)
            {
                bonusFromEquipment += equipmentGraphicsHandler.statusEffectResistances[idx].resistanceBonus;
            }

            return target.resistance + bonusFromEquipment + (Player.instance.endurance * levelMultiplier);
        }

        public float GetMagicDefense()
        {
            return equipmentGraphicsHandler.magicDefenseBonus;
        }

    }
}
