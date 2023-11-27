using UnityEngine;

namespace AF.Shooting
{
    public interface IProjectile
    {
        public void Shoot(Transform target, Vector3 aimForce, ForceMode forceMode);

        public float GetForwardVelocity();

        public ForceMode GetForceMode();

    }
}
