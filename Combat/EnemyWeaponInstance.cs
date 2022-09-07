using UnityEngine;

namespace AF
{

    [RequireComponent(typeof(BoxCollider))]
    public class EnemyWeaponInstance : MonoBehaviour, IWeaponInstance
    {
        public AudioClip weaponSwingSfx;
        public AudioClip weaponImpactSfx;

        Enemy enemy;

        ICombatable combatable;
        AudioSource combatantAudioSource;

        // References
        BoxCollider boxCollider => GetComponent<BoxCollider>();
        Healthbox targetHealthbox;

        private void Awake()
        {
            enemy = GetComponentInParent<Enemy>();
            enemy.TryGetComponent<ICombatable>(out combatable);

            if (combatable != null)
            {
                combatantAudioSource = combatable.GetCombatantAudioSource();
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            DisableHitbox();
        }

        public void EnableHitbox()
        {
            if (weaponSwingSfx != null && combatantAudioSource != null)
            {
                Utils.PlaySfx(combatantAudioSource, weaponSwingSfx);
            }

            boxCollider.enabled = true;
        }

        public void DisableHitbox()
        {
            boxCollider.enabled = false;
        }

        public void OnTriggerEnter(Collider other)
        {
            other.TryGetComponent(out targetHealthbox);

            if (targetHealthbox == null)
            {
                return;
            }

            float damageToReceive = Mathf.Clamp(
                enemy.weaponDamage - PlayerStatsManager.instance.GetDefenseAbsorption(),
                1f,
                PlayerStatsManager.instance.GetMaxHealthPoints()
                ); 

            targetHealthbox.TakeDamage(damageToReceive, enemy.transform, weaponSwingSfx);

            if (enemy.weaponStatusEffect != null)
            {
                PlayerStatsManager.instance.UpdateStatusEffect(enemy.weaponStatusEffect, enemy.statusEffectAmount);
            }
        }

    }

}
