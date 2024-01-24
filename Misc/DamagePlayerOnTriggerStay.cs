using System.Collections;
using System.Collections.Generic;
using AF.Stats;
using UnityEngine;

namespace AF
{
    public class DamagePlayerOnTriggerStay : MonoBehaviour
    {
        public int damage = 50;

        public float maxCooldownBeforeAnotherDamage = 2f;
        float cooldown = Mathf.Infinity;

        public ParticleSystem part => GetComponent<ParticleSystem>();

        public List<ParticleCollisionEvent> collisionEvents;

        public bool deactivateOnCollision = false;

        [Header("Attack Options")]
        public int elementalDamage = 0;
        public WeaponElementType weaponElementType = WeaponElementType.None;
        public int poiseDamage = 0;
        public int postureDamage = 0;
        public bool ignoreDodging = false;
        public StatusEffect statusEffect;
        public int statusEffectAmount = 0;
        public bool takeArmorInConsideration = false;

        [Header("Player Options")]
        public bool damageScalesWithIntelligence = false;

        DefenseStatManager defenseStatManager;

        [Header("Damage Enemies Instead?")]
        public bool damageEnemiesInstead = false;
        public bool forceEnemiesIntoCombat = true;


        [Header("Components")]
        public StatsBonusController playerStatsBonusController;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;


        private void Awake()
        {
            defenseStatManager = FindObjectOfType<DefenseStatManager>(true);
        }

        private void Start()
        {
            collisionEvents = new List<ParticleCollisionEvent>();

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

            if (numCollisionEvents <= 0)
            {
                return;
            }

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


                    var finalElementalDamage = Formulas.CalculateIncomingElementalAttack((int)elementalDamage, weaponElementType, defenseStatManager);

                }

                cooldown = 0f;
            }

            if (damageEnemiesInstead)
            {

                //EnemyHealthController enemyHealthController = null;

                if (other.CompareTag("Enemy"))
                {
                    //  enemyHealthController = other.GetComponent<EnemyHealthController>();
                }
                /*
                                if (enemyHealthController == null)
                                {
                                    enemyHealthController = other.GetComponent<CharacterHealthHitbox>()?.characterManager?.enemyHealthController;
                                }

                                if (enemyHealthController != null)
                                {
                                    var copiedDamage = damage;

                                    if (copiedDamage > 0)
                                    {

                                        if (damageScalesWithIntelligence)
                                        {
                                            copiedDamage = Formulas.CalculateSpellValue(copiedDamage, playerSpellManager.GetCurrentInteligence());
                                        }

                                        enemyHealthController.TakeEnvironmentalDamage(copiedDamage);
                                    }

                                    cooldown = 0f;
                                }


                                if (statusEffect != null && enemyHealthController != null)
                                {
                                    var enemyNegativeStatus = enemyHealthController.GetComponent<EnemyNegativeStatusController>();

                                    if (enemyNegativeStatus != null)
                                    {
                                        enemyNegativeStatus.InflictStatusEffect(statusEffect, statusEffectAmount);

                                    }
                                }

                                if (postureDamage > 0)
                                {
                                    enemyHealthController.GetComponent<EnemyPostureController>().TakePostureDamage(postureDamage);
                                }


                                if (forceEnemiesIntoCombat && enemyHealthController != null)
                                {
                                    enemyHealthController.GetComponent<CharacterManager>().enemyBehaviorController.ChasePlayer();
                                }*/
            }
        }

    }

}
