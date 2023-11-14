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

        public int projectileAttackElement = 20;
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

        [Header("Fire traps edge cases")]
        public bool collideOnlyWithEnemies = false;

        [Header("Confetti Traps")]
        public bool isConfettiTrap = false;
        public StatusEffect[] possibleStatusEffects;

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
                // Instantiate(Player.instance.equippedShield.blockFx, other.transform.position, Quaternion.identity);
            }

            if (GamePreferences.instance.gameDifficulty == GamePreferences.GameDifficulty.EASY)
            {
                projectileDamage = (int)(projectileDamage / 3);
            }
            else if (GamePreferences.instance.gameDifficulty == GamePreferences.GameDifficulty.MEDIUM)
            {
                projectileDamage = (int)(projectileDamage / 2);
            }


            // Take damage
            playerHealthbox.TakeDamage(projectileDamage, this.transform, projectileImpactOnBodySfx, projectilePoiseDamage, projectileAttackElement, projectileAttackElementType);

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

        void EvaluateConfettiTrap(EnemyManager enemyManager)
        {
            var dice = Random.Range(0f, 100);
            // Elemental Damage
            if (dice <= 25)
            {
                // Elemental Sutff going on
                var randomElementDice = Random.Range(0, 1f);
                WeaponElementType randomElement = WeaponElementType.None;

                if (randomElementDice <= .25f)
                {
                    randomElement = WeaponElementType.Fire;
                }
                else if (randomElementDice <= 0.5f)
                {
                    randomElement = WeaponElementType.Frost;
                }
                else if (randomElementDice <= 0.75f)
                {
                    randomElement = WeaponElementType.Magic;
                }
                else
                {
                    randomElement = WeaponElementType.Lightning;
                }

                var elementalDefense = 1f;
                switch (randomElement)
                {
                    case WeaponElementType.Fire:
                        elementalDefense = enemyManager.enemy.fireDamageBonus * 25 / 100; // Convert to percentage
                        break;
                    case WeaponElementType.Frost:
                        elementalDefense = enemyManager.enemy.frostDamageBonus * 25 / 100; // Convert to percentage
                        break;
                    case WeaponElementType.Lightning:
                        elementalDefense = enemyManager.enemy.lightningDamageBonus * 25 / 100; // Convert to percentage
                        break;
                    case WeaponElementType.Magic:
                        elementalDefense = enemyManager.enemy.magicDamageBonus * 25 / 100; // Convert to percentage
                        break;
                }
                projectileDamage += (projectileDamage * elementalDefense);

                enemyManager.enemyHealthController
                    .TakeProjectileDamage(projectileDamage + FindObjectOfType<AttackStatManager>(true).GetArrowDamageBonus(), this);
            }
            else if (dice <= 50)
            {
                var statusEffectToApply = possibleStatusEffects[Random.Range(0, possibleStatusEffects.Length)];
                if (statusEffectToApply != null && enemyManager.enemyNegativeStatusController != null)
                {
                    enemyManager.enemyNegativeStatusController.InflictStatusEffect(statusEffectToApply, Random.Range(25f, 50f));
                }
            }
            else if (dice <= 75)
            {
                enemyManager.PushEnemy(Random.Range(2, 8), ForceMode.Impulse);
            }
            else if (dice <= 85)
            {
                enemyManager.enemyHealthController.RestoreHealthPercentage(100);
            }
            else if (dice <= 95)
            {
                enemyManager.enemyHealthController.TakeEnvironmentalDamage(9999999);
            }
            else
            {
                enemyManager.enemyHealthController.TakeEnvironmentalDamage(25);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (collideOnlyWithEnemies)
            {
                if (!other.gameObject.CompareTag("Enemy"))
                {
                    return;
                }

                EnemyManager enemyManager = other.gameObject.GetComponent<EnemyManager>();
                if (enemyManager == null)
                {
                    enemyManager = other.GetComponentInParent<EnemyManager>();
                }

                if (enemyManager == null)
                {
                    return;
                }

                if (!hasCollidedAlready)
                {

                    if (particleFx != null)
                    {
                        particleFx.gameObject.SetActive(false);
                    }

                    if (particleOnHitFx != null && hasCollidedAlready == false)
                    {
                        Instantiate(particleOnHitFx, other.ClosestPointOnBounds(transform.position), Quaternion.identity);
                    }


                    hasCollidedAlready = true;

                    if (isConfettiTrap)
                    {
                        EvaluateConfettiTrap(enemyManager);

                        Destroy(this.gameObject);
                        return;
                    }

                    // Apply elemental defense reduction based on weaponElementType
                    float elementalDefense = 0f;
                    switch (projectileAttackElementType)
                    {
                        case WeaponElementType.Fire:
                            elementalDefense = enemyManager.enemy.fireDamageBonus * 25 / 100; // Convert to percentage
                            break;
                        case WeaponElementType.Frost:
                            elementalDefense = enemyManager.enemy.frostDamageBonus * 25 / 100; // Convert to percentage
                            break;
                        case WeaponElementType.Lightning:
                            elementalDefense = enemyManager.enemy.lightningDamageBonus * 25 / 100; // Convert to percentage
                            break;
                        case WeaponElementType.Magic:
                            elementalDefense = enemyManager.enemy.magicDamageBonus * 25 / 100; // Convert to percentage
                            break;
                    }

                    projectileDamage += (projectileDamage * elementalDefense);


                    enemyManager.enemyHealthController
                        .TakeProjectileDamage(projectileDamage + FindObjectOfType<AttackStatManager>(true).GetArrowDamageBonus(), this);

                    enemyManager.PushEnemy(projectilePushForce, ForceMode.Impulse);

                    if (statusEffectToApply != null && enemyManager.enemyNegativeStatusController != null)
                    {
                        enemyManager.enemyNegativeStatusController.InflictStatusEffect(statusEffectToApply, amountOfStatusEffectToApply);
                    }
                }

                return;
            }


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
                    // Apply elemental defense reduction based on weaponElementType
                    float elementalDefense = 0f;
                    switch (projectileAttackElementType)
                    {
                        case WeaponElementType.Fire:
                            elementalDefense = enemyHealthHitbox.enemyManager.enemy.fireDamageBonus * 25 / 100; // Convert to percentage
                            break;
                        case WeaponElementType.Frost:
                            elementalDefense = enemyHealthHitbox.enemyManager.enemy.frostDamageBonus * 25 / 100; // Convert to percentage
                            break;
                        case WeaponElementType.Lightning:
                            elementalDefense = enemyHealthHitbox.enemyManager.enemy.lightningDamageBonus * 25 / 100; // Convert to percentage
                            break;
                        case WeaponElementType.Magic:
                            elementalDefense = enemyHealthHitbox.enemyManager.enemy.magicDamageBonus * 25 / 100; // Convert to percentage
                            break;
                    }

                    projectileDamage += (projectileDamage * elementalDefense);


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
                    // Instantiate(Player.instance.equippedShield.blockFx, other.transform.position, Quaternion.identity);

                    if (destroyOnCollision)
                    {
                        Destroy(this.gameObject);
                    }

                    return;
                }

                playerHealthbox.TakeDamage(projectileDamage, this.transform, projectileImpactOnBodySfx, projectilePoiseDamage, projectileAttackElement, projectileAttackElementType);
            }

            if (destroyOnCollision && hasCollidedAlready)
            {
                Destroy(this.gameObject);
            }

        }
    }
}
