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

            if (damageCooldownTimer <= maxTimerBeforeAllowingDamageAgain)
            {
                damageCooldownTimer += Time.deltaTime;
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
                            return;
                        }


                    }

                }
                return;
            }

            // If hit enemy, dont allow to hit objects to avoid overlap
            timer = 0;

            if (damageCooldownTimer <= maxTimerBeforeAllowingDamageAgain) {
                return;
            }

            DisableHitbox();

            enemyHealthHitbox.enemyHealthController.TakeDamage(
                attackStatManager,
                weapon,
                this.transform,
                (weapon != null && weapon.impactFleshSfx != null) ? weapon.impactFleshSfx : hitImpactSfx);

            maxTimerBeforeAllowingDamageAgain = 0f;
        }

    }

}
