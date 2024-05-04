using System.Collections;
using AF.Stats;
using UnityEngine;

namespace AF
{
    public class StaminaStatManager : MonoBehaviour
    {

        [Header("Regeneration Settings")]
        public float STAMINA_REGENERATION_RATE = 20f;
        public float STAMINA_REGENERATION_RATE_BONUS = 0f;
        public float negativeStaminaRegenerationBonus = 0f;
        public const float EMPTY_STAMINA_REGENERATION_DELAY = 0.5f;
        public bool shouldRegenerateStamina = false;

        [Header("Combat Stamina")]
        public int unarmedLightAttackStaminaCost = 15;
        public int unarmedHeavyAttackStaminaCost = 35;

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
            if (playerStatsDatabase.currentStamina == -1)
            {
                playerStatsDatabase.currentStamina = GetMaxStamina();
            }
            else if (playerStatsDatabase.currentStamina < GetMaxStamina())
            {
                shouldRegenerateStamina = true;
            }
        }

        public int GetMaxStamina()
        {
            return playerStatsDatabase.maxStamina + playerStatsBonusController.staminaBonus + Mathf.RoundToInt((
                playerStatsDatabase.endurance + playerStatsBonusController.enduranceBonus) * playerStatsDatabase.levelMultiplierForStamina);
        }

        public bool CanPerformAction(int amountDrained)
        {
            return playerStatsDatabase.currentStamina - amountDrained > 0;
        }

        public float GetCurrentStaminaPercentage()
        {
            return playerStatsDatabase.currentStamina * 100 / GetMaxStamina();
        }

        public void DecreaseStamina(float amount)
        {
            shouldRegenerateStamina = false;

            playerStatsDatabase.currentStamina = Mathf.Clamp(playerStatsDatabase.currentStamina - amount, 0, GetMaxStamina());

            StartCoroutine(RegenerateEmptyStamina());
        }

        IEnumerator RegenerateEmptyStamina()
        {
            yield return new WaitForSeconds(EMPTY_STAMINA_REGENERATION_DELAY);

            shouldRegenerateStamina = true;
        }

        private void Update()
        {
            if (shouldRegenerateStamina)
            {
                if (inputs.sprint)
                {
                    return;
                }

                HandleStaminaRegen();
            }
        }

        void HandleStaminaRegen()
        {
            var finalRegenerationRate = STAMINA_REGENERATION_RATE + playerStatsBonusController.staminaRegenerationBonus - negativeStaminaRegenerationBonus + STAMINA_REGENERATION_RATE_BONUS;

            if (playerManager.characterBlockController.isBlocking)
            {
                finalRegenerationRate = finalRegenerationRate / 4;
            }

            playerStatsDatabase.currentStamina = Mathf.Clamp(playerStatsDatabase.currentStamina + finalRegenerationRate * Time.deltaTime, 0f, GetMaxStamina());

            if (playerStatsDatabase.currentStamina >= GetMaxStamina())
            {
                shouldRegenerateStamina = false;
            }
        }

        public bool HasEnoughStaminaForAction(float actionStaminaCost)
        {
            return playerStatsDatabase.currentStamina - actionStaminaCost > 0;
        }

        public void RestoreStaminaPercentage(float amount)
        {
            var percentage = this.GetMaxStamina() * amount / 100;
            var nextValue = Mathf.Clamp(playerStatsDatabase.currentStamina + percentage, 0, this.GetMaxStamina());

            playerStatsDatabase.currentStamina = nextValue;
        }

        public void RestoreStaminaPoints(float amount)
        {
            var nextValue = Mathf.Clamp(playerStatsDatabase.currentStamina + amount, 0, this.GetMaxStamina());

            playerStatsDatabase.currentStamina = nextValue;
        }


        public float GetStaminaPointsForGivenEndurance(int endurance)
        {
            return playerStatsDatabase.maxStamina + (int)Mathf.Ceil(endurance * playerStatsDatabase.levelMultiplierForStamina);
        }

        public void DecreaseLightAttackStamina()
        {
            DecreaseStamina(
                equipmentDatabase.GetCurrentWeapon() != null
                    ? equipmentDatabase.GetCurrentWeapon().lightAttackStaminaCost
                    : unarmedLightAttackStaminaCost);
        }
        public void DecreaseHeavyAttackStamina()
        {
            DecreaseStamina(
                equipmentDatabase.GetCurrentWeapon() != null
                    ? equipmentDatabase.GetCurrentWeapon().heavyAttackStaminaCost
                    : unarmedHeavyAttackStaminaCost);
        }

        public bool HasEnoughStaminaForLightAttack()
        {
            var staminaCost = equipmentDatabase.GetCurrentWeapon() != null
                ? equipmentDatabase.GetCurrentWeapon().lightAttackStaminaCost : unarmedLightAttackStaminaCost;

            return HasEnoughStaminaForAction(staminaCost);
        }

        public bool HasEnoughStaminaForHeavyAttack()
        {
            var staminaCost = equipmentDatabase.GetCurrentWeapon() != null
                ? equipmentDatabase.GetCurrentWeapon().heavyAttackStaminaCost : unarmedHeavyAttackStaminaCost;

            return HasEnoughStaminaForAction(staminaCost);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void SetNegativeStaminaRegenerationBonus(int value)
        {
            negativeStaminaRegenerationBonus = value;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void ResetNegativeStaminaRegenerationBonus()
        {
            negativeStaminaRegenerationBonus = 0f;
        }

        public void SetStaminaRegenerationBonus(float value)
        {
            this.STAMINA_REGENERATION_RATE_BONUS = value;
        }
    }
}
