
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class DestroyableSpellParticle : DestroyableParticle
    {
        public Spell spell;


        EnemyHealthController enemyHealthController = null;

        List<EnemyHealthController> enemiesHit = new();

        private void OnParticleCollision(GameObject other)
        {
            OnCollide(other);
        }

        public void OnCollide(GameObject other)
        {
            if (spell == null)
            {
                return;
            }

            enemyHealthController = other.GetComponent<EnemyHealthController>();
            if (enemyHealthController == null)
            {
                enemyHealthController = other.GetComponentInParent<EnemyHealthController>();
            }

            if (enemyHealthController != null)
            {
                if (enemiesHit.Contains(enemyHealthController))
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

                var damage = spell.damageOnHitEnemy;
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

                if (Player.instance.equippedAccessory != null && Player.instance.equippedAccessory.increasesSpellDamage)
                {
                    damage = (int)(damage * Player.instance.equippedAccessory.spellDamageMultiplier);
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
                    var enemyStatus = other.GetComponent<EnemyNegativeStatusController>();

                    enemyStatus.InflictStatusEffect(spell.statusEffectInflict, spell.statusEffectInflictAmount);
                }


            }
        }

    }
}