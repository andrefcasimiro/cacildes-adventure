
using AF.Shooting;
using AF.Stats;
using UnityEngine;

namespace AF
{
    public class DestroyableSpellParticle : DestroyableParticle, IProjectile
    {
        public Spell spell;
        [HideInInspector] public StatsBonusController statsBonusController;

        public bool collideOnlyOnce = true;

        public float timeBetweenDamage = Mathf.Infinity;
        public float maxTimeBetweenDamage = 0.1f;

        public int pushForce = 0;
        public bool isPullForce = false;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        public Rigidbody rigidBody;

        [Header("Projectile Options")]

        public ForceMode forceMode;
        public float forwardVelocity = 10f;

        private void OnParticleCollision(GameObject other)
        {
            OnCollide(other);
        }

        public void Shoot(CharacterBaseManager shooter, Vector3 aimForce, ForceMode forceMode)
        {
            rigidBody.AddForce(aimForce, forceMode);
        }

        public void OnCollide(GameObject other)
        {

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
