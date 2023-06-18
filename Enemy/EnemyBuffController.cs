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
            public string buffName;

            public string animationTag;

            public string animationStartName;

            public bool isActive = true;

            public float duration = -1f;

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
            public bool disengageLockOn = false;

            [Header("Frequency")]
            [Tooltip("Higher means more often")][Range(0, 100)] public float weight = 30f;
        }

        [Header("Buffs")]
        public Buff[] buffs;
        [HideInInspector] public List<Buff> usedBuffs = new List<Buff>();
        public AudioClip enemyBuffSfx;

        EnemyManager enemyManager => GetComponent<EnemyManager>();

        LockOnManager lockOnManager;

        private void Awake()
        {
            lockOnManager = FindObjectOfType<LockOnManager>(true);
        }

        private void Update()
        {
            UpdateBuffs();
        }

        public void PlayEnemyBuffSfx()
        {
            BGMManager.instance.PlaySound(enemyBuffSfx, enemyManager.combatAudioSource);
        }

        public void PickRandomBuff()
        {
            if (buffs.Length > 0)
            {
                var shuffledBuffs = Randomize(buffs.ToArray());

                var possibleBuff = shuffledBuffs.FirstOrDefault(x => x.minimumDistanceToUseBuff + enemyManager.agent.stoppingDistance >= enemyManager.agent.stoppingDistance);
                float distanceBetweenEnemyAndTarget = Vector3.Distance(enemyManager.agent.transform.position,enemyManager.player.transform.position);

                if (possibleBuff != null)
                {
                    if (CanUseBuff(possibleBuff))
                    {
                        PrepareBuff(possibleBuff);

                    }
                }
            }
        }

        IEnumerable<EnemyBuffController.Buff> Randomize(EnemyBuffController.Buff[] source)
        {
            System.Random rnd = new System.Random();
            return source.OrderBy((item) => rnd.Next());
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
            if (!buff.isActive)
            {
                return false;
            }

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
                || distanceToPlayer < buff.minimumDistanceToUseBuff)
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

            if (buff.stopMovementWhileCasting && enemyManager.agent.isActiveAndEnabled)
            {
                enemyManager.agent.isStopped = true;
            }

            enemyManager.animator.Play(buff.animationStartName);

            buff.onBuffEventStart.Invoke();

            if (buff.disengageLockOn)
            {
                lockOnManager.DisableLockOn();
            }

            if (buff.weaponToShow != null)
            {
                StartCoroutine(ShowWeaponAfter(buff));
            }

            if (buff.duration != -1f)
            {
                StartCoroutine(EndBuffAfter(buff));
            }
        }

        IEnumerator EndBuffAfter(Buff buff)
        {
            yield return new WaitForSeconds(buff.duration);

            buff.onBuffEventEnd.Invoke();
            enemyManager.facePlayer = false;
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

            var currentAnimation = enemyManager.animator.GetCurrentAnimatorStateInfo(0);
            var targetBuff = buffs.FirstOrDefault(buff => currentAnimation.IsTag(buff.animationTag));

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

            var currentAnimation = enemyManager.animator.GetCurrentAnimatorStateInfo(0);
            var targetBuff = buffs.FirstOrDefault(buff => currentAnimation.IsTag(buff.animationTag));

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
