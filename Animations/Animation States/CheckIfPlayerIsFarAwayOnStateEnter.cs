using UnityEngine;

namespace AF
{
    public class CheckIfPlayerIsFarAwayOnStateEnter : StateMachineBehaviour
    {
        CharacterManager character;

        public string animatorStateToExitIfTargetIsFarAway = "Idle";

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (character == null)
            {
                animator.TryGetComponent(out character);
            }


            if (character != null && character.targetManager.IsTargetOutOfMeleeRange())
            {
                character.animator.CrossFade(animatorStateToExitIfTargetIsFarAway, 0.25f);
            }
        }
    }
}
