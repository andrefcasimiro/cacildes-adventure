using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace AF
{

    [System.Serializable]
    public class AppliedConsumable
    {
        public float currentDuration;
        public Consumable consumable;
    }

    [System.Serializable]
    public class AppliedStatus
    {
        public StatusEffect statusEffect;

        public bool hasReachedTotalAmount;

        public float currentAmount;
    }

    public class PlayerStatsManager : MonoBehaviour, ISaveable
    {
        public static PlayerStatsManager instance;

        // Stats
        public float currentHealth;
        public float currentMagic;
        public float currentStamina;
        public int currentReputation;
        public float currentExperience;

        public int gold;

        public float currentPhysicalAttack;
        public float currentPhysicalDefense;

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

        public float STAMINA_REGENERATION_RATE = 18f;
        bool regenStamina = false;
        public const float EMPTY_STAMINA_REGENERATION_DELAY = 0.5f;

        public List<AppliedConsumable> appliedConsumables = new List<AppliedConsumable>();
        public List<AppliedStatus> appliedStatus = new List<AppliedStatus>();

        public List<StatusEffect> statusEffectsDatabase = new List<StatusEffect>();

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
            this.currentStamina = GetMaxStaminaPoints();
        }

        private void Update()
        {
            ManageConsumablesOverTime();

            if (regenStamina)
            {
                HandleStaminaRegen();
            }

            HandleAppliedStatusEffect();
        }

        #region Experience
        public void AddExperience(int amount)
        {
            this.currentExperience += amount;
        }

        public bool HasEnoughExperienceForLevelling(float experience, int levelDesired)
        {
            if (experience >= GetRequiredExperienceForGivenLevel(levelDesired - 1))
            {
                return true;
            }

            return (experience - GetRequiredExperienceForGivenLevel(levelDesired)) >= 0;
        }
        #endregion

        #region Weapon Attack Calculations
        public float GetWeaponAttack(Weapon weapon)
        {
            float baseDamageSolelyFromWeapon = weapon.physicalAttack;

            float dexterityBonus = GetFinalDexterityBonusDifference(weapon);
            float strengthBonus = GetFinalStrengthBonusDifference(weapon);

            return GetLevelPhysicalAttack() + (baseDamageSolelyFromWeapon + dexterityBonus + strengthBonus);
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

        public float GetLevelPhysicalAttack()
        {
            return Mathf.Ceil(100 + ((strength + dexterity) * 2.25f));
        }

        public float GetPhysicalAttackForDesiredStrengthAndDexterity(int strength, int dexterity)
        {
            return Mathf.Ceil(100 + ((strength + dexterity) * 2.25f));
        }
        #endregion

        #region Level Calculations
        public int GetCurrentLevel()
        {
            return (this.vitality + this.strength + this.dexterity + this.intelligence + this.endurance + this.charisma + this.arcane);
        }

        public int GetRequiredExperienceForGivenLevel(int level)
        {
            return Mathf.RoundToInt(Mathf.Ceil((level + level - 1) * 1.25f * 100));
        }

        public float GetRequiredExperienceForNextLevel()
        {
            return Mathf.RoundToInt(Mathf.Ceil((this.GetCurrentLevel() + this.GetCurrentLevel() - 1) * 1.25f * 100));
        }
        #endregion

        #region Health Calculations

        public float GetMaxHealthPoints()
        {
            return Mathf.Ceil(this.vitality * 2.25f) + 100; // + Accessory 1 & 2 Bonus Increases and Temporary Buffs
        }

        public float GetHealthPointsForGivenVitality(int vitality)
        {
            return Mathf.Ceil(vitality * 2.25f) + 100; // + Accessory 1 & 2 Bonus Increases and Temporary Buffs
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
            return Mathf.Ceil((this.intelligence * 2.25f) + 100);
        }

        public float GetMagicPointsForGivenIntelligence(int intelligence)
        {
            return Mathf.Ceil(intelligence * 2.25f) + 100; 
        }

        public void SetCurrentMagic(float points)
        {
            this.currentMagic = Mathf.Clamp(points, 0, this.GetMaxMagicPoints());
        }
        
        public void RestoreMagicPercentage(float currentPercentage)
        {
            float percentage = this.GetMaxMagicPoints() * currentPercentage / 100;
            float nextCurrentHealthValue = Mathf.Clamp(this.currentMagic + percentage, 0, this.GetMaxMagicPoints());

            SetCurrentMagic(nextCurrentHealthValue);
        }

        public void RestoreMagicPoints(float points)
        {
            float nextValue = Mathf.Clamp(this.currentMagic + points, 0, this.GetMaxMagicPoints());

            SetCurrentMagic(nextValue);
        }
        #endregion

        #region Stamina Calculations
        public float GetMaxStaminaPoints()
        {
            return Mathf.Ceil((this.endurance * .25f) + 100);
        }

        public float GetCurrentStamina()
        {
            return Mathf.Round(this.currentStamina);
        }

        public float GetStaminaPointsForGivenEndurance(int endurance)
        {
            return Mathf.Ceil(endurance * 2.25f) + 100;
        }

        public bool HasEnoughStaminaForAction(float actionStaminaCost)
        {
            return this.currentStamina - actionStaminaCost > 0;
        }

        public void SetCurrentStamina(float points)
        {
            this.currentStamina = Mathf.Clamp(points, 0, this.GetMaxStaminaPoints());
        }

        public void RestoreStaminaPoints(float points)
        {
            float nextValue = Mathf.Clamp(this.currentStamina + points, 0, this.GetMaxStaminaPoints());

            SetCurrentStamina(nextValue);
        }

        void HandleStaminaRegen()
        {
            Player player = FindObjectOfType<Player>(true);
            if (player.isSprinting)
            {
                return;
            }

            var finalRegenerationRate = STAMINA_REGENERATION_RATE;

            foreach (AppliedConsumable appliedConsumable in this.appliedConsumables)
            {
                if (appliedConsumable.consumable.action == Action.Regenerate)
                {
                    if (appliedConsumable.consumable.stat == Stat.Stamina)
                    {
                        float bonusRate = appliedConsumable.consumable.value * STAMINA_REGENERATION_RATE / 100;

                        finalRegenerationRate += bonusRate;
                    }
                }
            }

            this.currentStamina += finalRegenerationRate * Time.deltaTime;


            if (this.currentStamina >= GetMaxStaminaPoints())
            {
                regenStamina = false;
            }
        }

        public void DecreaseStamina(float amount)
        {
            regenStamina = false;

            var nextValue = this.currentStamina - amount;

            this.currentStamina = Mathf.Clamp(nextValue, 0, GetMaxStaminaPoints());

            StartCoroutine(RegenerateEmptyStamina());
        }

        IEnumerator RegenerateEmptyStamina()
        {
            yield return new WaitForSeconds(EMPTY_STAMINA_REGENERATION_DELAY);

            regenStamina = true;
        }
        #endregion

        #region Defense Calculations
        public float GetDefenseAbsorption()
        {
            return GetLevelPhysicalDefense() + GetEquipmentDefenseAbsorption();
        }

        public float GetEquipmentDefenseAbsorption()
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

        public float GetLevelPhysicalDefense()
        {
            return Mathf.Ceil(100 + (endurance * 2.25f));
        }

        public float GetPhysicalDefenseForGivenEndurance(int endurance)
        {
            return Mathf.Ceil(endurance * 2.25f) + 100;
        }

        #endregion

        #region Consumables
        private void ManageConsumablesOverTime()
        {
            if (this.appliedConsumables.Count > 0)
            {
                foreach (AppliedConsumable appliedConsumable in this.appliedConsumables)
                {
                    Consumable consumable = appliedConsumable.consumable;
                    appliedConsumable.currentDuration -= Time.deltaTime;

                    if (appliedConsumable.currentDuration <= 0f)
                    {
                        RemoveAppliedConsumable(appliedConsumable);
                    }
                }
            }
        }

        public void AddConsumable(Consumable consumable)
        {
            AppliedConsumable appliedConsumable = new AppliedConsumable();
            appliedConsumable.currentDuration = consumable.effectDuration;
            appliedConsumable.consumable = consumable;


            if (consumable.action == Action.Increase)
            {
                if (consumable.attribute == Attribute.Arcane)
                {
                    PlayerStatsManager.instance.arcane += (int)consumable.value;
                    return;
                }
                if (consumable.attribute == Attribute.Dexterity)
                {
                    PlayerStatsManager.instance.dexterity += (int)consumable.value;
                    return;
                }
                if (consumable.attribute == Attribute.Endurance)
                {
                    PlayerStatsManager.instance.endurance += (int)consumable.value;
                    return;
                }
                if (consumable.attribute == Attribute.Intelligence)
                {
                    PlayerStatsManager.instance.intelligence += (int)consumable.value;
                    return;
                }
                if (consumable.attribute == Attribute.Strength)
                {
                    PlayerStatsManager.instance.strength += (int)consumable.value;
                    return;
                }
                if (consumable.attribute == Attribute.Vitality)
                {
                    PlayerStatsManager.instance.vitality += (int)consumable.value;
                    return;
                }
            }

            this.appliedConsumables.Add(appliedConsumable);
        }

        private void RemoveAppliedConsumable(AppliedConsumable appliedConsumable)
        {
            this.appliedConsumables.Remove(appliedConsumable);

            Consumable consumable = appliedConsumable.consumable;

            if (consumable.action == Action.Increase)
            {
                if (consumable.attribute == Attribute.Arcane)
                {
                    PlayerStatsManager.instance.arcane -= (int)consumable.value;
                    return;
                }
                if (consumable.attribute == Attribute.Dexterity)
                {
                    PlayerStatsManager.instance.dexterity -= (int)consumable.value;
                    return;
                }
                if (consumable.attribute == Attribute.Endurance)
                {
                    PlayerStatsManager.instance.endurance -= (int)consumable.value;
                    return;
                }
                if (consumable.attribute == Attribute.Intelligence)
                {
                    PlayerStatsManager.instance.intelligence -= (int)consumable.value;
                    return;
                }
                if (consumable.attribute == Attribute.Strength)
                {
                    PlayerStatsManager.instance.strength -= (int)consumable.value;
                    return;
                }
                if (consumable.attribute == Attribute.Vitality)
                {
                    PlayerStatsManager.instance.vitality -= (int)consumable.value;
                    return;
                }
            }
        }

        #endregion

        #region Status Effect
        public void UpdateStatusEffect(StatusEffect statusEffect, float statusAmountToAdd)
        {
            UIDocumentStatusEffect uIDocumentStatusEffect = FindObjectOfType<UIDocumentStatusEffect>(true);

            var idx = this.appliedStatus.FindIndex(appliedStatus => appliedStatus.statusEffect == statusEffect);

            if (idx != -1)
            {
                // Don't do nothing right now because the status has already been applied, no need to add to it
                if (this.appliedStatus[idx].hasReachedTotalAmount)
                {
                    return;
                }

                this.appliedStatus[idx].currentAmount = Mathf.Clamp(this.appliedStatus[idx].currentAmount + statusAmountToAdd, 0, this.appliedStatus[idx].statusEffect.maxAmountBeforeDamage);

                // If amount of status reached 100%, damage player and mark the effect as temporarily permanent
                if (
                    this.appliedStatus[idx].currentAmount >= statusEffect.maxAmountBeforeDamage
                    && this.appliedStatus[idx].hasReachedTotalAmount == false)
                {
                    this.appliedStatus[idx].hasReachedTotalAmount = true;

                    Instantiate(statusEffect.particleOnDamage, FindObjectOfType<Player>(true).transform);
                    return;
                }

                return;
            }

            AppliedStatus appliedStatus = new AppliedStatus();
            appliedStatus.statusEffect = statusEffect;
            appliedStatus.currentAmount += statusAmountToAdd;

            this.appliedStatus.Add(appliedStatus);

            if (uIDocumentStatusEffect != null)
            {
                uIDocumentStatusEffect.AddStatusEntry(statusEffect);
            }
        }

        public void RemoveStatusEffect(StatusEffect statusEffect)
        {
            var idx = this.appliedStatus.FindIndex(appliedStatus => appliedStatus.statusEffect == statusEffect);

            if (idx != -1)
            {
                this.appliedStatus.RemoveAt(idx);

                UIDocumentStatusEffect uIDocumentStatusEffect = FindObjectOfType<UIDocumentStatusEffect>(true);
                uIDocumentStatusEffect.RemoveStatusEntry(statusEffect);
            }
        }

        private void HandleAppliedStatusEffect()
        {
            if (this.appliedStatus.Count <= 0)
            {
                return;
            }

            UIDocumentStatusEffect uiDocumentStatusEffect = FindObjectOfType<UIDocumentStatusEffect>(true);

            List<AppliedStatus> statusToDelete = new List<AppliedStatus>();

            foreach (var entry in this.appliedStatus)
            {
                entry.currentAmount -= (entry.hasReachedTotalAmount
                    ? entry.statusEffect.decreaseRateWithDamage
                    : entry.statusEffect.decreaseRateWithoutDamage) * Time.deltaTime;

                uiDocumentStatusEffect.UpdateStatusEntry(entry.statusEffect, entry.currentAmount, entry.hasReachedTotalAmount);

                if (entry.hasReachedTotalAmount)
                {
                    if (entry.statusEffect.statAffected == Stat.Health)
                    {
                        SetCurrentHealth(
                            this.currentHealth - entry.statusEffect.damagePerSecond * Time.deltaTime
                        );

                        if (this.currentHealth <= 0)
                        {
                            FindObjectOfType<Player>(true).GetComponentInChildren<Healthbox>().DieWithSound();
                            
                            UIDocumentGameOverScreen uIDocumentGameOverScreen = FindObjectOfType<UIDocumentGameOverScreen>(true);
                            uIDocumentGameOverScreen.ShowGameOverScreen();


                            // Remove this status
                            statusToDelete.Add(entry);
                            uiDocumentStatusEffect.RemoveStatusEntry(entry.statusEffect);
                        }
                    }
                    else if (entry.statusEffect.statAffected == Stat.Stamina)
                    {
                        DecreaseStamina(entry.statusEffect.damagePerSecond);
                    }
                }

                if (entry.currentAmount <= 0)
                {
                    uiDocumentStatusEffect.RemoveStatusEntry(entry.statusEffect);

                    statusToDelete.Add(entry);
                }
            }

            foreach (var status in statusToDelete)
            {
                this.appliedStatus.Remove(status);
            }
        }

        public StatusEffect GetStatusEffectByName(string statusEffectName)
        {
            return statusEffectsDatabase.Find(statusEffect => statusEffect.name == statusEffectName);
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

            this.gold = playerData.gold;

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

            if (this.currentStamina < GetMaxStaminaPoints())
            {
                DecreaseStamina(0);
            }

            this.appliedConsumables.Clear();
            foreach (var serializedConsumable in gameData.consumables)
            {
                var consumable = PlayerInventoryManager.instance.GetItem(serializedConsumable.consumableName);

                AppliedConsumable appliedConsumable = new AppliedConsumable();
                appliedConsumable.consumable = (Consumable)consumable;
                appliedConsumable.currentDuration = serializedConsumable.currentDuration;

                this.appliedConsumables.Add(appliedConsumable);
            }

            this.appliedStatus.Clear();
            UIDocumentStatusEffect uIDocumentStatusEffect = FindObjectOfType<UIDocumentStatusEffect>(true);
            if (uIDocumentStatusEffect != null)
            {
                uIDocumentStatusEffect.ClearAllStatusEntry();
            }
            foreach (var serializedStatusEffects in gameData.statusEffects)
            {
                var statusEffect = GetStatusEffectByName(serializedStatusEffects.statusEffectName);

                UpdateStatusEffect(statusEffect, serializedStatusEffects.currentAmount);

                this.appliedStatus.Find(x => x.statusEffect == statusEffect).hasReachedTotalAmount = serializedStatusEffects.hasReachedTotalAmount;
            }
        }
        #endregion
    }
}
