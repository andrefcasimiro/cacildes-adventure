using UnityEngine;

namespace AF
{
    public class PlayerState_Idle : StateMachineBehaviour
    {

        PlayerInventory playerInventory;
        DodgeController dodgeController;

        CharacterController characterController;
        PlayerComponentManager playerComponentManager;
        ThirdPersonController thirdPersonController;

        PlayerCombatController combatController;
        EquipmentGraphicsHandler equipmentGraphicsHandler;

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

            if (dodgeController == null)
            {
                animator.GetComponent<DodgeController>();
            }
            else
            {
                dodgeController.hasIframes = false;
            }

            if (equipmentGraphicsHandler == null)
            {
                equipmentGraphicsHandler = animator.GetComponent<EquipmentGraphicsHandler>();
            }

            if (characterController == null)
            {
                characterController = animator.GetComponent<CharacterController>();
            }

            if (combatController == null)
            {
                combatController = animator.GetComponent<PlayerCombatController>();
            }

            if (playerComponentManager == null)
            {
                playerComponentManager = animator.GetComponent<PlayerComponentManager>();
            }
            else
            {
                playerComponentManager.EnableCollisionWithEnemies();
            }

            combatController.skipIK = false;
        }
    }
}
