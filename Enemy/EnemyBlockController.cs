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

        CharacterManager characterManager => GetComponent<CharacterManager>();

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
            return false;
            //return characterManager.animator.GetBool(characterManager.hashIsBlocking);
        }

        public void ActivateParry()
        {
            // characterManager.animator.Play(characterManager.hashParry);

            playerPoiseController.IncreasePoiseDamage(1);
            playerPoiseController.PlayParried();

            if (shield != null)
            {
                Instantiate(playerParryManager.parryFx, shield.transform.position, Quaternion.identity);
            }
        }

        public void HandleBlock(Vector3 blockContactPosition, int guardBreakHitAmount)
        {
            /* if (characterManager.enemyPostureController != null)
             {
                 bool hasBrokePosture = characterManager.enemyPostureController.TakePostureDamage(guardBreakHitAmount);

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
                 characterManager.animator.Play(blockHitAnimationName);
             }*/
        }

        /// <summary>
        /// Animation Event
        /// </summary>
        public void EvaluateDecisionAfterBlockHit()
        {
            if (Random.Range(0, 100) <= 50)
            {
                // characterManager.animator.Play(characterManager.hashCombatting);
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

            // characterManager.enemyHealthController.DisableHealthHitboxes();
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

            //  characterManager.enemyHealthController.EnableHealthHitboxes();
        }

        public bool CanBlock()
        {
            if (false) //characterManager.enemyBlockController.blockWeight == 0)
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

            return Random.Range(0, 100) < 1; //characterManager.enemyBlockController.blockWeight;
        }

        public bool CanParry()
        {
            if (false)//characterManager.enemyBlockController.parryWeight == 0)
            {
                return false;
            }

            if (IsBusy())
            {
                return false;
            }

            return Random.Range(0, 100) < 1f;//characterManager.enemyBlockController.parryWeight;
        }

        public bool IsBusy()
        {
            /*
                        // If not waiting, don't allow to block or parry because it means we are in the middle of another action
                        if (!characterManager.enemyCombatController.IsWaiting())
                        {
                            return true;
                        }

                        if (characterManager.enemyPostureController != null && characterManager.enemyPostureController.IsStunned())
                        {
                            return true;
                        }

                        if (characterManager.enemyDodgeController != null && characterManager.enemyDodgeController.IsDodging())
                        {
                            return true;
                        }*/

            return false;
        }
    }

}
