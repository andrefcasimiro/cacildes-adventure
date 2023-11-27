
using UnityEngine;

namespace AF
{

    public class AnimatorUnityEventHandler: MonoBehaviour
    {
        public Animator animator;
        public string boolName = "myBool";

        public void SetBool(bool value)
        {
            animator.SetBool(boolName, value);
        }
    }
}
