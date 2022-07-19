using UnityEngine;

namespace AF
{

    public class ClimbPhysics : StateMachineBehaviour
    {
        Player player;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.gameObject.TryGetComponent<Player>(out player);

            if (player == null)
            {
                return;
            }

            player.rigidbody.useGravity = false;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (player == null)
            {
                return;
            }

        }

        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (player == null)
            {
                return;
            }


            player.rigidbody.useGravity = true;
        }

    }

}