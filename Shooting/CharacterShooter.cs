using UnityEngine;

namespace AF.Shooting
{
    public class CharacterShooter : CharacterBaseShooter
    {

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
            if (queuedProjectile == null)
            {
                return;
            }

            FireProjectile(queuedProjectile.gameObject, queuedTransformOrigin, GetCharacterManager().targetManager.currentTarget.transform);
        }


        void FireProjectile(GameObject projectile, Transform origin, Transform lockOnTarget)
        {
            GameObject projectileInstance = Instantiate(projectile.gameObject, origin.position, Quaternion.identity);

            projectileInstance.TryGetComponent(out IProjectile componentProjectile);
            if (componentProjectile == null)
            {
                return;
            }

            var rot = lockOnTarget.position - origin.position;
            rot.y = 0;
            projectileInstance.transform.rotation = Quaternion.LookRotation(rot);

            componentProjectile.Shoot(characterBaseManager, projectileInstance.transform.forward * componentProjectile.GetForwardVelocity(), componentProjectile.GetForceMode());

            if (lockOnTarget != null)
            {
                var rotation = lockOnTarget.transform.position - characterBaseManager.transform.position;
                rotation.y = 0;
                characterBaseManager.transform.rotation = Quaternion.LookRotation(rotation);
            }
        }

        public override bool CanShoot()
        {
            return true;
        }
    }

}
