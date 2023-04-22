using UnityEngine;

namespace AF
{
    public class PlayerState_CheckIfShouldShowShieldOnStateEnter : StateMachineBehaviour
    {
        public bool shouldHideShield = false;

        EquipmentGraphicsHandler equipmentGraphicsHandler;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (equipmentGraphicsHandler == null)
            {
                equipmentGraphicsHandler = animator.GetComponent<EquipmentGraphicsHandler>();
            }

            if (Player.instance.equippedWeapon != null && Player.instance.equippedWeapon.hideShield)
            {
                if (shouldHideShield)
                {
                    equipmentGraphicsHandler.HideShield();
                    return;
                }
            }

            equipmentGraphicsHandler.ShowShield();
        }
    }
}
