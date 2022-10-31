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
        public const float EMPTY_STAMINA_REGENERATION_DELAY = 0.5f;
        public bool shouldRegenerateStamina = false;

        StarterAssets.StarterAssetsInputs inputs;

        private void Awake()
        {
            inputs = FindObjectOfType<StarterAssets.StarterAssetsInputs>(true);
        }

        public int GetMaxStamina()
        {
            return baseStamina + (int)(Mathf.Ceil(Player.instance.endurance * levelMultiplier));
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
            var finalRegenerationRate = STAMINA_REGENERATION_RATE;

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
    }
}
