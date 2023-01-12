using UnityEngine;

namespace AF
{

    public class DeactivateHitboxOnExit : StateMachineBehaviour
    {
        EnemyManager enemy;

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent(out enemy);

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>(true);
            }

            enemy.DeactivateAreaOfImpactHitbox();
            enemy.DeactivateHeadHitbox();
            enemy.DeactivateLeftHandHitbox();
            enemy.DeactivateLeftLegHitbox();
            enemy.DeactivateRightHandHitbox();
            enemy.DeactivateRightLegHitbox();
        }
    }

}
