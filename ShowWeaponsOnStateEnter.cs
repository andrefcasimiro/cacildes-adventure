using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class ShowWeaponsOnStateEnter : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<EquipmentGraphicsHandler>().ShowWeapons();
        }
    }

}
