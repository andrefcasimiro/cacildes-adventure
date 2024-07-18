using AF.Animations;
using UnityEngine;

namespace AF
{
    public class ResetCanTakeFlagOnStateExit : StateMachineBehaviour
    {
        CharacterBaseManager character;
        CharacterAnimationEventListener characterAnimationEventListener;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (character == null)
            {
                animator.TryGetComponent(out character);
            }

            if (character.damageReceiver != null)
            {
                character.damageReceiver.SetCanTakeDamage(true);
            }
        }


        // Useful when enemy is delaying an attack, but gets hit or must exit its attack state abruptly, then we need to restore the animation speed to its default state for the next clip
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (characterAnimationEventListener == null)
            {
                animator.TryGetComponent(out characterAnimationEventListener);
            }

            if (characterAnimationEventListener != null && characterAnimationEventListener.ShouldResetAnimationSpeed())
            {
                characterAnimationEventListener.RestoreDefaultAnimatorSpeed();
            }
        }
    }
}
