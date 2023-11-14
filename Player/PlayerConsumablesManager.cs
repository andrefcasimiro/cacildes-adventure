using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AF.Inventory;
using AF.Stats;
using UnityEngine;

namespace AF
{
    public class PlayerConsumablesManager : MonoBehaviour
    {
        DefenseStatManager defenseStatManager => GetComponent<DefenseStatManager>();
        HealthStatManager healthStatManager => GetComponent<HealthStatManager>();
        StaminaStatManager staminaStatManager => GetComponent<StaminaStatManager>();
        PlayerHealthbox playerHealthbox => GetComponentInChildren<PlayerHealthbox>();
        AttackStatManager attackStatManager => GetComponent<AttackStatManager>();
        ThirdPersonController thirdPersonController => GetComponent<ThirdPersonController>();

        [Header("Components")]
        public PlayerStatusManager playerStatusManager;
        public PlayerPoiseController playerPoiseController;

        bool isPassingTime = false;

        FavoriteItemsManager favoriteItemsManager => GetComponent<FavoriteItemsManager>();
        UIDocumentStatusEffectV2 uIDocumentStatusEffectV2;


        [Header("Components")]
        public StatsBonusController playerStatsBonusController;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public InventoryDatabase inventoryDatabase;
        public StatusDatabase statusDatabase;


        [Header("Systems")]
        public WorldSettings worldSettings;

        private void Awake()
        {
            uIDocumentStatusEffectV2 = FindAnyObjectByType<UIDocumentStatusEffectV2>(FindObjectsInactive.Include);
        }

        private void Update()
        {
            if (statusDatabase.appliedConsumables.Count > 0)
            {
                HandleConsumableEffects();
            }
        }

        public void AddConsumableEffect(AppliedConsumable consumableEffect)
        {
            var idx = statusDatabase.appliedConsumables.FindIndex(x => x.consumableEffect.consumablePropertyName == consumableEffect.consumableEffect.consumablePropertyName);
            if (idx != -1)
            {
                // If consuming an already applied consumable, prolong its time
                statusDatabase.appliedConsumables[idx].currentDuration += consumableEffect.consumableEffect.effectDuration;
                return;
            }

            statusDatabase.appliedConsumables.Add(consumableEffect);

            // If we only wish to evaluate the effect once, evaluate on add
            if (consumableEffect.consumableEffect.tick == false)
            {
                EvaluateEffect(statusDatabase.appliedConsumables.Last());
            }

            uIDocumentStatusEffectV2.AddConsumableEntry(consumableEffect.consumableEffect);
        }

        private void HandleConsumableEffects()
        {
            List<AppliedConsumable> consumablesToDelete = new List<AppliedConsumable>();

            foreach (var entry in statusDatabase.appliedConsumables)
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
            var consumables = statusDatabase.appliedConsumables.ToList();

            foreach (var c in consumables)
            {
                RemoveConsumable(c);
            }
        }

        public void RemoveConsumable(AppliedConsumable consumableToDelete)
        {
            statusDatabase.appliedConsumables.Remove(consumableToDelete);
            uIDocumentStatusEffectV2.RemoveConsumableEntry(consumableToDelete);

            if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.STAMINA_REGENERATION_RATE)
            {
                staminaStatManager.staminaRegenerationBonus = 0f;
            }
            else if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.JUMP_HEIGHT)
            {
                thirdPersonController.trackFallDamage = true;
                thirdPersonController.JumpHeightBonus = 0f;
            }
            else if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.PHYSICAL_ATTACK_BONUS)
            {
                attackStatManager.physicalAttackBonus = 0f;
            }
            else if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.SLOWER_STAMINA_REGENERATION_RATE)
            {
                staminaStatManager.negativeStaminaRegenerationBonus = 0f;
            }
            else if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.ALL_STATS_INCREASE)
            {
                playerStatsBonusController.vitalityBonus -= (int)consumableToDelete.consumableEffect.value;
                playerStatsBonusController.enduranceBonus -= (int)consumableToDelete.consumableEffect.value;
                playerStatsBonusController.dexterityBonus -= (int)consumableToDelete.consumableEffect.value;
                playerStatsBonusController.strengthBonus -= (int)consumableToDelete.consumableEffect.value;
                playerStatsBonusController.intelligenceBonus -= (int)consumableToDelete.consumableEffect.value;
            }
            else if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.VITALITY_INCREASE)
            {
                playerStatsBonusController.vitalityBonus -= (int)consumableToDelete.consumableEffect.value;
            }
            else if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.ENDURANCE_INCREASE)
            {
                playerStatsBonusController.enduranceBonus -= (int)consumableToDelete.consumableEffect.value;
            }
            else if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.STRENGTH_INCREASE)
            {
                playerStatsBonusController.strengthBonus -= (int)consumableToDelete.consumableEffect.value;
            }
            else if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.DEXTERITY_INCREASE)
            {
                playerStatsBonusController.dexterityBonus -= (int)consumableToDelete.consumableEffect.value;
            }
            else if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.INTELLIGENCE_INCREASE)
            {
                playerStatsBonusController.intelligenceBonus -= (int)consumableToDelete.consumableEffect.value;
            }
            else if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.NO_DAMAGE_FOR_X_SECONDS)
            {
                playerHealthbox.isInvencible = false;
            }
            else if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.FART_ON_HIT)
            {
                playerHealthbox.fartOnHit = false;
            }
            else if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.IMMUNE_TO_FIRE)
            {
                playerHealthbox.isImmuneToFireDamage = false;
            }
            else if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.IMMUNE_TO_POISON)
            {
                playerStatusManager.immuneToPoison = false;
            }
            else if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.IMMUNE_TO_FEAR)
            {
                playerStatusManager.immuneToFear = false;
            }
            else if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.IMMUNE_TO_FROSTBITE)
            {
                playerStatusManager.immuneToFrostbite = false;
            }
            else if (consumableToDelete.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.INCREASES_POISE)
            {
                playerPoiseController.poiseBonus -= (int)consumableToDelete.consumableEffect.value;
            }
        }

        public void EvaluateEffect(AppliedConsumable entry)
        {
            if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.HEALTH_REGENERATION)
            {
                healthStatManager.RestoreHealthPoints((int)entry.consumableEffect.value);
            }
            else if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.STAMINA_REGENERATION_RATE)
            {
                staminaStatManager.staminaRegenerationBonus = entry.consumableEffect.value;
            }
            else if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.JUMP_HEIGHT)
            {
                thirdPersonController.trackFallDamage = false;
                thirdPersonController.JumpHeightBonus = entry.consumableEffect.value;
            }
            else if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.PHYSICAL_ATTACK_BONUS)
            {
                attackStatManager.physicalAttackBonus = entry.consumableEffect.value;
            }
            else if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.SLOWER_STAMINA_REGENERATION_RATE)
            {
                staminaStatManager.negativeStaminaRegenerationBonus = entry.consumableEffect.value;
            }
            else if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.ALL_STATS_INCREASE)
            {
                playerStatsBonusController.vitalityBonus += (int)entry.consumableEffect.value;
                playerStatsBonusController.enduranceBonus += (int)entry.consumableEffect.value;
                playerStatsBonusController.dexterityBonus += (int)entry.consumableEffect.value;
                playerStatsBonusController.strengthBonus += (int)entry.consumableEffect.value;
                playerStatsBonusController.intelligenceBonus += (int)entry.consumableEffect.value;
            }
            else if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.VITALITY_INCREASE)
            {
                playerStatsBonusController.vitalityBonus += (int)entry.consumableEffect.value;
            }
            else if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.ENDURANCE_INCREASE)
            {
                playerStatsBonusController.enduranceBonus += (int)entry.consumableEffect.value;
            }
            else if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.STRENGTH_INCREASE)
            {
                playerStatsBonusController.strengthBonus += (int)entry.consumableEffect.value;
            }
            else if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.DEXTERITY_INCREASE)
            {
                playerStatsBonusController.dexterityBonus += (int)entry.consumableEffect.value;
            }
            else if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.INTELLIGENCE_INCREASE)
            {
                playerStatsBonusController.intelligenceBonus += (int)entry.consumableEffect.value;
            }
            else if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.NO_DAMAGE_FOR_X_SECONDS)
            {
                playerHealthbox.isInvencible = true;
            }
            else if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.REVEAL_ILLUSIONARY_WALLS)
            {
                foreach (var illusionaryWall in FindObjectsByType<IllusionaryWall>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                {
                    illusionaryWall.hasBeenHit = true;
                }
                foreach (var veilPiercerActivable in FindObjectsByType<VeilPiercerActivable>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                {
                    veilPiercerActivable.gameObject.SetActive(false);
                }
            }
            else if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.FART_ON_HIT)
            {
                playerHealthbox.fartOnHit = true;
            }
            else if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.SPEED_1_HOUR)
            {
                StartCoroutine(MoveTime());
            }
            else if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.RESTORE_SPELL_USE)
            {
                foreach (var item in inventoryDatabase.ownedItems)
                {
                    var castedSpell = item.item as Spell;

                    if (castedSpell != null)
                    {
                        item.amount += item.usages;
                        item.usages = 0;

                    }
                }

                // favoriteItemsManager.UpdateFavoriteItems();
            }
            else if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.IMMUNE_TO_FIRE)
            {
                playerHealthbox.isImmuneToFireDamage = true;
            }
            else if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.IMMUNE_TO_POISON)
            {
                playerStatusManager.immuneToPoison = true;
            }
            else if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.IMMUNE_TO_FEAR)
            {
                playerStatusManager.immuneToFear = true;
            }
            else if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.IMMUNE_TO_FROSTBITE)
            {
                playerStatusManager.immuneToFrostbite = true;
            }
            else if (entry.consumableEffect.consumablePropertyName == Consumable.ConsumablePropertyName.INCREASES_POISE)
            {
                playerPoiseController.poiseBonus += (int)entry.consumableEffect.value;
            }

        }


        IEnumerator MoveTime()
        {
            if (!isPassingTime)
            {
                isPassingTime = true;

                bool isInteriorOriginal = FindObjectOfType<SceneSettings>(true).isInterior;

                FindObjectOfType<SceneSettings>(true).isInterior = false;
                FindObjectOfType<DayNightManager>(true).tick = true;
                var originalDaySpeed = worldSettings.daySpeed;

                var targetHour = Mathf.Floor(worldSettings.timeOfDay) + 1;

                if (targetHour > 23)
                {
                    worldSettings.timeOfDay = 0;
                    targetHour = 0;
                }

                yield return null;

                worldSettings.daySpeed = 2;

                yield return new WaitUntil(() => Mathf.Floor(worldSettings.timeOfDay) == targetHour);

                worldSettings.daySpeed = originalDaySpeed;

                FindObjectOfType<DayNightManager>(true).tick = FindObjectOfType<DayNightManager>(true).TimePassageAllowed();
                FindObjectOfType<SceneSettings>(true).isInterior = isInteriorOriginal;

                isPassingTime = false;
            }

            yield return null;
        }
    }
}

