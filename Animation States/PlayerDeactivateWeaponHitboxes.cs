using UnityEngine;

namespace AF
{
    public class PlayerDeactivateWeaponHitboxes : StateMachineBehaviour
    {
        PlayerManager playerManager;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (playerManager == null)
            {
                playerManager = animator.GetComponent<PlayerManager>();
            }

            playerManager.equipmentGraphicsHandler.DeactivateAllHitboxes();
        }
    }
}
