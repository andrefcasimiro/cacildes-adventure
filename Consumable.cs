using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Item / New Consumable")]
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
        }

        [System.Serializable]
        public class ConsumableEffect
        {
            public ConsumablePropertyName consumablePropertyName;
            public string displayName;
            public Sprite sprite;
            public Color barColor;
            public float value;
            public float effectDuration;
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

            var playerConsumablesManager = FindObjectOfType<PlayerConsumablesManager>(true);
            foreach (var consumableEffect in consumableEffects)
            {
                AppliedConsumable appliedConsumables = new AppliedConsumable();
                appliedConsumables.consumableEffect = consumableEffect;
                appliedConsumables.currentDuration = consumableEffect.effectDuration;
                appliedConsumables.consumableEffectSprite = consumableEffect.sprite;

                playerConsumablesManager.AddConsumableEffect(appliedConsumables);
            }
        }
    }

}
