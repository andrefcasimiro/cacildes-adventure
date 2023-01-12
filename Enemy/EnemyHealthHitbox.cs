using System.Collections;
using UnityEngine;

namespace AF
{
    public class EnemyHealthHitbox : MonoBehaviour
    {
        [HideInInspector] public EnemyManager enemyManager;

        public float damageBonus = 0f;

        private void Start()
        {
            enemyManager = GetComponentInParent<EnemyManager>();
        }

    }
}
