using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF
{

    public class EnemyState_Chasing : StateMachineBehaviour
    {
        EnemyManager enemy;
        PlayerComponentManager playerComponentManager;

        public float chaseSpeedOverride = -1f;

        float agentSpeed = -1f;


        bool hasDecidedRunAttack = false;


        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            hasDecidedRunAttack = false;

            animator.gameObject.TryGetComponent<EnemyManager>(out enemy);

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

            if (playerComponentManager == null)
            {
                playerComponentManager = enemy.player.GetComponent<PlayerComponentManager>();
            }

            if (agentSpeed == -1)
            {
                agentSpeed = enemy.agent.speed;
            }

            enemy.RepositionNavmeshAgent();

            if (enemy.enemyHealthController != null)
            {
                enemy.enemyHealthController.ShowHUD();
            }

            if (enemy.enemyBossController != null)
            {
                if (enemy.enemyBossController.bossMusic != null)
                {
                    if (BGMManager.instance.IsPlayingMusicClip(enemy.enemyBossController.bossMusic.name) == false)
                    {
                        BGMManager.instance.PlayMusic(enemy.enemyBossController.bossMusic);
                    }
                }

                enemy.enemyBossController.BeginBossBattle();
            }
            else
            {
                BGMManager.instance.PlayBattleMusic();
            }

            if (chaseSpeedOverride != -1)
            {
                enemy.agent.speed = chaseSpeedOverride;
            }

            if (enemy.chaseVelocityOverride != -1)
            {
                enemy.agent.speed = enemy.chaseVelocityOverride;
            }

            if (enemy.agent.enabled)
            {
                if (enemy.enemyTargetController.ignoreCompanions == false && enemy.enemyTargetController.currentCompanion != null)
                {
                    enemy.agent.SetDestination(enemy.enemyTargetController.currentCompanion.transform.position);
                }
                else
                {
                    enemy.agent.SetDestination(enemy.player.transform.position);
                }
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            if (enemy.agent.enabled)
            {
                if (enemy.enemyTargetController.ignoreCompanions == false && enemy.enemyTargetController.currentCompanion != null)
                {
                    enemy.agent.SetDestination(enemy.enemyTargetController.currentCompanion.transform.position);
                }
                else
                {
                    enemy.agent.SetDestination(enemy.player.transform.position);
                }
            }

            Vector3 targetPosition;

            bool focusedOnCompanion = enemy.enemyTargetController.ignoreCompanions == false && enemy.enemyTargetController.currentCompanion != null;
            if (focusedOnCompanion)
            {
                targetPosition = enemy.enemyTargetController.currentCompanion.transform.position;
            }
            else
            {
                targetPosition = enemy.player.transform.position;
            }

            float distanceBetweenEnemyAndTarget = Vector3.Distance(enemy.agent.transform.position, targetPosition);


            // Evaluate if there are any buffs long distance
            if (enemy.enemyBuffController != null && enemy.enemyBuffController.buffs.Length > 0)
            {
                var shuffledBuffs = Randomize(enemy.enemyBuffController.buffs.ToArray());

                var possibleBuff = shuffledBuffs.FirstOrDefault(x => x.minimumDistanceToUseBuff + enemy.agent.stoppingDistance >= enemy.agent.stoppingDistance);

                if (possibleBuff != null && distanceBetweenEnemyAndTarget > possibleBuff.minimumDistanceToUseBuff && distanceBetweenEnemyAndTarget <= possibleBuff.maximumDistanceToUseBuff)
                {
                    if (enemy.enemyBuffController.CanUseBuff(possibleBuff))
                    {
                        enemy.enemyBuffController.PrepareBuff(possibleBuff);

                        enemy.agent.updatePosition = false;
                        return;
                    }
                }
            }

            // Evaluate if we can shoot
            if (enemy.enemyProjectileController != null
                && distanceBetweenEnemyAndTarget > enemy.maximumChaseDistance / 2
                && distanceBetweenEnemyAndTarget <= enemy.maximumChaseDistance)
            {
                if (enemy.enemyProjectileController.isReadyToShoot == false)
                {
                    enemy.enemyProjectileController.isReadyToShoot = true;
                }
                else if (enemy.enemyProjectileController != null && enemy.enemyProjectileController.CanShoot())
                {
                    enemy.enemyProjectileController.PrepareProjectile();
                    enemy.agent.updatePosition = false;
                    return;
                }
            }

            // Change to patrol
            if (distanceBetweenEnemyAndTarget > enemy.maximumChaseDistance || playerComponentManager.IsBusy())
            {
                animator.SetBool(enemy.hashChasing, false);
                animator.SetBool(enemy.hashPatrol, true);

                if (enemy.enemyBossController == null)
                {
                    BGMManager.instance.PlayMapMusicAfterKillingEnemy(enemy);
                }

                if (enemy.enemyProjectileController != null)
                {
                    enemy.enemyProjectileController.isReadyToShoot = false;
                }

            }
            else if (distanceBetweenEnemyAndTarget <= (enemy.agent.stoppingDistance + (focusedOnCompanion ? enemy.enemyTargetController.currentCompanion.agent.stoppingDistance : 0)))
            {
                enemy.agent.updatePosition = false;
                animator.SetBool(enemy.hashChasing, false);
                animator.SetBool(enemy.hashCombatting, true);
            }
            // We are between max and min range of  chase, let's decide if we are close to a running attack here
            else if (hasDecidedRunAttack == false)
            {
                // Since this code runs on every frame of state chasing, we need to perform this decision only once
                hasDecidedRunAttack = true;

                var runningAttack = enemy.enemyCombatController.GetRunAttack(true);
                if (runningAttack != null && Random.Range(0, 1f) > 0.85f)  // Half chance
                {
                    enemy.agent.updatePosition = false;
                    animator.SetBool(enemy.hashChasing, false);

                    enemy.FacePlayerImmediately();
                    animator.Play(runningAttack.runningAttack);
                }
            }

        }


        IEnumerable<EnemyBuffController.Buff> Randomize(EnemyBuffController.Buff[] source)
        {
            System.Random rnd = new System.Random();
            return source.OrderBy((item) => rnd.Next());
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (chaseSpeedOverride != -1)
            {
                enemy.agent.speed = agentSpeed;
            }

            hasDecidedRunAttack = false;
        }
    }


}
