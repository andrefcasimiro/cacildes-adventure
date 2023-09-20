using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class ExplodingBarrelDamage : MonoBehaviour
    {
        public int damageFromFarDistance = 50;
        public int multiplierForEnemies = 3;

        public float maxExplosionDuration = .5f;
        public float maxEffectiveDistance = 4f;

        PlayerHealthbox playerHealthbox;
        public ParticleSystem part => GetComponent<ParticleSystem>();

        public List<ParticleCollisionEvent> collisionEvents;

        [Header("Attack Options")]
        public WeaponElementType weaponElementType = WeaponElementType.None;
        public int poiseDamage = 0;
        public bool ignoreDodging = false;
        public bool takeArmorInConsideration = false;

        DefenseStatManager defenseStatManager;

        [Header("Damage Enemies Instead?")]
        public bool forceEnemiesIntoCombat = true;
        public int pushforceForEnemies = 3;

        List<GameObject> hitEntities = new();

        bool explosionIsActivated = true;

        private void Awake()
        {
            defenseStatManager = FindObjectOfType<DefenseStatManager>(true);
        }

        private void Start()
        {
            collisionEvents = new List<ParticleCollisionEvent>();

            playerHealthbox = FindObjectOfType<PlayerHealthbox>(true);
        }


        private void OnParticleCollision(GameObject other)
        {
            int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

            if (numCollisionEvents <= 0)
            {
                return;
            }

            if (hitEntities.Contains(other))
            {
                return;
            }

            if (!explosionIsActivated)
            {
                return;
            }

            StartCoroutine(DisableExplosion());

            if (other.CompareTag("Player") || other.CompareTag("PlayerHealthbox"))
            {
                hitEntities.Add(other);

                var copiedDamage = damageFromFarDistance;

                var distance = Vector3.Distance(other.transform.position, transform.position);

                // Calculate damage multiplier based on distance (damage increases as distance decreases)
                float distanceMultiplier = 1.0f - Mathf.Clamp01(distance / maxEffectiveDistance);

                // Apply the multiplier to the copied damage and ensure it's not negative
                copiedDamage = (int)Mathf.Max(Mathf.RoundToInt(copiedDamage * distanceMultiplier), 0);

                if (copiedDamage > 0)
                {
                    copiedDamage = Player.instance.CalculateIncomingElementalAttack(copiedDamage, weaponElementType, defenseStatManager);

                    playerHealthbox.TakeEnvironmentalDamage(0, poiseDamage, ignoreDodging, copiedDamage, weaponElementType);
                }
            }
            else
            {
                EnemyHealthController enemyHealthController = null;

                if (other.CompareTag("Enemy"))
                {
                    hitEntities.Add(other);

                    enemyHealthController = other.GetComponent<EnemyHealthController>();
                }



                if (enemyHealthController == null)
                {
                    enemyHealthController = other.GetComponent<EnemyHealthHitbox>()?.enemyManager?.enemyHealthController;
                }

                if (enemyHealthController != null)
                {

                    var copiedDamage = damageFromFarDistance;

                    var distance = Vector3.Distance(other.transform.position, transform.position);

                    // Calculate damage multiplier based on distance (damage increases as distance decreases)
                    float distanceMultiplier = 1.0f - Mathf.Clamp01(distance / maxEffectiveDistance);

                    // Apply the multiplier to the copied damage and ensure it's not negative
                    copiedDamage = (int)Mathf.Max(Mathf.RoundToInt(copiedDamage * distanceMultiplier), 0);

                    if (copiedDamage > 0)
                    {
                        enemyHealthController.TakeEnvironmentalDamage(copiedDamage * multiplierForEnemies);
                    }

                    enemyHealthController.GetComponent<EnemyManager>().PushEnemy(pushforceForEnemies, ForceMode.Acceleration);
                }




                if (forceEnemiesIntoCombat && enemyHealthController != null)
                {
                    enemyHealthController.GetComponent<EnemyManager>().enemyBehaviorController.ChasePlayer();
                }
            }

        }

        IEnumerator DisableExplosion()
        {
            yield return new WaitForSeconds(maxExplosionDuration);
            explosionIsActivated = false;
        }
    }


}
