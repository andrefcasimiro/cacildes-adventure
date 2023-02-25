using UnityEngine;

namespace AF
{
    public class PlayerCheckForWeaponHandlerRef : StateMachineBehaviour
    {
        EquipmentGraphicsHandler equipmentGraphicsHandler;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            equipmentGraphicsHandler = animator.GetComponent<EquipmentGraphicsHandler>();

            if (equipmentGraphicsHandler != null)
            {
                equipmentGraphicsHandler.AssignWeaponHandlerRefs();
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (equipmentGraphicsHandler != null)
            {
                equipmentGraphicsHandler.AssignWeaponHandlerRefs();
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (equipmentGraphicsHandler != null)
            {
                equipmentGraphicsHandler.UnassignWeaponHandlerRefs();
            }
        }
    }
}
