using UnityEngine;

namespace AF
{

    public class PlayerShowWeaponsOnStateEnter : StateMachineBehaviour
    {
        PlayerManager playerManager;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (playerManager == null)
            {
                animator.TryGetComponent(out playerManager);
            }
            // If blocking with shield, skip show weapons
            if (playerManager.equipmentGraphicsHandler.shieldGraphic != null
                && playerManager.equipmentGraphicsHandler.shieldGraphic.gameObject.activeSelf && animator.GetBool("IsBlocking"))
            {
                return;
            }

            playerManager.equipmentGraphicsHandler.ShowWeapons();
        }
    }

}
