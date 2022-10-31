using System.Collections;
using UnityEngine;

namespace AF
{
    public class EnemyHealthHitbox : MonoBehaviour
    {
        [HideInInspector] public EnemyHealthController enemyHealthController;
        [HideInInspector] public EnemyCombatController enemyCombatController;

        private void Start()
        {
            enemyCombatController = GetComponentInParent<EnemyCombatController>();
            enemyHealthController = enemyCombatController.GetComponent<EnemyHealthController>();
        }

    }
}
