using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    [System.Serializable]
    public class AnimatorBool
    {
        public string name;
        public bool value;
    }

    public class ResetBoolOnStateEnter : StateMachineBehaviour
    {
        public List<AnimatorBool> boolList = new List<AnimatorBool>();

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            foreach (AnimatorBool b in boolList)
            {
                animator.SetBool(b.name, b.value);
            }
        }

    }

}
