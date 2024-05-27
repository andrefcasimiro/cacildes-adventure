using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using AF.Stats;
using AF.Health;
using System.Collections;
using UnityEngine.Events;

namespace AF
{
    public class AttackStatManager : MonoBehaviour
    {
        // RECOMMENDED ATTACK FORMULA:
        // STR LEVEL * levelMultiplier * weaponScaling
        // A weapon that has S scaling with a levelmultiplier of 3.25 produces:
        // 1 * 3.25 * 2.4 = 8
        // 4 * 3.25 * 2.4 = 31
        // 8 * 3.25 * 2.4 = 62
        // 16 * 3.25 * 2.4 = 125
        // This gives good values, similar to Dark Souls

        [Header("Scaling Multipliers")]
        public float E = 0.25f;
        public float D = 1f;
        public float C = 1.3f;
        public float B = 1.65f;
        public float A = 1.95f;
        public float S = 2.25f;

        private Dictionary<string, float> scalingDictionary = new();

        [Header("Status attack bonus")]
        [Tooltip("Increased by buffs like potions, or equipment like accessories")]
        public float physicalAttackBonus = 0f;

        [Header("Unarmed Attack Options")]
        public int unarmedLightAttackPostureDamage = 18;
        public int unarmedPostureDamageBonus = 10;

        [Header("Physical Attack")]
        public int basePhysicalAttack = 100;
        public float levelMultiplier = 2.55f;

        public float jumpAttackMultiplier = 3.25f;
        float twoHandAttackBonusMultiplier = 1.35f;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        [Header("Components")]
        public PlayerManager playerManager;

        [Header("Rage")]
        public Coroutine RageCoroutine;
        public int maxRage = 10;
        public float rageDelayInSecondsBeforeRemovingOneUnit = 3f;
        public int rageCount = 0;
        public float rageCountMultiplier = 25f;

        public UnityEvent onLowRage;
        public UnityEvent onMidRage;
        public UnityEvent onHighRage;

        bool shouldResetRage = false;

        private void Start()
        {
            scalingDictionary.Add("E", E);
            scalingDictionary.Add("D", D);
            scalingDictionary.Add("C", C);
            scalingDictionary.Add("B", B);
            scalingDictionary.Add("A", A);
            scalingDictionary.Add("S", S);
        }

        public void ResetStates()
        {
            CheckIfShouldResetRage();
        }

        public bool IsHeavyAttacking()
        {
            return playerManager.playerCombatController.isHeavyAttacking;
        }

        public bool IsJumpAttacking()
        {
            return playerManager.playerCombatController.isJumpAttacking;
        }

        public bool HasBowEquipped()
        {
            return equipmentDatabase.IsBowEquipped();
        }

        public Damage GetAttackDamage()
        {
            int rageBonus = 0;

            if (rageCount > 0)
            {
                rageBonus = (int)(rageCount * rageCountMultiplier);
                EvaluateRageAttack();
                shouldResetRage = true;
            }

            Weapon weapon = equipmentDatabase.GetCurrentWeapon();
            if (weapon != null)
            {
                Damage weaponDamage = new Damage(
                    physical: GetWeaponAttack(weapon) + rageBonus,
                    fire: (int)weapon.damage.fire,
                    frost: (int)weapon.damage.frost,
                    magic: (int)weapon.damage.magic,
                    lightning: (int)weapon.damage.lightning,
                    darkness: (int)weapon.damage.darkness,
                    postureDamage: (IsHeavyAttacking() || IsJumpAttacking())
                    ? weapon.damage.postureDamage + weapon.heavyAttackPostureDamageBonus
                    : weapon.damage.postureDamage,
                    poiseDamage: weapon.damage.poiseDamage,
                    weaponAttackType: weapon.damage.weaponAttackType,
                    statusEffects: weapon.damage.statusEffects,
                    pushForce: weapon.damage.pushForce,
                    canNotBeParried: weapon.damage.canNotBeParried,
                    ignoreBlocking: weapon.damage.ignoreBlocking
                );

                if (rageBonus > 0)
                {
                    weaponDamage.damageType = DamageType.ENRAGED;
                }

                return playerManager.playerWeaponsManager.GetBuffedDamage(weaponDamage);
            }


            Damage unarmedDamage = new Damage(
                physical: GetCurrentPhysicalAttack() + rageBonus,
                fire: 0,
                frost: 0,
                magic: 0,
                lightning: 0,
                darkness: 0,
                postureDamage: (IsHeavyAttacking() || IsJumpAttacking())
                    ? unarmedLightAttackPostureDamage + unarmedPostureDamageBonus
                    : unarmedLightAttackPostureDamage,
                poiseDamage: 1,
                weaponAttackType: WeaponAttackType.Blunt,
                statusEffects: null,
                pushForce: 0,
                canNotBeParried: false,
                ignoreBlocking: false
            );
            if (rageBonus > 0)
            {
                unarmedDamage.damageType = DamageType.ENRAGED;
            }

            return unarmedDamage;
        }

        void CheckIfShouldResetRage()
        {
            if (shouldResetRage)
            {
                ResetRage();
                shouldResetRage = false;
            }
        }

        public int GetCurrentPhysicalAttack()
        {
            int heavyAttackBonus = 0;
            if (equipmentDatabase.GetCurrentWeapon() == null && playerManager.playerCombatController.isHeavyAttacking)
            {
                heavyAttackBonus = playerManager.playerCombatController.unarmedHeavyAttackBonus;
            }

            var value = basePhysicalAttack;

            if (IsJumpAttacking())
            {
                value = Mathf.FloorToInt(value * jumpAttackMultiplier);
            }

            return (int)Mathf.Round(
                Mathf.Ceil(
                    value
                        + (playerStatsDatabase.strength * levelMultiplier)
                        + (playerStatsDatabase.dexterity * levelMultiplier)
                        + (playerManager.statsBonusController.strengthBonus * levelMultiplier)
                        + (playerManager.statsBonusController.dexterityBonus * levelMultiplier)
                    ) + physicalAttackBonus + heavyAttackBonus
                );
        }


        public int GetCurrentPhysicalAttackForGivenStrengthAndDexterity(int strength, int dexterity)
        {
            return (int)Mathf.Round(
                Mathf.Ceil(
                    basePhysicalAttack
                        + (strength * levelMultiplier)
                        + (dexterity * levelMultiplier)
                    )
                );
        }


        #region Weapon Attack
        public int CompareWeapon(Weapon weaponToCompare)
        {
            if (equipmentDatabase.GetCurrentWeapon() == null)
            {
                return 1;
            }

            var weaponToCompareAttack = GetWeaponAttack(weaponToCompare);
            var currentWeaponAttack = GetWeaponAttack(equipmentDatabase.GetCurrentWeapon());

            if (weaponToCompareAttack > currentWeaponAttack)
            {
                return 1;
            }

            if (weaponToCompareAttack == currentWeaponAttack)
            {
                return 0;
            }

            return -1;
        }

        public int GetWeaponAttack(Weapon weapon)
        {
            float strengthBonusFromWeapon = GetStrengthBonusFromWeapon(weapon);
            float dexterityBonus = GetDexterityBonusFromWeapon(weapon);
            float intelligenceBonus = GetIntelligenceBonusFromWeapon(weapon);

            var baseValue = HasBowEquipped() || weapon.damage.physical <= 0 ? 0 : GetCurrentPhysicalAttack();

            var value = (int)(
                baseValue + weapon.damage.physical <= 0 ? 0 : (
                +weapon.GetWeaponAttack()
                + GetStrengthBonusFromWeapon(weapon)
                + GetDexterityBonusFromWeapon(weapon)
                + GetIntelligenceBonusFromWeapon(weapon))
            );

            if (equipmentDatabase.isTwoHanding)
            {
                value = (int)(value * twoHandAttackBonusMultiplier) + (int)strengthBonusFromWeapon;
            }

            if (playerManager.playerCombatController.isHeavyAttacking)
            {
                int heavyAttackBonus = weapon.heavyAttackBonus;
                value += heavyAttackBonus;
            }

            if (IsJumpAttacking())
            {
                value = Mathf.FloorToInt(value * jumpAttackMultiplier);

                var jumpAttackBonuses = equipmentDatabase.accessories.Sum(x => x != null ? x.jumpAttackBonus : 0);
                value += jumpAttackBonuses;
            }

            if (weapon.halveDamage)
            {
                return (int)(value / 2);
            }

            if (equipmentDatabase.accessories.FirstOrDefault(x => x != null && x.increaseAttackPowerTheLowerTheReputation) != null)
            {
                if (playerStatsDatabase.GetCurrentReputation() < 0)
                {
                    int extraAttackPower = Mathf.Min(150, (int)(Mathf.Abs(playerStatsDatabase.GetCurrentReputation()) * 2.25f));

                    value += extraAttackPower;
                }
            }

            if (equipmentDatabase.accessories.FirstOrDefault(x => x != null && x.increaseAttackPowerWithLowerHealth) != null)
            {
                int extraAttackPower = (int)(value * (playerManager.health as PlayerHealth).GetExtraAttackBasedOnCurrentHealth());

                value += extraAttackPower;
            }

            if (playerManager.playerCombatController)
            {

                var attackBonuses = equipmentDatabase.accessories.Sum(x => x != null ? x.physicalAttackBonus : 0);
                value += attackBonuses;
            }

            if (playerManager.characterBlockController.IsWithinCounterAttackWindow())
            {
                value = (int)(value * playerManager.characterBlockController.counterAttackMultiplier);
            }

            return value;
        }
        #endregion

        #region Scaling

        public int GetStrengthBonusFromWeapon(Weapon weapon)
        {
            if (weapon.damage.physical <= 0)
            {
                return 0;
            }

            return (int)Mathf.Ceil((playerStatsDatabase.strength + playerManager.statsBonusController.strengthBonus)
            * levelMultiplier * scalingDictionary[weapon.strengthScaling.ToString()] / 2);
        }

        public float GetDexterityBonusFromWeapon(Weapon weapon)
        {
            if (weapon.damage.physical <= 0)
            {
                return 0;
            }

            return (int)Mathf.Ceil((playerStatsDatabase.dexterity + playerManager.statsBonusController.dexterityBonus)
                * levelMultiplier * scalingDictionary[weapon.dexterityScaling.ToString()] / 2);
        }

        public float GetIntelligenceBonusFromWeapon(Weapon weapon)
        {
            return (int)Mathf.Ceil((playerStatsDatabase.intelligence + playerManager.statsBonusController.intelligenceBonus)
                * levelMultiplier * scalingDictionary[weapon.intelligenceScaling.ToString()] / 2);
        }

        #endregion

        public void SetBonusPhysicalAttack(int value)
        {
            physicalAttackBonus = value;
        }

        public void ResetBonusPhysicalAttack()
        {
            physicalAttackBonus = 0f;
        }

        public void IncrementRage()
        {
            if (!CanRage())
            {
                return;
            }

            if (rageCount > maxRage)
            {
                return;
            }

            rageCount++;

            if (RageCoroutine != null)
            {
                StopCoroutine(RageCoroutine);
            }

            RageCoroutine = StartCoroutine(HandleRage_Coroutine());
        }


        IEnumerator HandleRage_Coroutine()
        {
            while (rageCount > 0)
            {
                yield return new WaitForSeconds(rageDelayInSecondsBeforeRemovingOneUnit);
                rageCount--;
            }

            if (rageCount <= 0) { rageCount = 0; }
        }

        bool CanRage()
        {
            if (playerManager.health.GetCurrentHealth() <= 0)
            {
                return false;
            }

            return playerManager.statsBonusController.canRage;
        }

        void ResetRage()
        {
            if (RageCoroutine != null)
            {
                StopCoroutine(RageCoroutine);
            }

            rageCount = 0;
        }

        void EvaluateRageAttack()
        {
            if (rageCount <= 0)
            {
                return;
            }

            if (rageCount <= 1)
            {
                onLowRage?.Invoke();
            }
            else if (rageCount <= 3)
            {
                onMidRage?.Invoke();
            }
            else
            {
                onHighRage?.Invoke();
            }
        }
    }
}
