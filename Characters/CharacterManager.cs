using AF.Combat;
using AF.Events;
using TigerForge;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{
    public class CharacterManager : MonoBehaviour, ICharacter
    {

        [Header("Components")]
        public Animator animator;
        public NavMeshAgent agent;
        public CharacterController characterController;

        [Header("Audio Sources")]
        public AudioSource combatAudioSource;

        [Header("Components")]
        public CharacterCombatController characterCombatController;
        public TargetManager targetManager;

        [Header("Flags")]
        public bool isBusy = false;

        public void ResetStates()
        {
            EventManager.EmitEvent(EventMessages.ON_CHARACTER_RESET_STATE, this);
        }

        public bool IsBusy()
        {
            return isBusy;
        }

        public void PlayAnimationWithCrossFade(string animationName)
        {
            animator.CrossFade(animationName, 0.2f);
        }

    }
}
