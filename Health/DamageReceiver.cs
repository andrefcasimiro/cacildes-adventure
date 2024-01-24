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
        [Header("Character")]
        public CharacterBaseManager character;
        [Range(0, 1f)] public float pushForceAbsorption = 1;

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


        [Header("Flags")]
        public bool canTakeDamage = true;
        public bool damageOnDodge = false;

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

            // If is an object
            if (character == null)
            {
                return true;
            }

            if (health.GetCurrentHealth() <= 0)
            {
                return false;
            }

            return true;
        }

        public void HandleIncomingDamage(CharacterBaseManager damageOwner, UnityAction onTakeDamage)
        {
            // Don't allow same factions to hit each other
            if (damageOwner?.characterFaction == character?.characterFaction)
            {
                return;
            }

            if (!CanTakeDamage())
            {
                return;
            }

            Damage incomingDamage = damageOwner.GetAttackDamage();

            if (character != null)
            {
                if (character is CharacterManager aiCharacter && aiCharacter.targetManager != null)
                {
                    aiCharacter.targetManager.SetTarget(damageOwner);
                }



                if (character.characterPosture.isStunned)
                {
                    character.characterPosture.RecoverFromStunned();
                }

                if (character.characterBlockController.IsWithinParryingWindow())
                {
                    Utils.FaceTarget(character.transform, damageOwner.transform);
                    Utils.FaceTarget(damageOwner.transform, character.transform);

                    character.characterBlockController.HandleParryEvent();
                    damageOwner.characterBlockController.HandleParriedEvent();
                    return;
                }

                if (incomingDamage.pushForce > 0 && character.characterPushController != null)
                {
                    character.characterPushController.ApplyForceSmoothly(
                        damageOwner.transform.forward,
                        Mathf.Clamp(incomingDamage.pushForce * pushForceAbsorption, 0, Mathf.Infinity) * 10,
                        .25f);
                }

                if (character.characterBlockController.CanBlockDamage(incomingDamage))
                {
                    Utils.FaceTarget(character.transform, damageOwner.transform);
                    Utils.FaceTarget(damageOwner.transform, character.transform);

                    character.characterBlockController.BlockAttack(incomingDamage);
                    return;
                }
            }

            TakeDamage(incomingDamage);

            onTakeDamage?.Invoke();
        }

        /// <summary>
        /// Unity Event
        /// 
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(Damage damage)
        {
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
            if (damageResistances != null)
            {
                damage = damageResistances.FilterIncomingDamage(damage);
            }

            if (character != null)
            {
                if (health.GetCurrentHealth() <= 0)
                {
                    return;
                }


                bool isPostureBroken = character.characterPosture.TakePostureDamage(damage.postureDamage);
                health.TakeDamage(GetTotalDamage(damage, isPostureBroken));

                character.characterPoise.TakePoiseDamage(damage.poiseDamage);

                if (character.statusController != null && damage.statusEffect != null)
                {
                    character.statusController.InflictStatusEffect(damage.statusEffect, damage.statusEffectAmount, false);
                }
            }

            if (damage.physical > 0)
            {
                if (combatNotificationsController != null)
                {
                    combatNotificationsController.ShowDamage(damage.physical);
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
                    poiseDamage: 1,
                    postureDamage: 2,
                    weaponAttackType: WeaponAttackType.Slash,
                    statusEffect: null,
                    statusEffectAmount: 0,
                    pushForce: 0));
        }

        int GetTotalDamage(Damage damage, bool isPostureBroken)
        {
            if (isPostureBroken)
            {
                damage.physical = (int)(damage.physical * character.characterPosture.postureBreakBonusMultiplier);
            }

            return (int)(damage.physical + damage.fire
                + damage.frost + damage.magic + damage.lightning);
        }
    }
}
