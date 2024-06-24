using System.Numerics;
using AF.Combat;
using AF.Companions;
using AF.Health;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{

    public class DamageReceiver : MonoBehaviour
    {
        public readonly int hashBackstabExecuted = Animator.StringToHash("AI Humanoid - Backstabbed");

        [Header("Character")]
        public CharacterBaseManager character;
        [Range(0, 1f)] public float pushForceAbsorption = 1;
        public bool canBeBackstabbed = true;

        [Header("Components")]
        public DamageResistances damageResistances;
        public CharacterBaseHealth health;
        public CombatNotificationsController combatNotificationsController;

        [Header("Unity Events")]
        public UnityEvent onDamageReceived;
        public UnityEvent onPhysicalDamage;
        public UnityEvent onFireDamage;
        public UnityEvent onFrostDamage;
        public UnityEvent onMagicDamage;
        public UnityEvent onLightningDamage;
        public UnityEvent onDarknessDamage;
        public UnityEvent onBackstabbed;
        public UnityEvent onAttackedWhileWithFlatulence;


        [Header("Flags")]
        public bool canTakeDamage = true;
        public bool damageOnDodge = false;
        public bool waitingForBackstab = false;
        public bool hasFlatulence = false;


        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void SetHasFlatulence(bool value)
        {
            this.hasFlatulence = value;
        }

        public void ResetStates()
        {
            canTakeDamage = true;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void SetCanTakeDamage(bool value)
        {
            canTakeDamage = value;
        }

        bool CanTakeDamage()
        {
            if (!canTakeDamage)
            {
                return false;
            }

            if (hasFlatulence)
            {
                return false;
            }

            // If is an object
            if (character == null)
            {
                return true;
            }

            if (health.GetCurrentHealth() <= 0)
            {
                return false;
            }

            if (character is PlayerManager player && player.climbController.climbState != Ladders.ClimbState.NONE)
            {
                return false;
            }

            return true;
        }


        public void HandleIncomingDamage(CharacterBaseManager damageOwner, UnityAction<Damage> onTakeDamage, bool ignoreSameFaction)
        {
            // Don't allow same factions to hit each other
            if (!ignoreSameFaction && damageOwner.IsFromSameFaction(character))
            {
                return;
            }

            if (hasFlatulence)
            {
                onAttackedWhileWithFlatulence?.Invoke();
            }

            if (!CanTakeDamage())
            {
                return;
            }

            Damage incomingDamage = damageOwner.GetAttackDamage();

            if (character != null)
            {
                if (character is CharacterManager aiCharacter)
                {
                    if (aiCharacter.targetManager != null)
                    {
                        aiCharacter.targetManager.SetTarget(damageOwner);
                    }

                    // On Damage Taken, Stop Rotation
                    aiCharacter.ResetFaceTargetFlag();
                }

                if (character.characterPosture.isStunned && waitingForBackstab == false)
                {
                    character.characterPosture.RecoverFromStunned();
                }

                if (character.characterBlockController.CanParry(incomingDamage))
                {
                    (character as CharacterManager)?.FaceTarget();

                    character.characterBlockController.HandleParryEvent();
                    damageOwner.characterBlockController.HandleParriedEvent(character.characterBlockController.GetPostureDamageFromParry());
                    return;
                }

                if (character.characterBlockController.CanBlockDamage(incomingDamage) && HandleBlocking(incomingDamage, damageOwner))
                {
                    return;
                }

                HandlePlayerArmorAttacks(damageOwner);

                HandlePlayerRage();

                HandlePlayerHealthBack(damageOwner);
            }

            ApplyDamage(incomingDamage);

            onTakeDamage?.Invoke(incomingDamage);
        }

        bool HandleBlocking(Damage incomingDamage, CharacterBaseManager damageOwner)
        {
            if (character is PlayerManager playerManager)
            {
                if (playerManager.staminaStatManager.CanPerformAction(playerManager.playerWeaponsManager.GetCurrentBlockStaminaCost()))
                {
                    incomingDamage = playerManager.playerWeaponsManager.GetCurrentShieldDefenseAbsorption(incomingDamage);

                    if (damageOwner != null && damageOwner is CharacterManager enemy)
                    {
                        playerManager.playerWeaponsManager.ApplyShieldDamageToAttacker(enemy);
                    }

                    playerManager.staminaStatManager.DecreaseStamina((int)playerManager.playerWeaponsManager.GetCurrentBlockStaminaCost());
                    character.characterBlockController.BlockAttack(incomingDamage);

                    if (playerManager.characterBlockController is PlayerBlockController playerBlockController)
                    {
                        playerBlockController.SetCanCounterAttack(true);
                    }
                }
            }
            else
            {
                character.characterBlockController.BlockAttack(incomingDamage);
                return true;
            }

            return false;
        }

        void HandlePlayerArmorAttacks(CharacterBaseManager damageOwner)
        {
            if (character is not PlayerManager playerManager)
            {
                return;
            }

            CharacterManager enemy = damageOwner as CharacterManager;
            if (enemy == null)
            {
                return;
            }

            if (playerManager.equipmentDatabase.helmet != null && playerManager.equipmentDatabase.helmet.canDamageEnemiesUponAttack)
            {
                playerManager.equipmentDatabase.helmet.AttackEnemy(enemy);
            }
            if (playerManager.equipmentDatabase.armor != null && playerManager.equipmentDatabase.armor.canDamageEnemiesUponAttack)
            {
                playerManager.equipmentDatabase.armor.AttackEnemy(enemy);
            }
            if (playerManager.equipmentDatabase.gauntlet != null && playerManager.equipmentDatabase.gauntlet.canDamageEnemiesUponAttack)
            {
                playerManager.equipmentDatabase.gauntlet.AttackEnemy(enemy);
            }
            if (playerManager.equipmentDatabase.legwear != null && playerManager.equipmentDatabase.legwear.canDamageEnemiesUponAttack)
            {
                playerManager.equipmentDatabase.legwear.AttackEnemy(enemy);
            }
        }



        void HandlePlayerRage()
        {
            if (character is not PlayerManager playerManager)
            {
                return;
            }

            playerManager.rageManager.IncrementRage();
        }


        void HandlePlayerHealthBack(CharacterBaseManager damageOwner)
        {
            if (damageOwner is PlayerManager playerManager)
            {
                if (
                    playerManager.playerWeaponsManager?.currentWeaponInstance != null
                    && playerManager.playerWeaponsManager?.currentWeaponInstance?.weapon != null
                    && playerManager.playerWeaponsManager?.currentWeaponInstance?.weapon?.healthRestoredWithEachHit > 0)
                {
                    playerManager.health.RestoreHealth(playerManager.playerWeaponsManager.currentWeaponInstance.weapon.healthRestoredWithEachHit);
                }
            }
        }



        /// <summary>
        /// Unity Event
        /// 
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(Damage damage)
        {

            if (hasFlatulence)
            {
                onAttackedWhileWithFlatulence?.Invoke();
            }

            if (!CanTakeDamage())
            {
                return;
            }

            ApplyDamage(damage);
        }

        /// <summary>
        /// Unity Event
        /// Bypass the CanTakeDamage check
        /// </summary>
        /// <param name="damage"></param>
        public void ApplyDamage(Damage damage)
        {
            if (!waitingForBackstab && damage.pushForce > 0 && character != null && character.characterPushController != null)
            {
                var targetPos = character.transform.position - Camera.main.transform.position;
                targetPos.y = 0;
                character.characterPushController.ApplyForceSmoothly(
                    targetPos.normalized,
                    Mathf.Clamp(damage.pushForce * pushForceAbsorption, 0, Mathf.Infinity) * 10,
                    .25f);
            }

            if (damageResistances != null)
            {
                damage = damageResistances.FilterIncomingDamage(damage);
            }

            if (character != null && character is PlayerManager player)
            {
                damage = player.playerWeaponsManager.GetCurrentShieldPassiveDamageFilter(damage);
            }

            bool damageFromBackstab = false;
            bool damageFromPostureBroken = false;
            if (character != null)
            {
                if (health.GetCurrentHealth() <= 0)
                {
                    return;
                }

                if (waitingForBackstab)
                {
                    waitingForBackstab = false;

                    character.PlayBusyHashedAnimationWithRootMotion(hashBackstabExecuted);
                    health.TakeDamage(GetTotalDamage(damage, true));
                    onBackstabbed?.Invoke();
                    damageFromBackstab = true;
                }
                else
                {
                    bool isPostureBroken = character.characterPosture.TakePostureDamage(damage.postureDamage);
                    health.TakeDamage(GetTotalDamage(damage, isPostureBroken));
                    character.characterPoise.TakePoiseDamage(damage.poiseDamage);

                    if (isPostureBroken)
                    {
                        damageFromPostureBroken = true;
                    }
                }

                if (character.statusController != null && damage.statusEffects != null && damage.statusEffects.Length > 0)
                {
                    foreach (var statusEffectToApply in damage.statusEffects)
                    {
                        character.statusController.InflictStatusEffect(statusEffectToApply.statusEffect, statusEffectToApply.amountPerHit, false);
                    }
                }
            }

            if (damage.physical > 0)
            {
                if (combatNotificationsController != null)
                {
                    if (damageFromPostureBroken)
                    {
                        combatNotificationsController.ShowPostureBroken(damage.physical);
                    }
                    else if (damage.damageType == DamageType.COUNTER_ATTACK)
                    {
                        combatNotificationsController.ShowGuardCounter(damage.physical);
                    }
                    else if (damage.damageType == DamageType.ENRAGED)
                    {
                        combatNotificationsController.ShowRageCounter(damage.physical);
                    }
                    else if (damageFromBackstab)
                    {
                        combatNotificationsController.ShowBackstab(damage.physical);
                    }
                    else
                    {
                        combatNotificationsController.ShowDamage(damage.physical);
                    }
                }

                onPhysicalDamage?.Invoke();
            }
            if (damage.fire > 0)
            {
                if (combatNotificationsController != null)
                {
                    combatNotificationsController.ShowFireDamage(damage.fire);
                }

                onFireDamage?.Invoke();
            }
            if (damage.frost > 0)
            {
                if (combatNotificationsController != null)
                {
                    combatNotificationsController.ShowFrostDamage(damage.frost);
                }

                onFrostDamage?.Invoke();
            }
            if (damage.magic > 0)
            {
                if (combatNotificationsController != null)
                {
                    combatNotificationsController.ShowMagicDamage(damage.magic);
                }

                onMagicDamage?.Invoke();
            }
            if (damage.lightning > 0)
            {
                if (combatNotificationsController != null)
                {
                    combatNotificationsController.ShowLightningDamage(damage.lightning);
                }

                onLightningDamage?.Invoke();
            }
            if (damage.darkness > 0)
            {
                if (combatNotificationsController != null)
                {
                    combatNotificationsController.ShowDarknessDamage(damage.darkness);
                }

                onDarknessDamage?.Invoke();
            }

            onDamageReceived?.Invoke();
        }

        public void TakeDamagePercentage(float damagePercentage)
        {
            int damageAmount = (int)damagePercentage * health.GetMaxHealth() / 100;

            ApplyDamage(
                new(
                    physical: damageAmount,
                    fire: 0,
                    frost: 0,
                    magic: 0,
                    lightning: 0,
                    darkness: 0,
                    poiseDamage: 1,
                    postureDamage: 2,
                    weaponAttackType: WeaponAttackType.Slash,
                    statusEffects: null,
                    pushForce: 0,
                    canNotBeParried: false,
                    ignoreBlocking: false));
        }

        int GetTotalDamage(Damage damage, bool isPostureBroken)
        {
            if (isPostureBroken)
            {
                damage.physical = (int)(damage.physical * character.characterPosture.postureBreakBonusMultiplier);
            }

            return (int)(damage.physical + damage.fire
                + damage.frost + damage.magic + damage.lightning + damage.darkness);
        }
    }
}
