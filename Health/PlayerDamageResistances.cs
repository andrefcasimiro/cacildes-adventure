using AF.Health;
using UnityEngine;

namespace AF
{
    public class PlayerDamageResistances : DamageResistances
    {
        public PlayerManager playerManager;

        public int damageReductionFactor = 2;

        public override Damage FilterIncomingDamage(Damage incomingDamage)
        {
            Damage filteredDamage = base.FilterIncomingDamage(incomingDamage);

            float physicalDefense = playerManager.defenseStatManager.GetDefenseAbsorption();

            // Improved formula with adjustment for high defense
            float damageReductionPercentage = 1 - Mathf.Pow(1f / (1f + physicalDefense / 100f), damageReductionFactor);

            filteredDamage.physical = (int)(filteredDamage.physical * (1f - damageReductionPercentage));

            if (playerManager.defenseStatManager.physicalDefenseAbsorption > 0)
            {
                filteredDamage.physical -= (int)(filteredDamage.physical * playerManager.defenseStatManager.physicalDefenseAbsorption / 100);
            }

            return filteredDamage;
        }
    }
}
