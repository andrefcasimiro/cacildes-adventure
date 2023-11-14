using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Item / New Consumable Projectile")]
    public class ConsumableProjectile : Consumable
    {
        public enum ProjectileType
        {
            BOW,
            THROWABLE,
        }

        public string animationToPlay = "Preparing Arrow";

        public Projectile projectile;

        public ProjectileType projectileType = ProjectileType.BOW;

        public override void OnConsume()
        {

            var playerShootingManager = FindObjectOfType<PlayerShootingManager>(true);

            if (projectileType == ProjectileType.BOW)
            {
                playerShootingManager.ShootBow(this);
            }
            else if (projectileType == ProjectileType.THROWABLE)
            {
                playerShootingManager.ThrowConsumable(this);
            }
        }

    }

}
