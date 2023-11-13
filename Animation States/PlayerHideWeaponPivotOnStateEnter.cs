using UnityEngine;

namespace AF
{
    public class PlayerHideWeaponPivotOnStateEnter : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            return;
            EquipmentGraphicsHandler equipmentGraphicsHandler = animator.gameObject.GetComponentInChildren<EquipmentGraphicsHandler>(true);
            WeaponPivotHandler weaponPivotHandler = equipmentGraphicsHandler.leftHand.GetComponentInChildren<WeaponPivotHandler>(true);

            if (weaponPivotHandler != null)
            {
                weaponPivotHandler.transform.localPosition = weaponPivotHandler.originalPosition;
                weaponPivotHandler.transform.localRotation = weaponPivotHandler.originalRotation;
            }
        }
    }
}
