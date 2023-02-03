using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class EnemyBlockController : MonoBehaviour
    {
        [Header("Block")]
        public DestroyableParticle blockParticleEffect;
        public EnemyShieldCollider shield;
        [Range(0, 100)] public int blockWeight = 0;
        public bool isShieldAlwaysVisible = false;

        EnemyManager enemyManager => GetComponent<EnemyManager>();

        // Start is called before the first frame update
        void Start()
        {
            InitializeShield();
        }

        void InitializeShield()
        {
            if (shield != null)
            {
                shield.gameObject.SetActive(isShieldAlwaysVisible);
            }
        }

        public bool IsBlocking()
        {
            return enemyManager.animator.GetBool(enemyManager.hashIsBlocking);
        }

        /// <summary>
        /// Animation Event
        /// </summary>
        public void ActivateBlock()
        {
            if (shield != null && !isShieldAlwaysVisible)
            {
                shield.gameObject.SetActive(true);
            }

            enemyManager.enemyHealthController.DisableHealthHitboxes();
        }

        /// <summary>
        /// Animation Event
        /// </summary>
        public void DeactivateBlock()
        {
            if (shield != null && !isShieldAlwaysVisible)
            {
                shield.gameObject.SetActive(false);
            }

            enemyManager.enemyHealthController.EnableHealthHitboxes();
        }
    }

}
