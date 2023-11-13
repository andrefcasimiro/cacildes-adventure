using UnityEngine;

namespace AF
{
    public class PlayerState_CheckIfShouldShowShieldOnStateEnter : StateMachineBehaviour
    {
        public bool shouldHideShield = false;

        PlayerManager playerManager;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (playerManager == null)
            {
                playerManager = animator.GetComponent<PlayerManager>();
            }

            if (Player.instance.equippedWeapon != null && Player.instance.equippedWeapon.hideShield)
            {
                if (shouldHideShield)
                {
                    playerManager.equipmentGraphicsHandler.HideShield();
                    return;
                }
            }

            playerManager.equipmentGraphicsHandler.ShowShield();
        }
    }
}
