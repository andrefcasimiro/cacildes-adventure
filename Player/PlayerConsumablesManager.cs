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
        EquipmentGraphicsHandler equipmentGraphicsHandler => GetComponent<EquipmentGraphicsHandler>();

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
            
            // If we only wish to evaluate the effect once, evaluate on add
            if (consumableEffect.consumableEffect.tick == false)
            {
                EvaluateEffect(Player.instance.appliedConsumables.Last());
            }

            FindObjectOfType<UIDocumentStatusEffectV2>(true).AddConsumableEntry(consumableEffect.consumableEffect);
        }

        private void HandleConsumableEffects()
        {
            List<AppliedConsumable> consumablesToDelete = new List<AppliedConsumable>();

            foreach (var entry in Player.instance.appliedConsumables)
            {
                entry.currentDuration -= Time.deltaTime;

                if (entry.consumableEffect.tick)
                {
                    EvaluateEffect(entry);
                }

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
                staminaStatManager.negativeStaminaRegenerationBonus = 0f;
            }

            if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.ALL_STATS_INCREASE)
            {
                equipmentGraphicsHandler.vitalityBonus -= (int)consumableToDelete.consumableEffect.value;
                equipmentGraphicsHandler.enduranceBonus -= (int)consumableToDelete.consumableEffect.value;
                equipmentGraphicsHandler.dexterityBonus -= (int)consumableToDelete.consumableEffect.value;
                equipmentGraphicsHandler.strengthBonus -= (int)consumableToDelete.consumableEffect.value;
            }
            if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.VITALITY_INCREASE)
            {
                equipmentGraphicsHandler.vitalityBonus -= (int)consumableToDelete.consumableEffect.value;
            }
            if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.ENDURANCE_INCREASE)
            {
                equipmentGraphicsHandler.enduranceBonus -= (int)consumableToDelete.consumableEffect.value;
            }
            if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.STRENGTH_INCREASE)
            {
                equipmentGraphicsHandler.strengthBonus -= (int)consumableToDelete.consumableEffect.value;
            }
            if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.DEXTERITY_INCREASE)
            {
                equipmentGraphicsHandler.dexterityBonus -= (int)consumableToDelete.consumableEffect.value;
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


            if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.ALL_STATS_INCREASE)
            {
                equipmentGraphicsHandler.vitalityBonus += (int)entry.consumableEffect.value;
                equipmentGraphicsHandler.enduranceBonus += (int)entry.consumableEffect.value;
                equipmentGraphicsHandler.dexterityBonus += (int)entry.consumableEffect.value;
                equipmentGraphicsHandler.strengthBonus += (int)entry.consumableEffect.value;
            }
            if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.VITALITY_INCREASE)
            {
                equipmentGraphicsHandler.vitalityBonus += (int)entry.consumableEffect.value;
            }
            if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.ENDURANCE_INCREASE)
            {
                equipmentGraphicsHandler.enduranceBonus += (int)entry.consumableEffect.value;
            }
            if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.STRENGTH_INCREASE)
            {
                equipmentGraphicsHandler.strengthBonus += (int)entry.consumableEffect.value;
            }
            if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.DEXTERITY_INCREASE)
            {
                equipmentGraphicsHandler.dexterityBonus += (int)entry.consumableEffect.value;
            }
        }
    }
}

