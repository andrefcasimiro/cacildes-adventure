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
                weaponPivotHandler.transform.localPosition = weaponPivotHandler.blockingPosition;
                weaponPivotHandler.transform.localRotation = Quaternion.Euler(new Vector3(weaponPivotHandler.blockRotationX, weaponPivotHandler.blockRotationY, weaponPivotHandler.blockRotationZ));
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
    }

}