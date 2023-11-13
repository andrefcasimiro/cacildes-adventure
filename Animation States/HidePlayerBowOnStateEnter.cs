using UnityEngine;

namespace AF
{
    public class HidePlayerBowOnStateEnter : StateMachineBehaviour
    {
        PlayerManager playerManager;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (playerManager == null)
            {
                playerManager = animator.GetComponent<PlayerManager>();
            }

            if (playerManager.equipmentGraphicsHandler.bow == null)
            {
                return;
            }

            playerManager.equipmentGraphicsHandler.bow.gameObject.SetActive(false);
        }
    }
}
