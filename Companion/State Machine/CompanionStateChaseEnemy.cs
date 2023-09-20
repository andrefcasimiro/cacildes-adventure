using UnityEngine;

namespace AF
{

    public class CompanionStateChaseEnemy : StateMachineBehaviour
    {
        CompanionManager companionManager;

        Vector3 previousPosition;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (companionManager == null)
            {
                animator.gameObject.TryGetComponent<CompanionManager>(out companionManager);

                companionManager = animator.GetComponentInParent<CompanionManager>(true);
            }

            companionManager.agent.speed = companionManager.runSpeed;

            companionManager.agent.isStopped = false;

            companionManager.FaceEnemy();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            companionManager.FaceEnemy();

            if (companionManager.currentEnemy == null || companionManager.currentEnemy.enemyHealthController.currentHealth <= 0)
            {
                animator.SetBool(companionManager.hashIsChasingEnemy, false);
                animator.SetBool(companionManager.hashIsCombatting, false);
                return;
            }

            if (
                companionManager.IsEnemyFarAway() == false)
            {
                animator.SetBool(companionManager.hashIsCombatting, true);
                animator.SetBool(companionManager.hashIsChasingEnemy, false);
                return;
            }

            if (!companionManager.agent.SetDestination(companionManager.currentEnemy.transform.position))
            {
                companionManager.agent.ResetPath();
            }

        }

    }

}
