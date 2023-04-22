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

        private void Start()
        {
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

            if (currentPostureDamage > 0)
            {
                currentPostureDamage -= Time.deltaTime * enemyManager.enemy.postureDecreaseRate;
            }
        }

        public void TakePostureDamage()
        {
            enemyManager.enemyWeaponController.DisableAllWeaponHitboxes();

            currentPostureDamage = Mathf.Clamp(currentPostureDamage + enemyManager.enemy.postureDamagePerParry, 0, GetMaxPostureDamage());

            if (currentPostureDamage >= GetMaxPostureDamage())
            {
                BreakPosture();
            }
            else
            {
                enemyManager.animator.Play(enemyManager.hashPostureHit);
            }
        }

        void BreakPosture()
        {
            currentPostureDamage = 0f;

            stunnedParticle.gameObject.SetActive(true);
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
            return Player.instance.CalculateAIPosture(enemyManager.enemy.maxPostureDamage, enemyManager.currentLevel);
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
