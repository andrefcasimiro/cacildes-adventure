using UnityEngine;
using UnityEngine.Events;

namespace AF.Shooting
{
    public class CharacterShooter : CharacterBaseShooter
    {

        public UnityEvent onShoot;

        Transform queuedTransformOrigin;
        Projectile queuedProjectile;

        /// <summary>
        /// Unity Event
        /// </summary>
        public void SetTransformOriginForProjectile(Transform transform)
        {
            this.queuedTransformOrigin = transform;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void Shoot(Projectile queuedProjectile)
        {
            this.queuedProjectile = queuedProjectile;
        }


        CharacterManager GetCharacterManager()
        {
            return characterBaseManager as CharacterManager;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public override void CastSpell()
        {
            FireProjectile(queuedProjectile.gameObject, queuedTransformOrigin, GetCharacterManager().targetManager.currentTarget.transform);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public override void FireArrow()
        {
            if (queuedProjectile == null || GetCharacterManager()?.targetManager?.currentTarget == null)
            {
                return;
            }

            FireProjectile(queuedProjectile?.gameObject, queuedTransformOrigin, GetCharacterManager()?.targetManager?.currentTarget?.transform);
        }


        void FireProjectile(GameObject projectile, Transform origin, Transform lockOnTarget)
        {
            GameObject projectileInstance = Instantiate(projectile.gameObject, origin.position, Quaternion.identity);
            if (projectileInstance == null)
            {
                return;
            }

            projectileInstance.TryGetComponent(out IProjectile componentProjectile);
            if (componentProjectile == null)
            {
                return;
            }

            if (lockOnTarget != null)
            {
                var rot = lockOnTarget.position + lockOnTarget.up - origin.position;
                projectileInstance.transform.rotation = Quaternion.LookRotation(rot);

                rot.y = 0;
                characterBaseManager.transform.rotation = Quaternion.LookRotation(rot);
            }

            componentProjectile.Shoot(characterBaseManager, projectileInstance.transform.forward * componentProjectile.GetForwardVelocity(), componentProjectile.GetForceMode());

            onShoot?.Invoke();
        }

        public override bool CanShoot()
        {
            return true;
        }
    }

}
