﻿using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Item / New Consumable")]
    public class Consumable : Item
    {
        public enum OnConsumeActionType
        {
            EAT,
            DRINK,
            CLOCK
        }

        public enum ConsumablePropertyName
        {
            HEALTH_REGENERATION,
            STAMINA_REGENERATION_RATE,
            JUMP_HEIGHT,
            PHYSICAL_ATTACK_BONUS,
            SLOWER_STAMINA_REGENERATION_RATE,

            VITALITY_INCREASE,
            ENDURANCE_INCREASE,
            STRENGTH_INCREASE,
            DEXTERITY_INCREASE,
            INTELLIGENCE_INCREASE,

            ALL_STATS_INCREASE,

            REPUTATION_INCREASE,

            REVEAL_ILLUSIONARY_WALLS,

            SPEED_1_HOUR,

            RESTORE_SPELL_USE,

            NO_DAMAGE_FOR_X_SECONDS,

            FART_ON_HIT,

            IMMUNE_TO_FIRE,

            IMMUNE_TO_POISON,

            IMMUNE_TO_FROSTBITE,

            IMMUNE_TO_FEAR,

            INCREASES_POISE,
        }

        [System.Serializable]
        public class ConsumableEffect
        {
            public ConsumablePropertyName consumablePropertyName;
            public Sprite sprite;
            public Color barColor;
            public float value;
            public float effectDuration;
            public bool isPositive = true;
            public LocalizedText unit;

            [Tooltip("If true, effect will be evaluated every frame")] public bool tick = true;

        }

        [Header("FX")]
        public GameObject graphic;
        public GameObject particleOnConsume;
        public AudioClip sfxOnConsume;
        public bool destroyItemOnConsumeMoment = true;

        public bool canFavorite = true;

        [Header("Restore Health")]
        public bool restoreHealth = false;
        public bool restoreHealthInPercentage = false;
        public float amountOfHealthToRestore = 0f;

        [Header("Restore Stamina")]
        public bool restoreStamina = false;
        public bool restoreStaminaInPercentage = false;
        public float amountOfStaminaToRestore = 0f;

        [Header("Remove Negative Status")]
        public bool removeNegativeStatus = false;
        public StatusEffect statusToRemove;

        [Header("Apply Negative Status")]
        public bool applyNegativeStatus = false;
        public StatusEffect statusToApply;

        public bool notStackable = false;
        public bool shouldNotRemoveOnUse = false;

        public bool isAlcoholic = false;

        public ConsumableEffect[] consumableEffects;

        public OnConsumeActionType onConsumeActionType = OnConsumeActionType.DRINK;

        public virtual void OnConsume()
        {
            FindObjectOfType<PlayerInventory>(true).PrepareItemForConsuming(this);

        }

        public virtual void OnConsumeSuccess(PlayerInventory playerInventory)//, StatusDatabase statusDatabase)
        {

            if (shouldNotRemoveOnUse == false)
            {
                playerInventory.RemoveItem(this, 1);
            }

            //StartEffectOnPlayer(statusDatabase);
        }

        public void StartEffectOnPlayer()//StatusDatabase statusDatabase)
        {

            if (restoreHealth)
            {
                var healthStatManager = FindObjectOfType<PlayerHealth>(true);
                if (restoreHealthInPercentage)
                {
                    healthStatManager.RestoreHealthPercentage((int)amountOfHealthToRestore);
                }
                else
                {
                    healthStatManager.RestoreHealth((int)amountOfHealthToRestore);
                }
            }

            if (restoreStamina)
            {
                var staminaStatManager = FindObjectOfType<StaminaStatManager>(true);
                if (restoreStaminaInPercentage)
                {
                    staminaStatManager.RestoreStaminaPercentage(amountOfStaminaToRestore);
                }
                else
                {
                    staminaStatManager.RestoreStaminaPoints(amountOfStaminaToRestore);
                }
            }

            if (removeNegativeStatus)
            {
                //FindObjectOfType<PlayerStatusManager>(true).RemoveAppliedStatus(statusDatabase.appliedStatus.Find(x => x.statusEffect == statusToRemove));
            }

            if (applyNegativeStatus)
            {
                float maxAmountBeforeSuffering = FindObjectOfType<DefenseStatManager>(true).GetMaximumStatusResistanceBeforeSufferingStatusEffect(statusToApply);

                //var playerStatusManager = FindAnyObjectByType<PlayerStatusManager>(FindObjectsInactive.Include);

                // playerStatusManager.InflictStatusEffect(statusToApply, maxAmountBeforeSuffering, true);
            }

            //var playerConsumablesManager = FindObjectOfType<PlayerConsumablesManager>(true);
            foreach (var consumableEffect in consumableEffects)
            {
                /*
                AppliedConsumable appliedConsumables = new AppliedConsumable();
                appliedConsumables.consumableEffect = consumableEffect;
                appliedConsumables.currentDuration = consumableEffect.effectDuration;
                appliedConsumables.consumableEffectSprite = consumableEffect.sprite;
                appliedConsumables.consumableItemName = this.name.GetEnglishText();


                // Remove any applied consumable that contains one the consumable effects of this consumable
                var idx = statusDatabase.appliedConsumables.FindIndex(appliedConsumable => appliedConsumable.consumableEffect.consumablePropertyName == consumableEffect.consumablePropertyName);

                if (idx != -1)
                {
                    playerConsumablesManager.RemoveConsumable(statusDatabase.appliedConsumables[idx]);
                }

                // Is it a instant effect?
                if (appliedConsumables.currentDuration <= 0)
                {
                    playerConsumablesManager.EvaluateEffect(appliedConsumables);
                    return;
                }*/

                // playerConsumablesManager.AddConsumableEffect(appliedConsumables);
            }
        }
    }
}
