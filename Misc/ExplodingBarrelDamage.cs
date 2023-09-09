using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class ExplodingBarrelDamage : MonoBehaviour
    {
        public int damageForPlayer = 50;
        public int damageForEnemies = 300;

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


                if (other.CompareTag("Player") || other.CompareTag("PlayerHealthbox"))
            {
                hitEntities.Add(other);

                var copiedDamage = damageForPlayer;
                    if (copiedDamage > 0)
                    {
                        if (takeArmorInConsideration)
                        {
                            copiedDamage = Mathf.Clamp((int)(copiedDamage - defenseStatManager.GetDefenseAbsorption()), 0, 999);
                        }

                        if (weaponElementType == WeaponElementType.Magic)
                        {
                            copiedDamage = Mathf.Clamp((int)(copiedDamage - defenseStatManager.GetMagicDefense()), 0, 999);
                        }

                        if (weaponElementType == WeaponElementType.Magic)
                        {
                            copiedDamage = Mathf.Clamp((int)(copiedDamage - defenseStatManager.GetFireDefense()), 0, 999);
                        }

                        playerHealthbox.TakeEnvironmentalDamage(copiedDamage, poiseDamage, ignoreDodging, weaponElementType);
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
                        var copiedDamage = damageForEnemies;

                        if (copiedDamage > 0)
                        {
                            enemyHealthController.TakeEnvironmentalDamage(damageForEnemies);
                        }

                    enemyHealthController.GetComponent<EnemyManager>().PushEnemy(pushforceForEnemies, ForceMode.Acceleration);
                    }

                    


                    if (forceEnemiesIntoCombat && enemyHealthController != null)
                    {
                        enemyHealthController.GetComponent<EnemyManager>().enemyBehaviorController.ChasePlayer();
                    }
                }

            }
        }


}
