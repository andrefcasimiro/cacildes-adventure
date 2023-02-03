using System.Collections;
using UnityEngine;

namespace AF
{
    public class AnimationEventHandlerWithMotion : AnimationEventHandler
    {
        private void OnAnimatorMove()
        {
            if (animator.applyRootMotion == false) { return; }
            bool agentIsEnabled = enemy.agent.enabled;

            if (agentIsEnabled)
            {
                enemy.agent.enabled = false;
            }
            Vector3 pos = animator.deltaPosition;

            pos.y = 0;

            enemy.transform.position += pos;

            var rot = animator.deltaRotation;
            rot.y = 0;
            enemy.transform.rotation *= rot;

            if (agentIsEnabled)
            {
                enemy.agent.nextPosition = enemy.transform.position;
                enemy.agent.enabled = true;
            }
        }
    }
}