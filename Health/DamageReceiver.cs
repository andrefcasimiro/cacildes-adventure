using AF.Health;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{

    public class DamageReceiver : MonoBehaviour
    {
        [Header("Components")]
        public DamageResistances damageResistances;
        public CharacterHealth health;
        public CombatNotificationsController combatNotificationsController;

        [Header("Unity Events")]
        public UnityEvent onPhysicalDamage;
        public UnityEvent onFireDamage;
        public UnityEvent onFrostDamage;
        public UnityEvent onMagicDamage;
        public UnityEvent onLightningDamage;


        public void TakeDamage(Damage damage)
        {
            damage = damageResistances.FilterIncomingDamage(damage);

            health.TakeDamage(GetTotalDamage(damage));

            if (damage.physical > 0)
            {
                combatNotificationsController.ShowDamage(damage.physical);

                onPhysicalDamage?.Invoke();
            }
            if (damage.fire > 0)
            {
                combatNotificationsController.ShowFireDamage(damage.fire);

                onFireDamage?.Invoke();
            }
            if (damage.frost > 0)
            {
                combatNotificationsController.ShowFrostDamage(damage.frost);

                onFrostDamage?.Invoke();
            }
            if (damage.magic > 0)
            {
                combatNotificationsController.ShowMagicDamage(damage.magic);

                onMagicDamage?.Invoke();
            }
            if (damage.lightning > 0)
            {
                combatNotificationsController.ShowLightningDamage(damage.lightning);

                onLightningDamage?.Invoke();
            }

        }

        int GetTotalDamage(Damage damage)
        {
            return (int)(damage.physical + damage.fire
                + damage.frost + damage.magic + damage.lightning);
        }

    }
}
