using System.Collections;
using UnityEngine;

namespace AF
{
    public class AnimationEventHandlerWithMotion : AnimationEventHandler
    {
        public bool syncCharacterController = true;

        private void OnAnimatorMove()
        {
            if (animator.applyRootMotion == false) { return; }
            bool agentIsEnabled = enemy.agent.isActiveAndEnabled;

            if (agentIsEnabled)
            {
                enemy.agent.enabled = false;
            }

            Vector3 pos = animator.deltaPosition;
            pos.y = 0;

            if (syncCharacterController) { 
                // Use CharacterController to move the character
                enemy.characterController.Move(pos);
            }

            var rot = animator.deltaRotation;
            rot.y = 0;

            // Update the character's rotation
            enemy.transform.rotation *= Quaternion.Euler(rot.eulerAngles);

            if (agentIsEnabled)
            {
                enemy.agent.nextPosition = enemy.transform.position;
                enemy.agent.enabled = true;
            }
        }
    }
}