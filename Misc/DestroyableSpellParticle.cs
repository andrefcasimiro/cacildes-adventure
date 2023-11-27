
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

        public void Shoot(Transform target, Vector3 aimForce, ForceMode forceMode)
        {
            if (target == null)
            {
                // Apply forces to the object
                if (rigidBody != null)
                {
                    // Apply force to move the arrow towards the center of the screen
                    rigidBody.AddForce(aimForce, forceMode);
                    //source = GetComponent<Cinemachine.CinemachineImpulseSource>();

                    //source.GenerateImpulse(Camera.main.transform.forward);
                }
                else
                {
                    Debug.LogError("Rigidbody component not found!");
                }

                return;
            }
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
