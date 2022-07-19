using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace AF
{
    public class PlayerStatsManager : MonoBehaviour
    {
        public static PlayerStatsManager instance;

        // Stats
        public float currentHealth;
        public float currentMagic;
        public float currentStamina;
        public int currentReputation;
        public int currentExperience;

        // Attributes
        public int vitality = 0;
        public int intelligence = 0;
        public int endurance = 0;
        public int strength = 0;
        public int dexterity = 0;
        public int arcane = 0;
        public int occult = 0;
        public int charisma = 0;
        public int luck = 0;

        // Scaling Multipliers
        readonly float EScalingBonus = 0;
        readonly float DScalingBonus = 0.4f;
        readonly float CScalingBonus = 0.8f;
        readonly float BScalingBonus = 1.2f;
        readonly float AScalingBonus = 1.6f;
        readonly float SScalingBonus = 2f;

        public List<Consumable> appliedConsumables = new List<Consumable>();

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }

        void Start()
        {

            this.currentHealth = GetMaxHealthPoints();
        }

        private void Update()
        {
            if (this.appliedConsumables.Count > 0)
            {
                foreach (Consumable consumable in this.appliedConsumables)
                {
                    consumable.effectDuration -= Time.deltaTime;

                    if (consumable.effectDuration <= 0f)
                    {
                        this.appliedConsumables.Remove(consumable);
                    }
                }
            }
        }

        #region Weapon Attack Calculations
        public float GetWeaponAttack(Weapon weapon)
        {
            float baseDamageSolelyFromWeapon = weapon.physicalAttack;

            float dexterityBonus = GetFinalDexterityBonusDifference(weapon);
            float strengthBonus = GetFinalStrengthBonusDifference(weapon);

            return (baseDamageSolelyFromWeapon + dexterityBonus + strengthBonus);
        }

        public float GetMaxStrengthAttackBonus()
        {
            return Mathf.Ceil(this.strength * 2.5f);
        }

        public float GetMaxDexterityAttackBonus()
        {
            return Mathf.Ceil(this.dexterity * 2.5f);
        }

        public float GetWeaponDexterityScaling(Weapon weapon)
        {
            float scalingBonus = 0f;

            if (weapon.dexterityScaling == Scaling.E)
            {
                scalingBonus = EScalingBonus;
            }
            else if (weapon.dexterityScaling == Scaling.D)
            {
                scalingBonus = DScalingBonus;
            }
            else if (weapon.dexterityScaling == Scaling.C)
            {
                scalingBonus = CScalingBonus;
            }
            else if (weapon.dexterityScaling == Scaling.B)
            {
                scalingBonus = BScalingBonus;
            }
            else if (weapon.dexterityScaling == Scaling.A)
            {
                scalingBonus = AScalingBonus;
            }
            else if (weapon.dexterityScaling == Scaling.S)
            {
                scalingBonus = SScalingBonus;
            }

            return scalingBonus;
        }

        public float GetWeaponStrengthScaling(Weapon weapon)
        {
            float scalingBonus = 0f;

            if (weapon.strengthScaling == Scaling.E)
            {
                scalingBonus = EScalingBonus;
            }
            else if (weapon.strengthScaling == Scaling.D)
            {
                scalingBonus = DScalingBonus;
            }
            else if (weapon.strengthScaling == Scaling.C)
            {
                scalingBonus = CScalingBonus;
            }
            else if (weapon.strengthScaling == Scaling.B)
            {
                scalingBonus = BScalingBonus;
            }
            else if (weapon.strengthScaling == Scaling.A)
            {
                scalingBonus = AScalingBonus;
            }
            else if (weapon.strengthScaling == Scaling.S)
            {
                scalingBonus = SScalingBonus;
            }

            return (scalingBonus);
        }

        public float CalculateStrengthBonusForWeapon(Weapon weapon)
        {
            float weaponScalingBonus = GetWeaponStrengthScaling(weapon);

            return Mathf.Ceil(this.GetMaxStrengthAttackBonus() * weaponScalingBonus);
        }

        public float CalculateDexterityBonusForWeapon(Weapon weapon)
        {
            float weaponScalingBonus = GetWeaponDexterityScaling(weapon);

            return Mathf.Ceil(this.GetMaxDexterityAttackBonus() * weaponScalingBonus);
        }

        public float GetFinalDexterityBonusDifference(Weapon weapon)
        {
            return (weapon.physicalAttack + CalculateDexterityBonusForWeapon(weapon)) - weapon.physicalAttack;
        }

        public float GetFinalStrengthBonusDifference(Weapon weapon)
        {
            return (weapon.physicalAttack + CalculateStrengthBonusForWeapon(weapon)) - weapon.physicalAttack;
        }
        #endregion

        #region Level Calculations
        public int GetCurrentLevel()
        {
            return 1 + (this.vitality + this.strength + this.dexterity - 3);
        }

        public float GetRequiredExperienceForNextLevel()
        {
            return (this.GetCurrentLevel() + this.GetCurrentLevel() - 1) * 1.25f * 100;
        }
        #endregion

        #region Health Calculations

        public float GetMaxHealthPoints()
        {
            return (this.vitality * .25f) * 100; // + Accessory 1 & 2 Bonus Increases and Temporary Buffs
        }

        public float GetCurrentHealth()
        {
            return this.currentHealth;
        }

        public void RestoreHealthPercentage(float currentHealthPercentage)
        {
            float percentage = this.GetMaxHealthPoints() * currentHealthPercentage / 100;
            float nextCurrentHealthValue = Mathf.Clamp(this.currentHealth + percentage, 0, this.GetMaxHealthPoints());

            SetCurrentHealth(nextCurrentHealthValue);
        }

        public void RestoreHealthPoints(float healthPoints)
        {
            float nextCurrentHealthValue = Mathf.Clamp(this.currentHealth + healthPoints, 0, this.GetMaxHealthPoints());

            SetCurrentHealth(nextCurrentHealthValue);
        }

        public void SetCurrentHealth(float currentHealth)
        {
            this.currentHealth = Mathf.Clamp(currentHealth, 0, this.GetMaxHealthPoints());
        }
        #endregion

        #region Magic Calculations
        public float GetMaxMagicPoints()
        {
            return (this.intelligence * .25f) * 100;
        }
        #endregion

        #region Stamina Calculations
        public float GetMaxStaminaPoints()
        {
            return (this.endurance * .25f) * 100;
        }

        public bool HasEnoughStaminaForAction(float actionStaminaCost)
        {
            return actionStaminaCost >= this.currentStamina;
        }
        #endregion

        #region Defense Calculations
        public float GetDefenseAbsorption()
        {
            float defenseAbsorption = 0f;

            if (PlayerInventoryManager.instance.currentHelmet != null)
            {
                defenseAbsorption += PlayerInventoryManager.instance.currentHelmet.physicalDefense;
            }
            if (PlayerInventoryManager.instance.currentChest != null)
            {
                defenseAbsorption += PlayerInventoryManager.instance.currentChest.physicalDefense;
            }
            if (PlayerInventoryManager.instance.currentGauntlets != null)
            {
                defenseAbsorption += PlayerInventoryManager.instance.currentGauntlets.physicalDefense;
            }
            if (PlayerInventoryManager.instance.currentLegwear)
            {
                defenseAbsorption += PlayerInventoryManager.instance.currentLegwear.physicalDefense;
            }

            return defenseAbsorption;
        }
        #endregion

        #region Save System
        public void OnGameLoaded(GameData gameData)
        {
            PlayerData playerData = gameData.playerData;

            this.currentExperience = playerData.currentExperience;
            SetCurrentHealth(playerData.currentHealth);
            this.currentMagic = playerData.currentMagic;
            this.currentStamina = playerData.currentStamina;

            this.currentReputation = playerData.currentReputation;

            this.vitality = playerData.vitality;
            this.intelligence = playerData.intelligence;
            this.endurance = playerData.endurance;
            this.strength = playerData.strength;
            this.dexterity = playerData.dexterity;
            this.arcane = playerData.arcane;
            this.occult = playerData.occult;
            this.charisma = playerData.charisma;
            this.luck = playerData.luck;

            if (this.currentHealth <= 0)
            {
                // If for some reason we saved a player with negative or zero health, this is some bug, so let's give him 1HP
                SetCurrentHealth(1f);
            }
        }
        #endregion
    }
}