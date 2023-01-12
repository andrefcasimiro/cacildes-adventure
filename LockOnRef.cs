using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class LockOnRef : MonoBehaviour
    {
        [HideInInspector] public EnemyManager enemyManager;

        private void Start()
        {
            enemyManager = GetComponentInParent<EnemyManager>(true);
        }

        public bool CanLockOn()
        {
            return enemyManager.currentHealth > 0;
        }
    }

}
