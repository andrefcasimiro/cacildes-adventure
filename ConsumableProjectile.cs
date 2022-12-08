using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Item / New Consumable Projectile")]
    public class ConsumableProjectile : Consumable
    {
        public enum ProjectileType
        {
            BOW,
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
        }

    }

}
