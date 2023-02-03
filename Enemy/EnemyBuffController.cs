using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace AF
{
    public class EnemyBuffController : MonoBehaviour
    {

        [System.Serializable]
        public class Buff
        {
            [Header("Animation That Starts")]
            public string animationStartName;
            public string animationLoopName;
            public string animationEndName;

            public UnityEvent onBuffEventStart;
            public UnityEvent onBuffEventCast;
            public UnityEvent onBuffEventEnd;
            public UnityEvent onBuffInterrupted;

            [Header("Conditions")]
            [Range(0, 100f)]
            public float maximumHealthThresholdBeforeUsingBuff = 100;
            [Range(0, 100f)]
            public float minimumHealthThresholdBeforeUsingBuff = 0;
            public float minimumDistanceToUseBuff = 0f;
            public float maximumDistanceToUseBuff = 15f;

            [Header("Cooldowns")]
            public float currentBuffCooldown = 0f;
            public float maxBuffCooldown = 15f;

            [Header("Options")]
            public bool facePlayer = true;
            public bool stopMovementWhileCasting = true;

            [Header("Pistol Or Bow")]
            public GameObject weaponToShow;
            public float showWeaponAfter;

            [Header("Frequency")]
            [Tooltip("Higher means more often")][Range(0, 100)] public float weight = 30f;
        }

        [Header("Buffs")]
        public Buff[] buffs;
        [HideInInspector] public List<Buff> usedBuffs = new List<Buff>();
        public AudioClip enemyBuffSfx;

        EnemyManager enemyManager => GetComponent<EnemyManager>();

        private void Update()
        {
            UpdateBuffs();
        }

        public void PlayEnemyBuffSfx()
        {
            BGMManager.instance.PlaySound(enemyBuffSfx, enemyManager.combatAudioSource);
        }

        void UpdateBuffs()
        {
            if (usedBuffs.Count <= 0)
            {
                return;
            }

            var copiedUsedBuffs = usedBuffs.ToArray();
            foreach (var usedBuff in copiedUsedBuffs)
            {
                usedBuff.currentBuffCooldown += Time.deltaTime;

                if (usedBuff.currentBuffCooldown >= usedBuff.maxBuffCooldown)
                {
                    usedBuffs.Remove(usedBuff);
                }
            }
        }

        public bool CanUseBuff(Buff buff)
        {
            if (IsUsingBuff())
            {
                return false;
            }

            float currentHealthPercentage = enemyManager.enemyHealthController.currentHealth * 100 / enemyManager.enemyHealthController.GetMaxHealth();

            if (currentHealthPercentage <= 0)
            {
                return false;
            }

            if (usedBuffs.Contains(buff))
            {
                return false;
            }

            if (currentHealthPercentage < buff.minimumHealthThresholdBeforeUsingBuff || currentHealthPercentage > buff.maximumHealthThresholdBeforeUsingBuff)
            {
                return false;
            }

            var distanceToPlayer = Vector3.Distance(transform.position, enemyManager.player.transform.position);
            if (
                distanceToPlayer > buff.maximumDistanceToUseBuff
                || distanceToPlayer < buff.minimumDistanceToUseBuff + GetComponent<NavMeshAgent>().stoppingDistance)
            {
                return false;
            }

            return true;
        }

        public bool IsUsingBuff()
        {
            return enemyManager.animator.GetBool(enemyManager.hashIsBuffing);
        }

        public void InterruptAllBuffs()
        {
            if (usedBuffs.Count <= 0)
            {
                return;
            }

            foreach (var usedBuff in enemyManager.enemyBuffController.usedBuffs)
            {
                usedBuff.onBuffInterrupted.Invoke();
            }
        }

        public void PrepareBuff(Buff buff)
        {
            buff.currentBuffCooldown = 0;
            this.usedBuffs.Add(buff);

            if (buff.facePlayer)
            {
                enemyManager.facePlayer = true;
            }

            if (buff.stopMovementWhileCasting)
            {
                enemyManager.agent.isStopped = true;
            }

            enemyManager.animator.CrossFade(buff.animationStartName, 0.1f);

            buff.onBuffEventStart.Invoke();

            if (buff.weaponToShow != null)
            {
                StartCoroutine(ShowWeaponAfter(buff));
            }
        }

        IEnumerator ShowWeaponAfter(Buff buff)
        {
            yield return new WaitForSeconds(buff.showWeaponAfter);

            buff.weaponToShow.gameObject.SetActive(true);
        }

        // Animation Event
        public void OnBuffStart()
        {
            if (buffs.Length <= 0)
            {
                return;
            }

            var targetBuff = buffs.FirstOrDefault(x =>
                enemyManager.animator.GetCurrentAnimatorStateInfo(0).IsName(x.animationStartName)
                || enemyManager.animator.GetCurrentAnimatorStateInfo(0).IsName(x.animationLoopName)
                || enemyManager.animator.GetCurrentAnimatorStateInfo(0).IsName(x.animationEndName));

            if (targetBuff == null)
            {
                return;
            }

            targetBuff.onBuffEventCast.Invoke();
        }

        public void OnBuffEnd()
        {
            if (buffs.Length <= 0)
            {
                return;
            }

            var targetBuff = buffs.FirstOrDefault(x =>
                enemyManager.animator.GetCurrentAnimatorStateInfo(0).IsName(x.animationStartName)
                || enemyManager.animator.GetCurrentAnimatorStateInfo(0).IsName(x.animationLoopName)
                || enemyManager.animator.GetCurrentAnimatorStateInfo(0).IsName(x.animationEndName));

            if (targetBuff == null)
            {
                return;
            }

            targetBuff.onBuffEventEnd.Invoke();

            if (targetBuff.facePlayer)
            {
                enemyManager.facePlayer = false;
            }
        }

        public IEnumerable<Buff> GetShuffledBuffs()
        {
            var source = buffs.ToArray();
            System.Random rnd = new System.Random();
            return source.OrderBy((item) => rnd.Next());
        }
    }

}
