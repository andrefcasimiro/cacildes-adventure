using System.Collections;
using AF.Events;
using AF.Stats;
using TigerForge;
using UnityEngine;

namespace AF
{
    public class ManaManager : MonoBehaviour
    {

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        [Header("Components")]
        public StatsBonusController playerStatsBonusController;

        public StarterAssetsInputs inputs;

        public EquipmentGraphicsHandler equipmentGraphicsHandler;

        public PlayerManager playerManager;

        [Header("Regeneration Settings")]
        public float MANA_REGENERATION_RATE = 20f;

        private void Start()
        {
            // Initialize Mana
            if (playerStatsDatabase.currentMana == -1)
            {
                playerStatsDatabase.currentMana = GetMaxMana();
            }
        }

        private void Update()
        {
            if (playerStatsBonusController.shouldRegenerateMana && playerStatsDatabase.currentMana < playerStatsDatabase.maxMana)
            {
                HandleManaRegen();
            }
        }

        void HandleManaRegen()
        {
            var finalRegenerationRate = MANA_REGENERATION_RATE + playerStatsBonusController.staminaRegenerationBonus;

            playerStatsDatabase.currentMana = Mathf.Clamp(playerStatsDatabase.currentMana + finalRegenerationRate * Time.deltaTime, 0f, GetMaxMana());
        }

        public int GetMaxMana()
        {
            return playerStatsDatabase.maxMana + playerStatsBonusController.magicBonus + Mathf.RoundToInt((
                playerStatsDatabase.intelligence + playerStatsBonusController.intelligenceBonus) * playerStatsDatabase.levelMultiplierForMana);
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
            return playerStatsDatabase.maxMana + (int)Mathf.Ceil(intelligence * playerStatsDatabase.levelMultiplierForMana);
        }


        public float GetCurrentManaPercentage()
        {
            return playerStatsDatabase.currentMana * 100 / GetMaxMana();
        }
    }
}
