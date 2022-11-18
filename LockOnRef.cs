using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class LockOnRef : MonoBehaviour
    {
        [HideInInspector] public EnemyHealthController enemyHealthController;

        private void Start()
        {
            enemyHealthController = GetComponentInParent<EnemyHealthController>(true);

            if (enemyHealthController == null)
            {
                enemyHealthController = GetComponent<EnemyHealthController>();
            }
        }

        public bool CanLockOn()
        {
            return enemyHealthController.currentHealth > 0;
        }
    }

}