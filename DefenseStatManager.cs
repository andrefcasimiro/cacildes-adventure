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

        public int GetDefenseAbsorption()
        {
            var equipmentBonus = 0f;

            var player = Player.instance;
            
            if (player.equippedHelmet != null)
            {
                equipmentBonus += player.equippedHelmet.physicalDefense;
            }
            if (player.equippedArmor != null)
            {
                equipmentBonus += player.equippedArmor.physicalDefense;
            }
            if (player.equippedGauntlets != null)
            {
                equipmentBonus += player.equippedGauntlets.physicalDefense;
            }
            if (player.equippedLegwear != null)
            {
                equipmentBonus += player.equippedLegwear.physicalDefense;
            }

            return (int)(
                GetCurrentPhysicalDefense()
                + equipmentBonus
                + physicalDefenseBonus
            );
        }

        public int GetCurrentPhysicalDefense()
        {
            return (int)(this.basePhysicalDefense * Player.instance.endurance * levelMultiplier);
        }

        public int GetCurrentPhysicalDefenseForGivenEndurance(int endurance)
        {
            return (int)(this.basePhysicalDefense * endurance * levelMultiplier);
        }

        public float GetMaximumStatusResistanceBeforeSufferingStatusEffect(StatusEffect statusEffect)
        {
            var target = this.negativeStatusResistances.Find(x => x.statusEffect == statusEffect);

            if (target == null)
            {
                // Fallback for unknown status
                return 100f;
            }

            return target.resistance + (Player.instance.endurance * levelMultiplier);
        }

    }
}
