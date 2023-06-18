using System.Collections;
using UnityEngine;

namespace AF
{
    public class EnemyHealthHitbox : MonoBehaviour
    {
        public EnemyManager enemyManager;

        public float damageBonus = 0f;


        private void Start()
        {
            if (enemyManager == null)
            {
                enemyManager = GetComponentInParent<EnemyManager>();
            }
        }


    }
}
