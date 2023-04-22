using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class DamagePlayerOnTriggerStay : MonoBehaviour
    {
        public int damage = 50;

        public float maxCooldownBeforeAnotherDamage = 2f;
        float cooldown = Mathf.Infinity;

        PlayerHealthbox playerHealthbox;
        public ParticleSystem part => GetComponent<ParticleSystem>();

        public List<ParticleCollisionEvent> collisionEvents;

        public bool deactivateOnCollision = false;

        [Header("Attack Options")]
        public WeaponElementType weaponElementType = WeaponElementType.None;
        public int poiseDamage = 0;
        public bool ignoreDodging = false;
        public StatusEffect statusEffect;
        public int statusEffectAmount = 0;
        public bool takeArmorInConsideration = false;

        DefenseStatManager defenseStatManager => FindObjectOfType<DefenseStatManager>(true);

        [Header("Damage Enemies Instead?")]
        public bool damageEnemiesInstead = false;

        private void Start()
        {
            collisionEvents = new List<ParticleCollisionEvent>();

            playerHealthbox = FindObjectOfType<PlayerHealthbox>(true);
        }

        private void OnEnable()
        {
        }

        private void Update()
        {
            if (cooldown <= maxCooldownBeforeAnotherDamage)
            {
                cooldown += Time.deltaTime;
            }
        }

        private void OnParticleCollision(GameObject other)
        {
            if (cooldown < maxCooldownBeforeAnotherDamage)
            {
                return;
            }
            int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

            int i = 0;
            while (i < numCollisionEvents)
            {
                // Damage Player Case
                if (!damageEnemiesInstead && (other.CompareTag("Player") || other.CompareTag("PlayerHealthbox")))
                {
                    var copiedDamage = damage;
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

                        playerHealthbox.TakeEnvironmentalDamage(copiedDamage, poiseDamage, ignoreDodging);
                    }

                    if (statusEffect != null)
                    {
                        FindObjectOfType<PlayerStatusManager>(true).InflictStatusEffect(statusEffect, statusEffectAmount, false);
                    }

                    cooldown = 0f;

                    if (deactivateOnCollision)
                    {
                        break;
                    }
                }

                if (damageEnemiesInstead)
                {

                    EnemyHealthController enemyHealthController = null;

                    if (other.CompareTag("Enemy"))
                    {
                        enemyHealthController = other.GetComponent<EnemyHealthController>();
                    }

                    if (enemyHealthController == null)
                    {
                        enemyHealthController = other.GetComponent<EnemyHealthHitbox>()?.enemyManager?.enemyHealthController;
                    }

                    if (enemyHealthController != null)
                    {
                        var copiedDamage = damage;

                        if (copiedDamage > 0)
                        {
                            enemyHealthController.TakeEnvironmentalDamage(copiedDamage);
                        }

                        cooldown = 0f;

                        if (deactivateOnCollision)
                        {
                            break;
                        }
                    }
                }

                i++;
            }
        }

    }

}
