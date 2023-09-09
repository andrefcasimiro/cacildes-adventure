using System.Runtime.Serialization.Json;
using UnityEngine;

namespace AF
{

    public class Projectile : MonoBehaviour
    {
        [Header("Lifespan")]
        public float flyingDuration = 2;
        public float timeBeforeDestroying = 10;
        float lifeTimer = 0f;

        [Header("Range")]
        public float maxRange = 10f;

        [Header("Velocity")]
        public ForceMode forceMode;
        public float forwardVelocity = 10f;
        public float upwardsVelocity = 10f;
        public bool forceTrajectoryTowardsPlayer = false;

        [Header("Stats")]
        public float projectileDamage = 50f;
        public int projectilePoiseDamage = 5;
        public WeaponElementType projectileAttackElementType;

        [Header("Status Effects")]
        public StatusEffect statusEffectToApply;
        public int amountOfStatusEffectToApply;

        [Header("Sound")]
        public AudioClip projectileImpactOnBodySfx;
        public AudioClip projectileImpactOnGroundSfx;
        public AudioClip throwSfx;

        [Header("FX")]
        public GameObject particleFx;
        public GameObject particleOnHitFx;

        Rigidbody rigidBody => GetComponent<Rigidbody>();
        Transform currentTarget;
        Vector3 targetDestination;

        public bool isFromPlayer = false;

        bool hasCollidedAlready = false;

        public bool destroyOnCollision = false;

        public bool useV2Collision = false;

        public float projectilePushForce = 2f;

        [Header("Triple Arrows Edge Case")]
        public bool useChildren = false;

        private void Start()
        {
            hasCollidedAlready = false;
        }

        public void Shoot(Transform target)
        {
            this.currentTarget = target;
            targetDestination = target.transform.position;

            transform.parent = null;

            Quaternion arrowRotation = Quaternion.LookRotation(targetDestination - transform.position);
            transform.rotation = arrowRotation;
            rigidBody.AddForce(this.transform.forward * forwardVelocity, forceMode);
            rigidBody.AddForce(this.transform.up * upwardsVelocity, forceMode);
        }

        public void ShootAndPreserveRotation(Transform target)
        {
            this.currentTarget = target;

            transform.parent = null;

            rigidBody.AddForce(this.transform.forward * forwardVelocity, forceMode);
            rigidBody.AddForce(this.transform.up * upwardsVelocity, forceMode);
        }

        public void Shoot(Vector3 position, bool directToEnemy)
        {
            targetDestination = position;

            transform.parent = null;

            Quaternion arrowRotation = Quaternion.LookRotation(targetDestination - transform.position);
            transform.rotation = arrowRotation;

            if (directToEnemy == false)
            {
                rigidBody.AddForce(this.transform.up * upwardsVelocity, forceMode);
            }

            rigidBody.AddForce(this.transform.forward * forwardVelocity, forceMode);

        }

        private void Update()
        {
            if (currentTarget == null)
            {
                return;
            }

            lifeTimer += Time.deltaTime;


            if (lifeTimer >= timeBeforeDestroying)
            {
                Destroy(this.gameObject);
            }
        }

        void HandleEnemyProjectile(Collider other)
        {
            if (hasCollidedAlready)
            {
                return;
            }

            var playerHealthbox = other.GetComponent<PlayerHealthbox>();
            if (playerHealthbox == null)
            {
                playerHealthbox = other.GetComponentInChildren<PlayerHealthbox>();
            }

            if (playerHealthbox == null)
            {
                return;
            }

            // We have a player to hit. Begin.

            // Show Impact FX
            if (particleOnHitFx != null && hasCollidedAlready == false)
            {
                Instantiate(particleOnHitFx, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
            }

            hasCollidedAlready = true;

            // Check if player is blocking
            var playerParrymanager = FindObjectOfType<PlayerParryManager>(true);
            if (playerParrymanager.IsBlocking() && Vector3.Angle(transform.forward, playerParrymanager.transform.forward * -1) <= 90f)
            {
                Instantiate(Player.instance.equippedShield.blockFx, other.transform.position, Quaternion.identity);

            }

            // Take damage
            playerHealthbox.TakeDamage(projectileDamage, this.transform, projectileImpactOnBodySfx, projectilePoiseDamage, projectileAttackElementType);

            // Check if projectile should be destroyed
            if (destroyOnCollision)
            {
                Destroy(this.gameObject);
            }
            else
            {
                this.enabled = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {


            if (other.CompareTag("Explodable"))
            {
                other.TryGetComponent(out ExplodingBarrel explodingBarrel);

                if (explodingBarrel != null)
                {
                    explodingBarrel.Explode();
                    return;
                }
            }

            if (other.CompareTag("Ignitable"))
            {
                other.TryGetComponent(out FireablePillar fireablePillar);

                if (fireablePillar != null)
                {
                    fireablePillar.Explode();
                    return;
                }
            }


            // The below code is already too messy, but many enemies use it, so use a feature flag to opt-in on new logic
            if (useV2Collision)
            {
                if (!isFromPlayer)
                {
                    HandleEnemyProjectile(other);
                }

                return;
            }

            if (other.CompareTag("Untagged"))
            {
                return;
            }

            if (hasCollidedAlready == true && isFromPlayer == false)
            {
                return;
            }

            if (other.gameObject.tag != "Enemy" && other.gameObject.tag != "EnemyHealthHitbox" && other.gameObject.tag != "" && isFromPlayer == false)
            {
                hasCollidedAlready = true;
            }

            if (particleFx != null)
            {
                particleFx.gameObject.SetActive(false);
            }

            if (particleOnHitFx != null && hasCollidedAlready == false)
            {
                Instantiate(particleOnHitFx, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
            }

            // Is Player ?
            if (isFromPlayer)
            {
                EnemyHealthHitbox enemyHealthHitbox = other.gameObject.GetComponentInChildren<EnemyHealthHitbox>(true);

                // Colliding with something else
                if (enemyHealthHitbox == null)
                {
                    if (projectileImpactOnBodySfx != null)
                    {
                        GetComponent<AudioSource>().PlayOneShot(projectileImpactOnBodySfx);
                    }

                    if (destroyOnCollision)
                    {
                        Destroy(this.gameObject);
                    }

                    return;
                }

                if (!hasCollidedAlready)
                {
                    enemyHealthHitbox.enemyManager.enemyHealthController
                        .TakeProjectileDamage(projectileDamage + FindObjectOfType<AttackStatManager>(true).GetArrowDamageBonus(), this);

                    enemyHealthHitbox.enemyManager.PushEnemy(projectilePushForce, ForceMode.Impulse);
                }

                if (statusEffectToApply != null && enemyHealthHitbox.enemyManager.enemyNegativeStatusController != null)
                {
                    enemyHealthHitbox.enemyManager.enemyNegativeStatusController.InflictStatusEffect(statusEffectToApply, amountOfStatusEffectToApply);
                }

                hasCollidedAlready = true;

                if (destroyOnCollision)
                {
                    Destroy(this.gameObject);
                }

                return;
            }

            PlayerHealthbox playerHealthbox = null;

            if (other.gameObject.tag == "PlayerHealthbox")
            {
                playerHealthbox = other.gameObject.GetComponent<PlayerHealthbox>();
            }
            else if (other.gameObject.tag == "Player")
            {
                playerHealthbox = other.gameObject.GetComponentInChildren<PlayerHealthbox>(true);
            }

            if (playerHealthbox != null)
            {
                var playerParrymanager = FindObjectOfType<PlayerParryManager>(true);

                if (playerParrymanager.IsBlocking() && Vector3.Angle(transform.forward, playerParrymanager.transform.forward * -1) <= 90f)
                {
                    Instantiate(Player.instance.equippedShield.blockFx, other.transform.position, Quaternion.identity);

                    if (destroyOnCollision)
                    {
                        Destroy(this.gameObject);
                    }

                    return;
                }

                playerHealthbox.TakeDamage(projectileDamage, this.transform, projectileImpactOnBodySfx, projectilePoiseDamage, projectileAttackElementType);
            }

            if (destroyOnCollision && hasCollidedAlready)
            {
                Destroy(this.gameObject);
            }

        }
    }
}
