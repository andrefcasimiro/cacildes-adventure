using UnityEngine;

namespace AF
{
    public class PlayerState_Idle : StateMachineBehaviour
    {

        PlayerInventory playerInventory;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (playerInventory == null)
            {
                animator.TryGetComponent<PlayerInventory>(out playerInventory);
            }

            if (playerInventory.currentConsumedItem != null)
            {
                playerInventory.FinishItemConsumption();
            }
        }
    }
}
