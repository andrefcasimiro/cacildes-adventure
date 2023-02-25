using UnityEngine;

namespace AF
{

    public class PlayerShowWeaponsOnStateEnter : StateMachineBehaviour
    {
        EquipmentGraphicsHandler equipmentGraphicsHandler;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (equipmentGraphicsHandler == null)
            {
                equipmentGraphicsHandler = animator.GetComponent<EquipmentGraphicsHandler>();
            }

            // If blocking with shield, skip show weapons
            if (equipmentGraphicsHandler.shieldGraphic != null
                && equipmentGraphicsHandler.shieldGraphic.gameObject.activeSelf && animator.GetBool("IsBlocking"))
            {
                return;
            }

            equipmentGraphicsHandler.ShowWeapons();
        }
    }

}
