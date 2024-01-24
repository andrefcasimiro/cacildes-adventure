using System.Collections;
using AF.Stats;
using UnityEngine;

namespace AF
{
    public class ManaManager : MonoBehaviour
    {
        public int baseMana = 100;
        public float levelMultiplier = 3.25f;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        [Header("Components")]
        public StatsBonusController playerStatsBonusController;

        public StarterAssetsInputs inputs;

        public EquipmentGraphicsHandler equipmentGraphicsHandler;

        public PlayerManager playerManager;

        private void Start()
        {
            // Initialize Mana
            if (playerStatsDatabase.currentMana == -1)
            {
                playerStatsDatabase.currentMana = GetMaxMana();
            }
        }

        public int GetMaxMana()
        {
            return baseMana + Mathf.RoundToInt((
                playerStatsDatabase.intelligence + playerStatsBonusController.intelligenceBonus) * levelMultiplier);
        }

        public void DecreaseMana(float amount)
        {
            playerStatsDatabase.currentMana = Mathf.Clamp(playerStatsDatabase.currentMana - amount, 0, GetMaxMana());
        }

        public bool HasEnoughManaForSpell(Spell spell)
        {
            if (spell == null)
            {
                return false;
            }

            return playerStatsDatabase.currentMana - spell.costPerCast > 0;
        }

        public void RestoreFullMana()
        {
            playerStatsDatabase.currentMana = GetMaxMana();
        }

        public void RestoreManaPercentage(float amount)
        {
            var percentage = this.GetMaxMana() * amount / 100;
            var nextValue = Mathf.Clamp(playerStatsDatabase.currentMana + percentage, 0, this.GetMaxMana());

            playerStatsDatabase.currentMana = nextValue;
        }

        public void RestoreManaPoints(float amount)
        {
            var nextValue = Mathf.Clamp(playerStatsDatabase.currentMana + amount, 0, this.GetMaxMana());

            playerStatsDatabase.currentMana = nextValue;
        }

        public float GetManaPointsForGivenIntelligence(int intelligence)
        {
            return baseMana + (int)Mathf.Ceil(intelligence * levelMultiplier);
        }


        public float GetCurrentManaPercentage()
        {
            return playerStatsDatabase.currentMana * 100 / GetMaxMana();
        }
    }
}
