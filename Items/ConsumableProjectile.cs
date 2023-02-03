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
        }

        public string animationToPlay = "Preparing Arrow";

        public Projectile projectile;

        public ProjectileType projectileType = ProjectileType.BOW;

        public override void OnConsume()
        {
            if (Player.instance.currentHealth <= 0)
            {
                return;
            }

            var playerShootingManager = FindObjectOfType<PlayerShootingManager>(true);

            if (projectileType == ProjectileType.BOW)
            {
                playerShootingManager.ShootBow(this);
            }
        }

    }

}
