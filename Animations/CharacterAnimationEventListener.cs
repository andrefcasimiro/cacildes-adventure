using UnityEngine;
using UnityEngine.Events;

namespace AF.Animations
{
    public class CharacterAnimationEventListener : MonoBehaviour, IAnimationEventListener
    {

        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Animator Settings")]
        public string speedParameter = "Speed";

        [Header("Unity Events")]
        public UnityEvent onLeftFootstep;
        public UnityEvent onRightFootstep;
        public UnityEvent onLeftWeaponHitboxOpen;
        public UnityEvent onLeftWeaponHitboxClose;
        public UnityEvent onRightWeaponHitboxOpen;
        public UnityEvent onRightWeaponHitboxClose;
        public UnityEvent onLeftFootHitboxOpen;
        public UnityEvent onRightFootHitboxOpen;

        private void OnAnimatorMove()
        {
            characterManager.animator.SetFloat(speedParameter, characterManager.agent.speed);
        }

        public void OnLeftFootstep()
        {
            onLeftFootstep?.Invoke();
        }

        public void OnRightFootstep()
        {

            onRightFootstep?.Invoke();
        }

        public void OpenLeftWeaponHitbox()
        {
            onLeftWeaponHitboxOpen?.Invoke();
        }

        public void CloseLeftWeaponHitbox()
        {
            onLeftWeaponHitboxClose?.Invoke();

        }

        public void OpenRightWeaponHitbox()
        {
            onRightWeaponHitboxOpen?.Invoke();
        }

        public void CloseRightWeaponHitbox()
        {
            onRightWeaponHitboxClose?.Invoke();
        }

        public void OpenLeftFootHitbox()
        {
            onLeftFootHitboxOpen?.Invoke();
        }

        public void CloseLeftFootHitbox()
        {
        }

        public void OpenRightFootHitbox()
        {
            onRightFootHitboxOpen?.Invoke();
        }

        public void CloseRightFootHitbox()
        {
        }


        public void DisableRotation()
        {
        }

        public void EnableRootMotion()
        {
            characterManager.animator.applyRootMotion = true;
        }

        public void DisableRootMotion()
        {
            characterManager.animator.applyRootMotion = false;
        }

        public void OnSpellCast()
        {
            characterManager.characterBaseShooter.CastSpell();
        }

        public void OnFireArrow()
        {
            characterManager.characterBaseShooter.FireArrow();
        }
    }
}
