using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class ActivateBlockingWeaponPivotOnStateEnter : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            EquipmentGraphicsHandler equipmentGraphicsHandler = animator.gameObject.GetComponentInChildren<EquipmentGraphicsHandler>(true);
            WeaponPivotHandler weaponPivotHandler = equipmentGraphicsHandler.leftHand.GetComponentInChildren<WeaponPivotHandler>(true);


            if (weaponPivotHandler != null)
            {
                Debug.Log("weaponPivotHandler.transform.position before: " + weaponPivotHandler.transform.position);
                weaponPivotHandler.transform.localPosition = weaponPivotHandler.blockingPosition;
                weaponPivotHandler.transform.localRotation = Quaternion.Euler(new Vector3(weaponPivotHandler.blockRotationX, weaponPivotHandler.blockRotationY, weaponPivotHandler.blockRotationZ));

                Debug.Log("weaponPivotHandler.transform.position after: " + weaponPivotHandler.transform.position);
            }
        }
    }

}