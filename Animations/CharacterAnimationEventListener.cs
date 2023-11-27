using UnityEngine;
using UnityEngine.Events;

namespace AF.Animations
{
    public class CharacterAnimationEventListener : MonoBehaviour, IAnimationEventListener
    {
        public CharacterManager characterManager;

        [Header("Unity Events")]
        public UnityEvent onLeftFootstep;
        public UnityEvent onRightFootstep;
        public UnityEvent onLeftWeaponHitboxOpen;
        public UnityEvent onRightWeaponHitboxOpen;
        public UnityEvent onLeftFootHitboxOpen;
        public UnityEvent onRightFootHitboxOpen;

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
        }

        public void OpenRightWeaponHitbox()
        {
            onRightWeaponHitboxOpen?.Invoke();
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

        public void CloseRightWeaponHitbox()
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
