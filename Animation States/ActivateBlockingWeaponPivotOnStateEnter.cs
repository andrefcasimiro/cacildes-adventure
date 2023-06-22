using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class ActivateBlockingWeaponPivotOnStateEnter : StateMachineBehaviour
    {
        WeaponPivotHandler weaponPivotHandler;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            EquipmentGraphicsHandler equipmentGraphicsHandler = animator.gameObject.GetComponentInChildren<EquipmentGraphicsHandler>(true);
            weaponPivotHandler = equipmentGraphicsHandler.leftHand.GetComponentInChildren<WeaponPivotHandler>(true);


            if (weaponPivotHandler != null)
            {

                if (weaponPivotHandler.useCustomLockOnBlockingTransform && animator.GetBool("IsLockedOn"))
                {
                    weaponPivotHandler.transform.localPosition = weaponPivotHandler.lockOnBlockingPosition;
                    weaponPivotHandler.transform.localRotation = Quaternion.Euler(new Vector3(weaponPivotHandler.lockOnBlockRotationX, weaponPivotHandler.lockOnBlockRotationY, weaponPivotHandler.lockOnBlockRotationZ));
                }
                else
                {
                    weaponPivotHandler.transform.localPosition = weaponPivotHandler.blockingPosition;
                    weaponPivotHandler.transform.localRotation = Quaternion.Euler(new Vector3(weaponPivotHandler.blockRotationX, weaponPivotHandler.blockRotationY, weaponPivotHandler.blockRotationZ));
                }
            }
        }
    }
}
