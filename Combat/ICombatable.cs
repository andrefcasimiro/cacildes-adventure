using UnityEngine;

namespace AF
{

    public interface ICombatable
    {
        public void ActivateHitbox();
        public void DeactivateHitbox();
        public void FireProjectile();

        public IWeaponInstance GetWeaponInstance();
        public ShieldInstance GetShieldInstance();

        public AudioSource GetCombatantAudioSource();

        public float GetCurrentHealth();
        public void SetCurrentHealth(float health);
        public float GetMaxHealth();

    }

}