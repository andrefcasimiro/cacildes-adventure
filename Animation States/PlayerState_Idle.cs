using UnityEngine;

namespace AF
{
    public class PlayerState_Idle : StateMachineBehaviour
    {
        PlayerManager playerManager;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (playerManager == null)
            {
                animator.TryGetComponent(out playerManager);
            }

            playerManager.ResetStates();
        }
    }
}
