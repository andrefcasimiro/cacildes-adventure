using System.Collections;
using System.Collections.Generic;
using AF.Health;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class OnDamageCollisionAbstractManager : MonoBehaviour
    {
        [Header("Projectile Settings")]
        public Projectile projectile;

        [Header("Damage Settings")]
        public Damage damage;
        List<DamageReceiver> damageReceivers = new();
        Coroutine ResetDamageReceiversCoroutine;
        public CharacterBaseManager damageOwner;
        public float damageCooldown = 1f;

        [Header("Events")]
        public UnityEvent onParticleDamage;

        public void OnCollision(GameObject other)
        {
            if (!other.TryGetComponent<DamageReceiver>(out var damageReceiver))
            {
                return;
            }

            if (damageOwner != null && damageOwner.damageReceiver == damageReceiver)
            {
                return;
            }

            if (damageReceivers.Contains(damageReceiver))
            {
                return;
            }

            damageReceivers.Add(damageReceiver);

            if (projectile != null)
            {
                projectile.HandleCollision(damageReceiver);
            }
            else if (damage != null)
            {
                damageReceiver.TakeDamage(damage);
            }

            onParticleDamage?.Invoke();

            if (ResetDamageReceiversCoroutine != null)
            {
                StopCoroutine(ResetDamageReceiversCoroutine);
            }

            ResetDamageReceiversCoroutine = StartCoroutine(ResetDamageReceivers_Coroutine());
        }

        IEnumerator ResetDamageReceivers_Coroutine()
        {
            yield return new WaitForSeconds(damageCooldown);

            damageReceivers.Clear();
        }
    }
}
