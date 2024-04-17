using System.Collections;
using AF.Health;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public abstract class CharacterAbstractPosture : MonoBehaviour
    {
        [Header("Posture")]
        public float currentPostureDamage;
        public float postureBreakBonusMultiplier = 3f;

        [Header("Unity Events")]
        public UnityEvent onPostureBreakDamage;
        public UnityEvent onDamageWhileStunned;


        [Header("Components")]
        public CharacterBaseHealth health;

        [Header("Optional AI Components")]

        public UnityEngine.UI.Slider postureBarSlider;
        public CharacterBaseManager characterBaseManager;

        public bool isStunned = false;

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

        public abstract int GetMaxPostureDamage();

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
                    currentPostureDamage -= Time.deltaTime * GetPostureDecreateRate();
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

            currentPostureDamage = Mathf.Clamp(currentPostureDamage + postureDamage, 0, GetMaxPostureDamage());
            StartCoroutine(BeginDecreasingPosture());

            if (currentPostureDamage >= GetMaxPostureDamage())
            {
                BreakPosture();
                return true;
            }

            return false;
        }

        IEnumerator BeginDecreasingPosture()
        {
            isDecreasingPosture = false;
            yield return new WaitForSeconds(1);
            isDecreasingPosture = true;
        }

        public void BreakPosture()
        {
            onPostureBreakDamage?.Invoke();
            currentPostureDamage = 0f;
            isStunned = true;
            characterBaseManager.health.PlayPostureBroke();
        }

        public void RecoverFromStunned()
        {
            isStunned = false;
            onDamageWhileStunned?.Invoke();
        }

        public abstract float GetPostureDecreateRate();
    }

}
