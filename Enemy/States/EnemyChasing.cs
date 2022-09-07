using UnityEngine;

namespace AF
{

    public class EnemyChasing : StateMachineBehaviour
    {

        float projectileCooldown = 0f;

        Enemy enemy;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<Enemy>(out enemy);

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<Enemy>(true);
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (enemy.player.IsBusy())
            {
                animator.SetBool(enemy.hashChasing, false);
                animator.SetBool(enemy.hashPatrol, true);
                return;
            }

            enemy.agent.SetDestination(enemy.player.transform.position);

            // If projectile cooldown is over, try to shoot
            if (enemy.CanShoot())
            {
                enemy.PrepareProjectile();
                return;
            }

            float distanceBetweenEnemyAndTarget = Vector3.Distance(enemy.agent.transform.position, enemy.player.transform.position);
            if (distanceBetweenEnemyAndTarget > enemy.maximumChaseDistance)
            {
                animator.SetBool(enemy.hashChasing, false);
                animator.SetBool(enemy.hashPatrol, true);
            }
            else if (distanceBetweenEnemyAndTarget <= enemy.agent.stoppingDistance)
            {
                animator.SetBool(enemy.hashChasing, false);
                animator.SetBool(enemy.hashCombatting, true);
            }

        }

    }

}