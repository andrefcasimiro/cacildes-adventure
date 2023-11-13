using System.Collections;
using AF.Stats;
using UnityEngine;

namespace AF
{
    public class StaminaStatManager : MonoBehaviour
    {
        public int baseStamina = 100;
        public float levelMultiplier = 3.25f;

        [Header("Regeneration Settings")]
        public float STAMINA_REGENERATION_RATE = 20f;
        public float staminaRegenerationBonus = 0f;
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

        public PlayerParryManager playerParryManager;

        private void Start()
        {
            playerStatsDatabase.currentStamina = GetMaxStamina();
        }

        public int GetMaxStamina()
        {
            return baseStamina + (int)(Mathf.RoundToInt((
                playerStatsDatabase.endurance + playerStatsBonusController.enduranceBonus) * levelMultiplier));
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
            var finalRegenerationRate = STAMINA_REGENERATION_RATE + staminaRegenerationBonus - negativeStaminaRegenerationBonus;

            if (playerParryManager.IsBlocking())
            {
                finalRegenerationRate = finalRegenerationRate / 4;
            }

            playerStatsDatabase.currentStamina += Mathf.Clamp(finalRegenerationRate * Time.deltaTime, 0f, GetMaxStamina());

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
            var percentage = (this.GetMaxStamina() * amount / 100);
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
            return baseStamina + (int)(Mathf.Ceil(endurance * levelMultiplier));
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
    }
}
