using System.Collections;
using AF.Health;
using AF.Shooting;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class Projectile : MonoBehaviour, IProjectile
    {
        [Header("Lifespan")]
        public float timeBeforeDestroying = 10;

        [Header("Velocity")]
        public ForceMode forceMode;
        public float forwardVelocity = 10f;

        [Header("Stats")]
        public Damage damage;
        public bool scaleWithIntelligence = false;

        [Header("Status Effects")]
        public StatusEffect statusEffectToApply;
        public int amountOfStatusEffectToApply;

        public Rigidbody rigidBody;

        [Header("Events")]

        [Tooltip("Fires immediately after instatied")] public UnityEvent onFired;
        [Tooltip("Fires after 0.1ms")] public UnityEvent onFired_After;
        public UnityEvent onCollision;
        public float onFired_AfterDelay = 0.1f;

        // Flags
        bool hasCollided = false;

        CharacterBaseManager shooter;

        private void OnEnable()
        {
            onFired?.Invoke();

            StartCoroutine(HandleOnFiredAfter_Coroutine());
        }

        IEnumerator HandleOnFiredAfter_Coroutine()
        {
            yield return new WaitForSeconds(onFired_AfterDelay);
            onFired_After?.Invoke();
        }

        public void Shoot(CharacterBaseManager shooter, Vector3 aimForce, ForceMode forceMode)
        {
            this.shooter = shooter;

            rigidBody.AddForce(aimForce, forceMode);
        }

        void OnTriggerEnter(Collider other)
        {
            if (hasCollided)
            {
                return;
            }

            other.TryGetComponent(out DamageReceiver damageReceiver);

            HandleCollision(damageReceiver);
        }

        public void HandleCollision(DamageReceiver damageReceiver)
        {
            if (damageReceiver == null || damageReceiver?.character == shooter)
            {
                return;
            }

            hasCollided = true;

            if (shooter is PlayerManager playerManager && scaleWithIntelligence)
            {
                damage = playerManager.attackStatManager.GetScaledSpellDamage(damage);
            }

            damageReceiver.TakeDamage(damage);

            if (shooter != null
                && damageReceiver?.character is CharacterManager characterManager
                && characterManager.targetManager != null)
            {
                characterManager.targetManager.SetTarget(shooter);
            }

            if (statusEffectToApply != null && damageReceiver?.character?.statusController != null)
            {
                damageReceiver.character.statusController.InflictStatusEffect(statusEffectToApply, amountOfStatusEffectToApply, false);
            }

            onCollision?.Invoke();

            StartCoroutine(HandleDestroy_Coroutine());
        }

        IEnumerator HandleDestroy_Coroutine()
        {
            yield return new WaitForSeconds(timeBeforeDestroying);
            Destroy(this.gameObject);
        }

        public float GetForwardVelocity()
        {
            return forwardVelocity;
        }

        public ForceMode GetForceMode()
        {
            return forceMode;
        }
    }
}
