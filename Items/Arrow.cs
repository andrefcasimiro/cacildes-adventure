using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Item / New Arrow")]
    public class Arrow : ConsumableProjectile
    {


        public Projectile projectile;

        public ProjectileType projectileType = ProjectileType.BOW;

        public override void OnConsume()
        {

        }

    }

}
