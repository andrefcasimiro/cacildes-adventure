using UnityEngine;

namespace AF
{

    public class FacePlayer : StateMachineBehaviour
    {
        Enemy enemy;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<Enemy>(out enemy);
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Utils.FaceTarget(enemy.transform, enemy.player.transform);
        }
    }

}
