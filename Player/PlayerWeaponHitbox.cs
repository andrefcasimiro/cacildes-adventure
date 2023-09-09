﻿using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF
{

    public class PlayerWeaponHitbox : MonoBehaviour
    {
        // References
        BoxCollider boxCollider => GetComponent<BoxCollider>();

        public TrailRenderer trailRenderer;

        PlayerCombatController playerCombatController;
        AttackStatManager attackStatManager;

        float maxTimerBeforeHittingObjects = 0.5f;
        float timer = Mathf.Infinity;

        float maxTimerBeforeAllowingDamageAgain = .5f;
        float damageCooldownTimer = Mathf.Infinity;

        List<EnemyHealthHitbox> hitEnemies = new();

        public bool debug = false;

        public DestroyableParticle heavyAttackParticle;

        public bool isFoot = false;

        private void Awake()
        {
            attackStatManager = GetComponentInParent<AttackStatManager>();
            playerCombatController = GetComponentInParent<PlayerCombatController>();

            if (trailRenderer == null)
            {
                trailRenderer = GetComponent<TrailRenderer>();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            DisableHitbox();
        }

        private void Update()
        {
            if (timer < maxTimerBeforeHittingObjects)
            {
                timer += Time.deltaTime;
            }


            if (hitEnemies.Count > 0)
            {
                if (damageCooldownTimer <= maxTimerBeforeAllowingDamageAgain)
                {
                    damageCooldownTimer += Time.deltaTime;
                }
                else
                {
                    hitEnemies.Clear();
                }
            }
        }

        public void EnableHitbox()
        {
            var weapon = Player.instance.equippedWeapon;

            if (isFoot == false && weapon != null && weapon.swingSfx != null)
            {
                BGMManager.instance.PlaySound(weapon.swingSfx, playerCombatController.combatAudioSource);
            }
            else if (playerCombatController.unarmedSwingSfx != null)
            {
                BGMManager.instance.PlaySound(playerCombatController.unarmedSwingSfx, playerCombatController.combatAudioSource);
            }

            if (trailRenderer != null)
            {
                trailRenderer.enabled = true;
            }

            if (debug)
            {
                GetComponent<MeshRenderer>().enabled = true;
            }

            boxCollider.enabled = true;
        }

        public void DisableHitbox()
        {
            if (trailRenderer != null)
            {
                trailRenderer.enabled = false;
            }

            if (debug)
            {
                GetComponent<MeshRenderer>().enabled = false;
            }

            boxCollider.enabled = false;
        }

        public void OnTriggerEnter(Collider other)
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

            var weapon = Player.instance.equippedWeapon;

            var enemyHealthHitbox = other.GetComponent<EnemyHealthHitbox>();
            if (enemyHealthHitbox == null)
            {
                enemyHealthHitbox = other.GetComponentInChildren<EnemyHealthHitbox>();
            }


            if (enemyHealthHitbox == null)
            {
                if (other.gameObject.CompareTag("IllusionaryWall"))
                {
                    other.GetComponent<IllusionaryWall>().hasBeenHit = true;
                }

                if (other.gameObject.CompareTag("Wood"))
                {
                    if (other.TryGetComponent<Destroyable>(out var destroyable))
                    {
                        destroyable.DestroyObject(other.ClosestPointOnBounds(destroyable.transform.position));
                    }
                    else if (weapon != null && weapon.woodImpactFx != null && timer > maxTimerBeforeHittingObjects)
                    {
                        Instantiate(weapon.woodImpactFx, transform.position, Quaternion.identity);
                    }
                }

                if (weapon != null && timer > maxTimerBeforeHittingObjects)
                {
                    if (other.gameObject.CompareTag("Metal") && weapon.metalImpactFx != null)
                    {
                        Instantiate(weapon.metalImpactFx, transform.position, Quaternion.identity);
                    }

                    if (other.gameObject.CompareTag("Water") && weapon.waterImpactFx != null)
                    {
                        Instantiate(weapon.waterImpactFx, transform.position, Quaternion.identity);
                    }
                }

                timer = 0;
                return;
            }

            if (!hitEnemies.Contains(enemyHealthHitbox))
            {
                if (enemyHealthHitbox.enabled)
                {
                    hitEnemies.Add(enemyHealthHitbox);

                    enemyHealthHitbox.enemyManager.enemyHealthController.TakeDamage(
                        attackStatManager,
                        weapon,
                        other.ClosestPointOnBounds(this.transform.position),
                        (weapon != null && weapon.impactFleshSfx != null) ? weapon.impactFleshSfx : playerCombatController.unarmedHitImpactSfx,
                        enemyHealthHitbox.damageBonus);

                    if (enemyHealthHitbox.enemyManager.enemyTargetController != null)
                    {
                        enemyHealthHitbox.enemyManager.enemyTargetController.BreakCompanionFocus();
                    }

                    if (heavyAttackParticle != null && attackStatManager.IsHeavyAttacking())
                    {
                        Instantiate(heavyAttackParticle, trailRenderer.transform.position, Quaternion.identity);
                    }


                }

                damageCooldownTimer = 0f;
            }
        }

    }

}
