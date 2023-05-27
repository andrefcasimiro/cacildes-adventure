using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Item / New Consumable")]
    public class Consumable : Item
    {
        public enum OnConsumeActionType
        {
            EAT,
            DRINK
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

            ALL_STATS_INCREASE,

            REPUTATION_INCREASE,
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

        public ConsumableEffect[] consumableEffects;

        public OnConsumeActionType onConsumeActionType = OnConsumeActionType.DRINK;

        public virtual void OnConsume()
        {
            PlayerInventory playerInventory = FindObjectOfType<PlayerInventory>(true);

            playerInventory.PrepareItemForConsuming(this);

        }

        public virtual void OnConsumeSuccess()
        {
            PlayerInventory playerInventory = FindObjectOfType<PlayerInventory>(true);

            if (Player.instance.currentHealth <= 0)
            {
                return;
            }

            playerInventory.RemoveItem(this, 1);

            if (restoreHealth)
            {
                var healthStatManager = FindObjectOfType<HealthStatManager>(true);
                if (restoreHealthInPercentage)
                {
                    float percentage = healthStatManager.GetMaxHealth() * amountOfHealthToRestore / 100;
                    healthStatManager.RestoreHealthPercentage(percentage);
                }
                else
                {
                    healthStatManager.RestoreHealthPoints(amountOfHealthToRestore);
                }
            }

            if (restoreStamina)
            {
                var staminaStatManager = FindObjectOfType<StaminaStatManager>(true);
                if (restoreStaminaInPercentage)
                {
                    float percentage = staminaStatManager.GetMaxStamina() * amountOfStaminaToRestore / 100;
                    staminaStatManager.RestoreStaminaPercentage(percentage);
                }
                else
                {
                    staminaStatManager.RestoreStaminaPoints(amountOfStaminaToRestore);
                }
            }
        
            if (removeNegativeStatus)
            {
                FindObjectOfType<PlayerStatusManager>(true).RemoveAppliedStatus(Player.instance.appliedStatus.Find(x => x.statusEffect == statusToRemove));
            }

            if (applyNegativeStatus)
            {
                float maxAmountBeforeSuffering = FindObjectOfType<DefenseStatManager>(true).GetMaximumStatusResistanceBeforeSufferingStatusEffect(statusToApply);

                FindObjectOfType<PlayerStatusManager>(true).InflictStatusEffect(statusToApply, maxAmountBeforeSuffering, true);
            }

            var playerConsumablesManager = FindObjectOfType<PlayerConsumablesManager>(true);
            foreach (var consumableEffect in consumableEffects)
            {
                AppliedConsumable appliedConsumables = new AppliedConsumable();
                appliedConsumables.consumableEffect = consumableEffect;
                appliedConsumables.currentDuration = consumableEffect.effectDuration;
                appliedConsumables.consumableEffectSprite = consumableEffect.sprite;
                appliedConsumables.consumableItemName = this.name.GetEnglishText();


                // Remove any applied consumable that contains one the consumable effects of this consumable
                var idx = Player.instance.appliedConsumables.FindIndex(appliedConsumable => appliedConsumable.consumableEffect.consumablePropertyName == consumableEffect.consumablePropertyName);

                if (idx != -1)
                {
                    playerConsumablesManager.RemoveConsumable(Player.instance.appliedConsumables[idx]);
                }

                playerConsumablesManager.AddConsumableEffect(appliedConsumables);
            }
        }
    }
}
