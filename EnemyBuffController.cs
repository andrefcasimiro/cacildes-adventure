using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{

    public class EnemyBuffController : MonoBehaviour
    {
        public readonly int hashBuff = Animator.StringToHash("Buffing");

        [Header("On Buff Events")]
        public UnityEvent onBuffEvent;

        [Header("Trigger Options")]
        [Range(0, 100)]
        public int healthPercentageBeforeUsingBuff;

        [Range(0, 100)]
        public int weight; // 0% means never, 100% means always

        [Header("Cooldown")]
        public float maxBuffCooldown = 15f;

        private float buffCooldown = 0f;
        private Enemy enemy => GetComponent<Enemy>();
        private EnemyHealthController enemyHealthController => GetComponent<EnemyHealthController>();


        public bool CanUseBuff()
        {
            float currentHealthPercentage = enemyHealthController.currentHealth * 100 / enemyHealthController.maxHealth;

            if (currentHealthPercentage > healthPercentageBeforeUsingBuff)
            {
                return false;
            }

            if (Random.Range(0, 100) < weight)
            {
                return true;
            }

            return false;
        }

        /// Animation Events
        public void CastBuff()
        {
            if (this.onBuffEvent != null)
            {
                this.onBuffEvent.Invoke();
            }
        }

    }

}
