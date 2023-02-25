using UnityEngine;

namespace AF {
    public class PlayerShowShieldOnStateEnter : StateMachineBehaviour
    {
        EquipmentGraphicsHandler equipmentGraphicsHandler;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (equipmentGraphicsHandler == null)
            {
                equipmentGraphicsHandler = animator.GetComponent<EquipmentGraphicsHandler>();
            }

            equipmentGraphicsHandler.HideWeapons();
            equipmentGraphicsHandler.ShowShield();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.GetBool("IsBlocking") == false)
            {
                equipmentGraphicsHandler.HideShield();
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            equipmentGraphicsHandler.ShowWeapons();
            equipmentGraphicsHandler.HideShield();
        }
    }
}
