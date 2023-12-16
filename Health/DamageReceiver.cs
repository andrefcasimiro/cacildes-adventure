using AF.Health;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{

    public class DamageReceiver : MonoBehaviour
    {
        [Header("Character")]
        public CharacterBaseManager character;

        [Header("Components")]
        public DamageResistances damageResistances;
        public CharacterBaseHealth health;
        public CombatNotificationsController combatNotificationsController;

        [Header("Unity Events")]
        public UnityEvent onPhysicalDamage;
        public UnityEvent onFireDamage;
        public UnityEvent onFrostDamage;
        public UnityEvent onMagicDamage;
        public UnityEvent onLightningDamage;

        [Header("Flags")]
        public bool canTakeDamage = true;
        public bool damageOnDodge = false;

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

            if (character.characterPosture.IsStunned())
            {
                return false;
            }

            return true;
        }

        public void HandleIncomingDamage(CharacterBaseManager damageOwner, UnityAction onTakeDamage)
        {
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

                if (character.characterBlockController.CanBlockDamage(incomingDamage))
                {
                    character.characterBlockController.BlockAttack(incomingDamage);
                    return;
                }
            }

            TakeDamage(incomingDamage);

            onTakeDamage?.Invoke();
        }

        public void TakeDamage(Damage damage)
        {
            if (!CanTakeDamage())
            {
                return;
            }

            if (damageResistances != null)
            {
                damage = damageResistances.FilterIncomingDamage(damage);
            }

            if (character != null)
            {
                health?.TakeDamage(GetTotalDamage(damage));
                if (health?.GetCurrentHealth() <= 0)
                {
                    return;
                }

                // Only take poise damage if posture is not going to break
                if (!character.characterPosture.WillBreakPosture(damage))
                {
                    character.characterPoise.TakePoiseDamage(damage.poiseDamage);
                }

                character.characterPosture.TakePostureDamage(damage.postureDamage);
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

        }

        int GetTotalDamage(Damage damage)
        {
            if (character != null && character.characterPosture.WillBreakPosture(damage))
            {
                damage.physical = (int)(damage.physical * character.characterPosture.postureBreakBonusMultiplier);
            }

            return (int)(damage.physical + damage.fire
                + damage.frost + damage.magic + damage.lightning);
        }
    }
}
