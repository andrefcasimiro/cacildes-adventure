using System.Collections;
using System.Collections.Generic;
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
        public const float EMPTY_STAMINA_REGENERATION_DELAY = 0.5f;
        public bool shouldRegenerateStamina = false;

        StarterAssets.StarterAssetsInputs inputs;

        EquipmentGraphicsHandler equipmentGraphicsHandler => GetComponent<EquipmentGraphicsHandler>();

        PlayerParryManager playerParryManager => GetComponent<PlayerParryManager>();

        private void Awake()
        {
            inputs = FindObjectOfType<StarterAssets.StarterAssetsInputs>(true);
        }

        private void Start()
        {
            if (Player.instance.currentStamina < GetMaxStamina())
            {
                shouldRegenerateStamina = true;
            }
        }

        public int GetMaxStamina()
        {
            return baseStamina + (int)(Mathf.Ceil(Player.instance.endurance * levelMultiplier)) + (int)(equipmentGraphicsHandler.enduranceBonus * levelMultiplier);
        }

        public void DecreaseStamina(float amount)
        {
            shouldRegenerateStamina = false;

            Player.instance.currentStamina = Mathf.Clamp(Player.instance.currentStamina - amount, 0, GetMaxStamina());
            
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
            var finalRegenerationRate = STAMINA_REGENERATION_RATE + staminaRegenerationBonus;

            if (playerParryManager.IsBlocking())
            {
                finalRegenerationRate = finalRegenerationRate / 4;
            }

            Player.instance.currentStamina += Mathf.Clamp(finalRegenerationRate * Time.deltaTime, 0f, GetMaxStamina());

            if (Player.instance.currentStamina >= GetMaxStamina())
            {
                shouldRegenerateStamina = false;
            }
        }

        public bool HasEnoughStaminaForAction(float actionStaminaCost)
        {
            return Player.instance.currentStamina - actionStaminaCost > 0;
        }

        public void RestoreStaminaPercentage(float amount)
        {
            var percentage = (this.GetMaxStamina() * amount / 100);
            var nextValue = Mathf.Clamp(Player.instance.currentStamina + percentage, 0, this.GetMaxStamina());

            Player.instance.currentStamina = nextValue;
        }

        public void RestoreStaminaPoints(float amount)
        {
            var nextValue = Mathf.Clamp(Player.instance.currentStamina + amount, 0, this.GetMaxStamina());

            Player.instance.currentStamina = nextValue;
        }


        public float GetStaminaPointsForGivenEndurance(int endurance)
        {
            return baseStamina + (int)(Mathf.Ceil(endurance * levelMultiplier));
        }
    }
}
