
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class DestroyableSpellParticle : DestroyableParticle
    {
        public Spell spell;
        [HideInInspector] public EquipmentGraphicsHandler equipmentGraphicsHandler;


        EnemyHealthController enemyHealthController = null;

        List<EnemyHealthController> enemiesHit = new();

        public bool collideOnlyOnce = true;

        public float timeBetweenDamage = Mathf.Infinity;
        public float maxTimeBetweenDamage = 0.1f;

        public int pushForce = 0;
        public bool isPullForce = false;

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
                if (equipmentGraphicsHandler != null)
                {
                    intelligenceBonus = equipmentGraphicsHandler.intelligenceBonus;
                }

                var damage = (float)Player.instance.CalculateSpellValue(
                    (int)spell.damageOnHitEnemy, intelligenceBonus + Player.instance.intelligence);

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
                    var reputation = Player.instance.GetCurrentReputation();

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
                    var finalPushForce = pushForce + Player.instance.CalculateSpellValue(pushForce, intelligenceBonus + Player.instance.intelligence);


                    if (spell.increaseDamageWithReputation)
                    {
                        var reputation = Player.instance.GetCurrentReputation();

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

                var targetAccessory = Player.instance.equippedAccessories.Find(x => x.increasesSpellDamage && x.spellDamageMultiplier > 0);
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
