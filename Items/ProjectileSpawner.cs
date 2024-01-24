using UnityEngine;

namespace AF.Inventory
{
    public class ProjectileSpawner : MonoBehaviour
    {
        public PlayerManager playerManager;
        public LockOnManager lockOnManager;
        Projectile queuedProjectile;

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="projectile"></param>
        public void SpawnProjectile(Projectile projectile)
        {
            queuedProjectile = projectile;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ThrowProjectile()
        {
            if (queuedProjectile == null)
            {
                return;
            }

            if (lockOnManager.nearestLockOnTarget != null)
            {
                var rotation = lockOnManager.nearestLockOnTarget.transform.position - playerManager.transform.position;
                rotation.y = 0;
                playerManager.transform.rotation = Quaternion.LookRotation(rotation);
            }

            Projectile instance = Instantiate(queuedProjectile, playerManager.transform.position + playerManager.transform.up, Quaternion.identity);

            if (lockOnManager.nearestLockOnTarget != null)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lockOnManager.nearestLockOnTarget.transform.position - instance.transform.position);
                instance.transform.rotation = targetRotation;
            }

            instance.Shoot(playerManager, instance.GetForwardVelocity() * instance.transform.forward, instance.forceMode);

            queuedProjectile = null;
        }
    }
}
