using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF
{

    public class EnemyStateChasing : StateMachineBehaviour
    {

        EnemyManager enemy;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<EnemyManager>(out enemy);


            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

            if (enemy.isBoss)
            {
                if (enemy.bossMusic != null)
                {
                    if (BGMManager.instance.bgmAudioSource.clip != null && BGMManager.instance.bgmAudioSource.clip != enemy.bossMusic || BGMManager.instance.bgmAudioSource.clip == null)
                    {
                        BGMManager.instance.PlayMusic(enemy.bossMusic);
                    }
                }

                enemy.InitiateBossBattle();
            }
            else
            {
                BGMManager.instance.PlayBattleMusic();
            }

            enemy.agent.isStopped = false;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            enemy.agent.isStopped = false;
            if (enemy.agent.enabled)
            {
                if (enemy.ignoreCompanions == false && enemy.currentCompanion != null)
                {
                    enemy.agent.SetDestination(enemy.currentCompanion.transform.position);
                }
                else
                {
                    enemy.agent.SetDestination(enemy.player.transform.position);
                }
            }


            Vector3 targetPosition;

            bool focusedOnCompanion = enemy.ignoreCompanions == false && enemy.currentCompanion != null;
            if (focusedOnCompanion)
            {
                targetPosition = enemy.currentCompanion.transform.position;
            }
            else
            {
                targetPosition = enemy.player.transform.position;
            }

            float distanceBetweenEnemyAndTarget = Vector3.Distance(enemy.agent.transform.position, targetPosition);


            // Evaluate if there are any buffs long distance
            if (enemy.buffs.Length > 0)
            {
                var shuffledBuffs = Randomize(enemy.buffs.ToArray());

                var possibleBuff = shuffledBuffs.FirstOrDefault(x => x.minimumDistanceToUseBuff + enemy.agent.stoppingDistance >= enemy.agent.stoppingDistance);

                if (possibleBuff != null && distanceBetweenEnemyAndTarget > possibleBuff.minimumDistanceToUseBuff && distanceBetweenEnemyAndTarget <= possibleBuff.maximumDistanceToUseBuff)
                {
                    if (enemy.CanUseBuff(possibleBuff))
                    {
                        enemy.PrepareBuff(possibleBuff);
                    }
                }
            }

            // Evaluate if we can shoot
            if (distanceBetweenEnemyAndTarget > enemy.maximumChaseDistance / 2 && distanceBetweenEnemyAndTarget <= enemy.maximumChaseDistance)
            {
                if (enemy.isReadyToShoot == false)
                {
                    enemy.isReadyToShoot = true;
                }
                else if (enemy.CanShoot())
                {
                    enemy.PrepareProjectile();
                    return;
                }
            }

            if (distanceBetweenEnemyAndTarget > enemy.maximumChaseDistance || enemy.playerComponentManager.IsBusy())
            {
                animator.SetBool(enemy.hashChasing, false);
                animator.SetBool(enemy.hashPatrol, true);

                BGMManager.instance.PlayMapMusicAfterKillingEnemy(enemy);

                enemy.isReadyToShoot = false;
            }
            else if (distanceBetweenEnemyAndTarget <= (enemy.agent.stoppingDistance + (focusedOnCompanion ? enemy.currentCompanion.agent.stoppingDistance : 0)))
            {
                animator.SetBool(enemy.hashChasing, false);
                animator.SetBool(enemy.hashCombatting, true);
            }

        }

        IEnumerable<Buff> Randomize(Buff[] source)
        {
            System.Random rnd = new System.Random();
            return source.OrderBy((item) => rnd.Next());
        }
    }

}
