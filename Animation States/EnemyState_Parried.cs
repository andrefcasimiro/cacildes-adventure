using UnityEngine;

namespace AF
{

    public class EnemyState_Parried : StateMachineBehaviour
    {
        EnemyManager enemy;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

            animator.TryGetComponent(out enemy);

            if (enemy == null)
            {
                enemy = animator.GetComponentInParent<EnemyManager>();
            }

            enemy.rigidbody.useGravity = true;
            enemy.rigidbody.isKinematic = false;
            enemy.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

            enemy.agent.updatePosition = false;
        }
    }

}
