using UnityEngine;

namespace AF
{

    [RequireComponent(typeof(BoxCollider))]
    public class WeaponInstance : MonoBehaviour, IWeaponInstance
    {
        public Weapon weapon;

        Player player;

        ICombatable combatable;
        AudioSource combatantAudioSource;

        // References
        BoxCollider boxCollider => GetComponent<BoxCollider>();
        Healthbox targetHealthbox;

        private void Awake()
        {
            player = GetComponentInParent<Player>();
            player.TryGetComponent<ICombatable>(out combatable);

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
            if (weapon.swingSfx != null && combatantAudioSource != null)
            {
                Utils.PlaySfx(combatantAudioSource, weapon.swingSfx);
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

            float damageToApply = PlayerStatsManager.instance.GetWeaponAttack(weapon);

            targetHealthbox.TakeDamage(damageToApply, player.gameObject.transform, player.name, weapon.impactSfx);

            //Disable hitbox to prevent any bugs of applying damage twice per frame
            DisableHitbox();
        }

    }

}
