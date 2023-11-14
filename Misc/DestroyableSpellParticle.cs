
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AF.Stats;
using UnityEngine;

namespace AF
{
    public class DestroyableSpellParticle : DestroyableParticle
    {
        public Spell spell;
        [HideInInspector] public StatsBonusController statsBonusController;


        EnemyHealthController enemyHealthController = null;

        List<EnemyHealthController> enemiesHit = new();

        public bool collideOnlyOnce = true;

        public float timeBetweenDamage = Mathf.Infinity;
        public float maxTimeBetweenDamage = 0.1f;

        public int pushForce = 0;
        public bool isPullForce = false;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        private void OnParticleCollision(GameObject other)
        {
            OnCollide(other);
        }

        private void Update()
        {
            if (timeBetweenDamage < maxTimeBetweenDamage)
            {
                timeBetweenDamage += Time.deltaTime;
            }
        }

        public void OnCollide(GameObject other)
        {
            if (spell == null)
            {
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


            if (collideOnlyOnce == false && timeBetweenDamage < maxTimeBetweenDamage)
            {
                return;
            }

            if (!other.TryGetComponent<EnemyHealthController>(out enemyHealthController))
            {
                enemyHealthController = other.GetComponentInParent<EnemyHealthController>();
            }

            if (enemyHealthController != null && enemyHealthController.currentHealth > 0)
            {
                if (collideOnlyOnce == true && enemiesHit.Contains(enemyHealthController))
                {
                    return;
                }
                else
                {
                    enemiesHit.Add(enemyHealthController);
                }

                if (spell.impactFx != null)
                {
                    Instantiate(spell.impactFx, other.transform.position, Quaternion.identity);
                }

                var intelligenceBonus = 0;
                if (statsBonusController != null)
                {
                    intelligenceBonus = statsBonusController.intelligenceBonus;
                }

                var damage = (float)Formulas.CalculateSpellValue(
                    (int)spell.damageOnHitEnemy, intelligenceBonus + playerStatsDatabase.intelligence);

                Enemy enemy = enemyHealthController.GetComponent<EnemyManager>().enemy;

                if (spell.spellElement == WeaponElementType.Fire)
                {
                    damage *= enemy.fireDamageBonus;
                }
                else if (spell.spellElement == WeaponElementType.Frost)
                {
                    damage *= enemy.frostDamageBonus;
                }
                else if (spell.spellElement == WeaponElementType.Lightning)
                {
                    damage *= enemy.lightningDamageBonus;
                }
                else if (spell.spellElement == WeaponElementType.Magic)
                {
                    damage *= enemy.magicDamageBonus;
                }

                if (spell.increaseDamageWithReputation)
                {
                    var reputation = playerStatsDatabase.GetCurrentReputation();

                    if (reputation <= 0)
                    {
                        reputation = 1;
                    }

                    damage += (reputation * 2.25f);
                }

                if (spell.spellPostureDamage > 0)
                {
                    enemyHealthController.GetComponent<EnemyPostureController>().TakePostureDamage(spell.spellPostureDamage);
                }

                if (pushForce != 0)
                {
                    var finalPushForce = pushForce + Formulas.CalculateSpellValue(pushForce, intelligenceBonus + playerStatsDatabase.intelligence);


                    if (spell.increaseDamageWithReputation)
                    {
                        var reputation = playerStatsDatabase.GetCurrentReputation();

                        if (reputation <= 0)
                        {
                            reputation = 1;
                        }

                        finalPushForce += (reputation);
                    }

                    int roundedValue = finalPushForce;
                    if (isPullForce)
                    {
                        roundedValue *= -1;
                    }
                    else
                    {
                        roundedValue = Mathf.RoundToInt(finalPushForce / 10);
                    }

                    if (other.TryGetComponent<EnemyManager>(out var enemyManager))
                    {
                        enemyManager.PushEnemy(roundedValue, ForceMode.Acceleration);
                    }
                }

                var targetAccessory = equipmentDatabase.accessories.FirstOrDefault(x => x.increasesSpellDamage && x.spellDamageMultiplier > 0);
                if (targetAccessory != null)
                {
                    damage = (int)(damage * targetAccessory.spellDamageMultiplier);
                }

                enemyHealthController.TakeEnvironmentalDamage(damage);

                var enemyBehavior = other.GetComponent<EnemyBehaviorController>();
                if (enemyBehavior != null)
                {
                    enemyBehavior.TurnAgressive();
                    enemyBehavior.ChasePlayer();
                }

                if (spell.statusEffectInflict != null)
                {
                    if (other.TryGetComponent<EnemyNegativeStatusController>(out var enemyStatus))
                    {
                        enemyStatus.InflictStatusEffect(spell.statusEffectInflict, spell.statusEffectInflictAmount);
                    }
                }

                if (collideOnlyOnce == false)
                {
                    timeBetweenDamage = 0;
                }

            }

        }

    }
}
