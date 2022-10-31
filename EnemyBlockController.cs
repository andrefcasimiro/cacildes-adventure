using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class EnemyBlockController : MonoBehaviour
    {
        public readonly int hashIsBlocking = Animator.StringToHash("IsBlocking");

        [Header("Block Settings")]
        public GameObject shield;
        public DestroyableParticle blockParticleEffect;

        [Header("Trigger Settings")]
        [Range(0, 100)] public int weight = 0;

        // Component refs
        private Enemy enemy => GetComponent<Enemy>();
        private EnemyHealthController enemyHealthController => GetComponent<EnemyHealthController>();

        public bool IsBlocking()
        {
            return enemy.animator.GetBool(hashIsBlocking);
        }

        #region Animation Events
        public void ActivateBlock()
        {
            enemyHealthController.DisableHealthHitboxes();
        }
        public void DeactivateBlock()
        {
            enemyHealthController.EnableHealthHitboxes();
        }
        #endregion
    }

}
