using System.Collections;
using UnityEngine;

namespace AF
{
    public class EnemyBlockController : MonoBehaviour
    {
        [Header("Block")]
        public DestroyableParticle blockParticleEffect;
        public EnemyShieldCollider shield;
        [Range(0, 100)] public int blockWeight = 0;
        [Range(0, 100)] public int parryWeight = 0;
        public bool isShieldAlwaysVisible = false;

        EnemyManager enemyManager => GetComponent<EnemyManager>();

        public string blockHitAnimationName = "";
        public bool blockWhileStrafing = false;

        PlayerPoiseController playerPoiseController;
        PlayerParryManager playerParryManager;


        private void Awake()
        {
             playerPoiseController = FindObjectOfType<PlayerPoiseController>(true);
             playerParryManager = FindObjectOfType<PlayerParryManager>(true);
        }

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

        public void ActivateParry()
        {
            enemyManager.animator.Play(enemyManager.hashParry);

            playerPoiseController.IncreasePoiseDamage(1);
            playerPoiseController.PlayParried();

            if (shield != null)
            {
                Instantiate(playerParryManager.parryFx, shield.transform.position, Quaternion.identity);
            }
        }

        public void HandleBlock(Vector3 blockContactPosition, int guardBreakHitAmount)
        {
            if (enemyManager.enemyPostureController != null)
            {
                bool hasBrokePosture = enemyManager.enemyPostureController.TakePostureDamage(guardBreakHitAmount);
            
                if (hasBrokePosture)
                {
                    return;
                }
            }

            if (blockParticleEffect != null)
            {
                Vector3 pos = blockContactPosition;

                if (pos == Vector3.zero && shield != null)
                {
                    pos = shield.transform.position;
                }

                Instantiate(blockParticleEffect, pos, Quaternion.identity);
            }

            if (!string.IsNullOrEmpty(blockHitAnimationName))
            {
                enemyManager.animator.Play(blockHitAnimationName);
            }
        }

        /// <summary>
        /// Animation Event
        /// </summary>
        public void EvaluateDecisionAfterBlockHit()
        {
            if (Random.Range(0, 100) <= 50)
            {
                enemyManager.animator.Play(enemyManager.hashCombatting);
            }
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

        public bool CanBlock()
        {
            if (enemyManager.enemyBlockController.blockWeight == 0)
            {
                return false;
            }

            if (IsBlocking())
            {
                return false;
            }

            if (IsBusy())
            {
                return false;
            }

            return Random.Range(0, 100) < enemyManager.enemyBlockController.blockWeight;
        }

        public bool CanParry()
        {
            if (enemyManager.enemyBlockController.parryWeight == 0) {
                return false;
            }

            if (IsBusy())
            {
                return false;
            }

            return Random.Range(0, 100) < enemyManager.enemyBlockController.parryWeight;
        }

        public bool IsBusy()
        {

            // If not waiting, don't allow to block or parry because it means we are in the middle of another action
            if (!enemyManager.enemyCombatController.IsWaiting())
            {
                return true;
            }

            if (enemyManager.enemyPostureController != null && enemyManager.enemyPostureController.IsStunned())
            {
                return true;
            }

            if (enemyManager.enemyDodgeController != null && enemyManager.enemyDodgeController.IsDodging())
            {
                return true;
            }

            return false;
        }
    }

}
