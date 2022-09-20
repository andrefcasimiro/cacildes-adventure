using UnityEngine;

namespace AF
{

    public interface ICombatable
    {
        public void ActivateLeftHandHitbox();
        public void DeactivateLeftHandHitbox();

        public void ActivateRightHandHitbox();
        public void DeactivateRightHandHitbox();

        public void ActivateRightLegHitbox();
        public void DeactivateRightLegHitbox();

        public void ActivateLeftLegHitbox();
        public void DeactivateLeftLegHitbox();

        public void ActivateHeadHitbox();
        public void DeactivateHeadHitbox();

        public void ActivateAreaOfImpactHitbox();
        public void DeactivateAreaOfImpactHitbox();

        public void FireProjectile();

        public IWeaponInstance GetWeaponInstance();
        public ShieldInstance GetShieldInstance();

        public AudioSource GetCombatantAudioSource();

        public float GetCurrentHealth();
        public void SetCurrentHealth(float health);
        public float GetMaxHealth();

    }

}