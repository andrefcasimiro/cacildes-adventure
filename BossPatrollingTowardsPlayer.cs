using UnityEngine;

namespace AF
{

    public class BossPatrollingTowardsPlayer : StateMachineBehaviour
    {
        Enemy enemy;
        EnemyPathController enemyPathController;

        public float walkingSpeed = 1.5f;

        float defaultSpeed;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<Enemy>(out enemy);
            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<Enemy>(true);
            }

            if (enemyPathController == null)
            {
                enemyPathController = enemy.GetComponent<EnemyPathController>();
            }

            defaultSpeed = enemy.agent.speed;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = walkingSpeed;

            if (Vector3.Distance(animator.transform.position, enemy.playerCombatController.transform.position) >= enemy.maximumChaseDistance)
            {
                enemy.agent.SetDestination(enemy.playerCombatController.transform.position);
            }
            else
            {
                animator.SetBool(enemyPathController.hashPatrol, false);
                animator.SetBool(enemy.hashChasing, true);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            enemy.agent.speed = defaultSpeed;
        }
    }

}
