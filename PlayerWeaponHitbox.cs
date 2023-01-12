using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF
{

    public class PlayerWeaponHitbox : MonoBehaviour
    {
        public Weapon weapon;

        // References
        BoxCollider boxCollider => GetComponent<BoxCollider>();

        TrailRenderer trailRenderer => GetComponent<TrailRenderer>();

        PlayerCombatController playerCombatController;
        AttackStatManager attackStatManager;

        public AudioClip swingSfx;
        public AudioClip hitImpactSfx;

        float maxTimerBeforeHittingObjects = 0.5f;
        float timer = Mathf.Infinity;

        float maxTimerBeforeAllowingDamageAgain = .5f;
        float damageCooldownTimer = Mathf.Infinity;

        List<EnemyHealthHitbox> hitEnemies = new List<EnemyHealthHitbox>();

        private void Awake()
        {
            attackStatManager = GetComponentInParent<AttackStatManager>();
            playerCombatController = GetComponentInParent<PlayerCombatController>();
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
            if (weapon != null && weapon.swingSfx != null)
            {
                BGMManager.instance.PlaySound(weapon.swingSfx, playerCombatController.combatAudioSource);
            }
            else if (swingSfx != null)
            {
                BGMManager.instance.PlaySound(swingSfx, playerCombatController.combatAudioSource);
            }

            if (trailRenderer != null)
            {
                trailRenderer.enabled = true;
            }

            boxCollider.enabled = true;
        }

        public void DisableHitbox()
        {
            if (trailRenderer != null)
            {
                trailRenderer.enabled = false;
            }

            boxCollider.enabled = false;
        }

        public void OnTriggerEnter(Collider other)
        {
            EnemyHealthHitbox enemyHealthHitbox = other.GetComponent<EnemyHealthHitbox>();
            if (enemyHealthHitbox == null)
            {
                if (timer > maxTimerBeforeHittingObjects)
                {
                    timer = 0f;

                    if (weapon != null)
                    {
                        if (other.gameObject.tag == "Metal" && weapon.metalImpactFx != null)
                        {
                            Instantiate(weapon.metalImpactFx, transform.position, Quaternion.identity);
                        }

                        if (other.gameObject.tag == "Water" && weapon.waterImpactFx != null)
                        {
                            Instantiate(weapon.waterImpactFx, transform.position, Quaternion.identity);
                        }

                        if (other.gameObject.tag == "Wood" && weapon.woodImpactFx != null)
                        {
                            Instantiate(weapon.woodImpactFx, transform.position, Quaternion.identity);

                            var destroyable = other.GetComponent<Destroyable>();
                            if (destroyable != null)
                            {
                                destroyable.OnDestroy();
                            }
                        }

                    }

                }
                return;
            }

            // If hit enemy, dont allow to hit objects to avoid overlap
            timer = 0;

            if (!hitEnemies.Contains(enemyHealthHitbox))
            {
                hitEnemies.Add(enemyHealthHitbox);

                enemyHealthHitbox.enemyManager.TakeDamage(
                    attackStatManager,
                    weapon,
                    this.transform,
                    (weapon != null && weapon.impactFleshSfx != null) ? weapon.impactFleshSfx : hitImpactSfx,
                    enemyHealthHitbox.damageBonus);

                enemyHealthHitbox.enemyManager.BreakCompanionFocus();

                damageCooldownTimer = 0f;
            }
        }

    }

}
