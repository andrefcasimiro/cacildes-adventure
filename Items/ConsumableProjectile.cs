using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Item / New Consumable Projectile")]
    public class ConsumableProjectile : Item
    {
        public enum ProjectileType
        {
            BOW,
            THROWABLE,
        }

        public string animationToPlay = "Preparing Arrow";

        public Projectile projectile;

        public ProjectileType projectileType = ProjectileType.BOW;

    }

}
