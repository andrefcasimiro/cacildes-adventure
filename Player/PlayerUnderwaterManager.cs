namespace AF
{
    using System.Collections;
    using UnityEngine;

    public class PlayerUnderwaterManager : MonoBehaviour
    {

        public bool canBreathUnderwater = false;
        public bool allowUnderwaterDamage = false;

        [Header("Player")]
        public PlayerManager playerManager;


        [Header("Settings")]
        public bool isUnderwaterMap = false;

        [Header("Cooldown")]
        public float maxIntervalBetweenUnderwaterDamage = 3f;
        float interval = 0f;

        public float underwaterDamage = 25f;

        private void Awake()
        {
            if (!isUnderwaterMap)
            {
                this.gameObject.SetActive(false);
            }
        }

        void Start()
        {
            StartCoroutine(AllowUnderwaterDamage_Coroutine());
        }

        IEnumerator AllowUnderwaterDamage_Coroutine()
        {
            yield return new WaitForSeconds(2f);

            allowUnderwaterDamage = true;
        }

        public void SetCanBreathUnderwater(bool value)
        {
            this.canBreathUnderwater = value;
        }

        void Update()
        {
            if (!this.canBreathUnderwater && allowUnderwaterDamage)
            {
                interval += Time.deltaTime;

                if (interval > maxIntervalBetweenUnderwaterDamage)
                {
                    interval = 0;

                    playerManager.health.TakeDamage(underwaterDamage);
                }
            }
        }
    }
}
