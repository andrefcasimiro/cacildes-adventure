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

        [Header("Block Defense")]
        public int maxHitsBeforeGuardBreak = 2;
        public float maxHitsCooldownBeforeResetGuardBreak = 15f;
        int currentBlockHits = 0;
        float cooldownBeforeResettingGuardBreak;

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

        private void Update()
        {
            if (cooldownBeforeResettingGuardBreak < maxHitsCooldownBeforeResetGuardBreak)
            {
                cooldownBeforeResettingGuardBreak += Time.deltaTime;
            }
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
            enemyManager.animator.Play("Parry");

            playerPoiseController.IncreasePoiseDamage(1);

            playerPoiseController.GetComponent<Animator>().Play("Parried");

            if (shield != null)
            {
                Instantiate(playerParryManager.parryFx, shield.transform.position, Quaternion.identity);
            }
        }

        public void HandleBlock(Vector3 blockContactPosition, int guardBreakHitAmount)
        {
            if (currentBlockHits >= maxHitsBeforeGuardBreak)
            {
                enemyManager.animator.Play("Guard Break");
                Soundbank.instance.PlayEnemyGuardBreak();
                Time.timeScale = 0.75f;
                StartCoroutine(ResetTimeScale());
                currentBlockHits = 0;
                return;
            }

            currentBlockHits += guardBreakHitAmount;

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

        IEnumerator ResetTimeScale()
        {
            yield return new WaitForSeconds(2f);
            Time.timeScale = 1f;
        }

        /// <summary>
        /// Animation Event
        /// </summary>
        public void EvaluateDecisionAfterBlockHit()
        {
            if (Random.Range(0, 100) <= 50)
            {
                enemyManager.animator.Play("Combatting");
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
    }

}
