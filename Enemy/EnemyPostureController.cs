using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class EnemyPostureController : MonoBehaviour
    {

        [Header("Posture")]
        public float currentPostureDamage;
        public UnityEngine.UI.Slider postureBarSlider;
        public GameObject stunnedParticle;

        EnemyManager enemyManager => GetComponent<EnemyManager>();

        public bool isParriable = false;

        bool isDecreasingPosture = false;

        ParticlePoolManager particlePoolManager;

        PlayerAchievementsManager playerAchievementsManager;

        private void Awake()
        {
            particlePoolManager = FindAnyObjectByType<ParticlePoolManager>(FindObjectsInactive.Include);
        }

        private void Start()
        {
            if (enemyManager.player != null)
            {
                playerAchievementsManager = enemyManager.player.GetComponent<PlayerAchievementsManager>();
            }

            InitializePostureHUD();
        }

        private void Update()
        {
            UpdatePosture();
        }

        public void InitializePostureHUD()
        {
            if (postureBarSlider != null)
            {
                postureBarSlider.maxValue = GetMaxPostureDamage() * 0.01f;
                postureBarSlider.value = currentPostureDamage * 0.01f;
                postureBarSlider.gameObject.SetActive(false);
            }

        }

        void UpdatePosture()
        {
            if (enemyManager.enemyHealthController.currentHealth <= 0)
            {
                return;
            }

            if (postureBarSlider != null)
            {
                postureBarSlider.value = currentPostureDamage * 0.01f;
                postureBarSlider.gameObject.SetActive(currentPostureDamage > 0);
            }

            if (isDecreasingPosture)
            {
                if (currentPostureDamage > 0)
                {
                    currentPostureDamage -= Time.deltaTime * enemyManager.enemy.postureDecreaseRate;
                }
                else
                {
                    isDecreasingPosture = false;
                }
            }
        }

        public bool TakePostureDamage(int extraPostureDamage)
        {
            enemyManager.enemyWeaponController.DisableAllWeaponHitboxes();

            var postureDamage = enemyManager.enemy.postureDamagePerParry;
            if (extraPostureDamage != 0)
            {
                postureDamage = extraPostureDamage;
            }

            currentPostureDamage = Mathf.Clamp(currentPostureDamage + postureDamage, 0, GetMaxPostureDamage());
            StartCoroutine(BeginDecreasingPosture());

            if (currentPostureDamage >= GetMaxPostureDamage())
            {
                StartCoroutine(PlayStunnedFX());

                BreakPosture();
                return true;
            }
            else if (extraPostureDamage == 0) // If damage from parry, play parry animation
            {
                enemyManager.animator.Play(enemyManager.hashPostureHit);
            }

            return false;
        }

        IEnumerator PlayStunnedFX()
        {

            var particle = particlePoolManager.stunnedFxPool.Pool.Get();
            if (particle == null)
            {
                yield break;
            }

            particle.transform.position = this.transform.position;
            particle.transform.rotation = Quaternion.identity;
            particle.Play();

            yield return new WaitForSeconds(5f);

            particlePoolManager.stunnedFxPool.Pool.Release(particle);

        }

        IEnumerator BeginDecreasingPosture()
        {
            isDecreasingPosture = false;
            yield return new WaitForSeconds(1);
            isDecreasingPosture = true;
        }

        void BreakPosture()
        {
            if (playerAchievementsManager != null)
            {
                playerAchievementsManager.achievementForBreakingEnemyStance.AwardAchievement();
            }

            currentPostureDamage = 0f;

            Soundbank.instance.PlayEnemyGuardBreak();

            //stunnedParticle.gameObject.SetActive(true);
            enemyManager.animator.Play(enemyManager.hashPostureBreak);
        }

        public void RecoverPosture()
        {
            stunnedParticle.gameObject.SetActive(false);
        }

        public bool IsStunned()
        {
            return enemyManager.animator.GetBool(enemyManager.hashIsStunned);
        }

        int GetMaxPostureDamage()
        {
            if (enemyManager.currentLevel <= 1)
            {
                return enemyManager.enemy.maxPostureDamage;
            }

            return Formulas.CalculateAIPosture(enemyManager.enemy.maxPostureDamage, enemyManager.currentLevel);
        }

        public void ShowHUD()
        {
            postureBarSlider.gameObject.SetActive(true);
        }
        public void HideHUD()
        {
            postureBarSlider.gameObject.SetActive(false);
        }

    }

}
