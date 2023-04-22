using UnityEngine;

namespace AF
{
    public class PlayerState_ForceHideShieldOnStateEnter : StateMachineBehaviour
    {

        EquipmentGraphicsHandler equipmentGraphicsHandler;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (equipmentGraphicsHandler == null)
            {
                equipmentGraphicsHandler = animator.GetComponent<EquipmentGraphicsHandler>();
            }

            equipmentGraphicsHandler.ForceHideShield();
        }
    }
}
