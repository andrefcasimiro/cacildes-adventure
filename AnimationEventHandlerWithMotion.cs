using System.Collections;
using UnityEngine;

namespace AF
{
    public class AnimationEventHandlerWithMotion : AnimationEventHandler
    {
        private void OnAnimatorMove()
        {
            if (animator.applyRootMotion == false) { return; }

            Vector3 pos = animator.deltaPosition;

            pos.y = 0;

            enemy.transform.position += pos;
            enemy.transform.rotation *= animator.deltaRotation;
        }
    }
}