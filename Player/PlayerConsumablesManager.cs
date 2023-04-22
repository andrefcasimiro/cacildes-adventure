using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF
{
    public class PlayerConsumablesManager : MonoBehaviour
    {
        DefenseStatManager defenseStatManager => GetComponent<DefenseStatManager>();
        HealthStatManager healthStatManager => GetComponent<HealthStatManager>();
        StaminaStatManager staminaStatManager => GetComponent<StaminaStatManager>();
        PlayerHealthbox playerHealthbox => GetComponent<PlayerHealthbox>();
        AttackStatManager attackStatManager => GetComponent<AttackStatManager>();
        StarterAssets.ThirdPersonController thirdPersonController => GetComponent<StarterAssets.ThirdPersonController>();

        private void Update()
        {
            if (Player.instance.appliedConsumables.Count > 0)
            {
                HandleConsumableEffects();
            }
        }

        public void AddConsumableEffect(AppliedConsumable consumableEffect)
        {
            var idx = Player.instance.appliedConsumables.FindIndex(x => x.consumableEffect.consumablePropertyName == consumableEffect.consumableEffect.consumablePropertyName);
            if (idx != -1)
            {
                // If consuming an already applied consumable, prolong its time
                Player.instance.appliedConsumables[idx].currentDuration += consumableEffect.consumableEffect.effectDuration;
                return;
            }

            Player.instance.appliedConsumables.Add(consumableEffect);
            FindObjectOfType<UIDocumentStatusEffectV2>(true).AddConsumableEntry(consumableEffect.consumableEffect);
        }

        private void HandleConsumableEffects()
        {
            List<AppliedConsumable> consumablesToDelete = new List<AppliedConsumable>();

            foreach (var entry in Player.instance.appliedConsumables)
            {
                entry.currentDuration -= Time.deltaTime;

                EvaluateEffect(entry);

                if (entry.currentDuration <= 0)
                {
                    consumablesToDelete.Add(entry);
                }
            }

            foreach (var consumableToDelete in consumablesToDelete)
            {
                RemoveConsumable(consumableToDelete);
            }

        }

        public void ClearAllConsumables()
        {
             var consumables = Player.instance.appliedConsumables.ToList();

            foreach (var c in consumables)
            {
                RemoveConsumable(c);
            }
        }

        public void RemoveConsumable(AppliedConsumable consumableToDelete)
        {
            Player.instance.appliedConsumables.Remove(consumableToDelete);
            FindObjectOfType<UIDocumentStatusEffectV2>(true).RemoveConsumableEntry(consumableToDelete);

            if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.STAMINA_REGENERATION_RATE)
            {
                staminaStatManager.staminaRegenerationBonus = 0f;
            }

            if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.JUMP_HEIGHT)
            {
                thirdPersonController.trackFallDamage = true;
                thirdPersonController.JumpHeightBonus = 0f;
            }

            if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.PHYSICAL_ATTACK_BONUS)
            {
                attackStatManager.physicalAttackBonus = 0f;
            }

            if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.SLOWER_STAMINA_REGENERATION_RATE)
            {
                staminaStatManager.negativeStaminaRegenerationBonus = consumableToDelete.consumableEffect.value;
            }
        }

        void EvaluateEffect(AppliedConsumable entry)
        {
            if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.HEALTH_REGENERATION)
            {
                healthStatManager.RestoreHealthPoints(entry.consumableEffect.value);
            }

            if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.STAMINA_REGENERATION_RATE)
            {
                staminaStatManager.staminaRegenerationBonus = entry.consumableEffect.value;
            }

            if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.JUMP_HEIGHT)
            {
                thirdPersonController.trackFallDamage = false;
                thirdPersonController.JumpHeightBonus = entry.consumableEffect.value;
            }

            if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.PHYSICAL_ATTACK_BONUS)
            {
                attackStatManager.physicalAttackBonus = entry.consumableEffect.value;
            }

            if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.SLOWER_STAMINA_REGENERATION_RATE)
            {
                staminaStatManager.negativeStaminaRegenerationBonus = entry.consumableEffect.value;
            }

        }
    }
}

