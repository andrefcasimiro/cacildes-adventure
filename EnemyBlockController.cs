using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class EnemyBlockController : MonoBehaviour
    {
        public readonly int hashIsBlocking = Animator.StringToHash("IsBlocking");

        [Header("Block Settings")]
        public DestroyableParticle blockParticleEffect;
        public EnemyShieldCollider shield;

        [Header("Trigger Settings")]
        [Range(0, 100)] public int weight = 0;

        // Component refs
        private Enemy enemy => GetComponent<Enemy>();
        private EnemyHealthController enemyHealthController => GetComponent<EnemyHealthController>();

        public bool hideShieldAutomatically = true;

        private void Start()
        {
            if (shield != null && hideShieldAutomatically)
            {
                shield.gameObject.SetActive(false);
            }
        }

        public bool IsBlocking()
        {
            return enemy.animator.GetBool(hashIsBlocking);
        }

        #region Animation Events
        public void ActivateBlock()
        {
            if (shield != null && hideShieldAutomatically)
            {
                shield.gameObject.SetActive(true);
            }
            enemyHealthController.DisableHealthHitboxes();
        }
        public void DeactivateBlock()
        {
            if (shield != null && hideShieldAutomatically)
            {
                shield.gameObject.SetActive(false);
            }
            enemyHealthController.EnableHealthHitboxes();
        }
        #endregion
    }

}
