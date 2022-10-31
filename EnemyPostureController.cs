using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AF
{
    public class EnemyPostureController : MonoBehaviour
    {
        public readonly int hashPostureHit = Animator.StringToHash("PostureHit");
        public readonly int hashPostureBreak = Animator.StringToHash("PostureBreak");
        public readonly int hashIsStunned = Animator.StringToHash("IsStunned");

        public Animator animator;

        public float bonusMultiplier = 3f;

        public float postureDamagePerParry = .25f;

        public float currentPostureDamage;
        public float maxPostureDamage;

        public Slider postureBarSlider;

        public GameObject stunnedParticle;

        public float decreaseRate = .5f;

        EnemyCombatController enemyCombatController => GetComponent<EnemyCombatController>();

        private void Start()
        {
            if (postureBarSlider != null)
            {
                postureBarSlider.maxValue = maxPostureDamage * 0.01f;
                postureBarSlider.value = currentPostureDamage * 0.01f;
            }
        }

        private void Update()
        {
            if (postureBarSlider != null)
            {
                postureBarSlider.value = currentPostureDamage * 0.01f;
                postureBarSlider.gameObject.SetActive(currentPostureDamage > 0);
            }

            if (currentPostureDamage > 0)
            {
                currentPostureDamage -= Time.deltaTime * decreaseRate;
            }
        }

        public void TakePostureDamage()
        {
            enemyCombatController.DisableAllWeaponHitboxes();

            currentPostureDamage = Mathf.Clamp(currentPostureDamage + postureDamagePerParry, 0, maxPostureDamage);

            if (currentPostureDamage >= maxPostureDamage)
            {
                BreakPosture();
            }
            else
            {
                animator.CrossFade(hashPostureHit, 0.05f);
            }
        }

         void BreakPosture()
        {
            stunnedParticle.gameObject.SetActive(true);
            animator.CrossFade(hashPostureBreak, 0.05f);
        }

        public void RecoverPosture()
        {
            stunnedParticle.gameObject.SetActive(false);
            currentPostureDamage = 0f;
        }

        public bool IsStunned()
        {
            return animator.GetBool(hashIsStunned);
        }

    }

}
