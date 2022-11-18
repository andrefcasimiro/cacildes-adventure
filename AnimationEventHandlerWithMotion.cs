using System.Collections;
using UnityEngine;

namespace AF
{
    public class AnimationEventHandlerWithMotion : AnimationEventHandler
    {
        private void OnAnimatorMove()
        {
            if (animator.applyRootMotion == false) { return; }

            enemy.transform.position += animator.deltaPosition;
        }
    }
}