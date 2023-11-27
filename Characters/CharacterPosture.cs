using System.Collections;
using System.Collections.Generic;
using AF.Events;
using AF.Health;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class CharacterPosture : MonoBehaviour
    {
        [Header("Posture")]
        public float currentPostureDamage;
        public int maxPostureDamage = 100;
        public float postureBreakBonusMultiplier = 3f;

        [Header("Unity Events")]
        public UnityEvent onPostureBreakDamage;

        [Header("Components")]
        public CharacterBaseHealth health;

        [Header("Optional AI Components")]

        public UnityEngine.UI.Slider postureBarSlider;
        public CharacterManager characterManager;

        private bool isStunned = false;

        private bool isDecreasingPosture = false;

        private void Start()
        {
            InitializePostureHUD();
        }

        private void Update()
        {
            UpdatePosture();
        }

        public void ResetStates()
        {
            isStunned = false;
        }

        public void InitializePostureHUD()
        {
            if (postureBarSlider != null)
            {
                postureBarSlider.maxValue = maxPostureDamage * 0.01f;
                postureBarSlider.value = currentPostureDamage * 0.01f;
                postureBarSlider.gameObject.SetActive(false);
            }
        }

        void UpdatePosture()
        {
            if (health.GetCurrentHealth() <= 0)
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
                    currentPostureDamage -= Time.deltaTime * 1;
                }
                else
                {
                    isDecreasingPosture = false;
                }
            }
        }

        public bool TakePostureDamage(int extraPostureDamage)
        {
            var postureDamage = 0;
            if (extraPostureDamage != 0)
            {
                postureDamage = extraPostureDamage;
            }

            currentPostureDamage = Mathf.Clamp(currentPostureDamage + postureDamage, 0, maxPostureDamage);
            StartCoroutine(BeginDecreasingPosture());

            if (currentPostureDamage >= maxPostureDamage)
            {
                BreakPosture();
                return true;
            }

            return false;
        }

        public bool WillBreakPosture(Damage incomingDamage)
        {
            return currentPostureDamage + incomingDamage.postureDamage > maxPostureDamage;
        }


        IEnumerator BeginDecreasingPosture()
        {
            isDecreasingPosture = false;
            yield return new WaitForSeconds(1);
            isDecreasingPosture = true;
        }

        void BreakPosture()
        {
            onPostureBreakDamage?.Invoke();
            currentPostureDamage = 0f;
            isStunned = true;
        }

        public bool IsStunned()
        {
            return isStunned;
        }

    }

}
