using AYellowpaper.SerializedCollections;
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
        public float animatorSpeed = 1f;

        [Header("Animation Clip Overrides")]
        public SerializedDictionary<string, AnimationClip> clipOverrides;

        [Header("Unity Events")]
        public UnityEvent onLeftFootstep;
        public UnityEvent onRightFootstep;
        public UnityEvent onLeftWeaponHitboxOpen;
        public UnityEvent onLeftWeaponHitboxClose;
        public UnityEvent onRightWeaponHitboxOpen;
        public UnityEvent onRightWeaponHitboxClose;
        public UnityEvent onLeftFootHitboxOpen;
        public UnityEvent onLeftFootHitboxClose;
        public UnityEvent onRightFootHitboxOpen;
        public UnityEvent onRightFootHitboxClose;
        public UnityEvent onBuff;
        public UnityEvent onCloth;
        public UnityEvent onImpact;
        public UnityEvent onOpenCombo;

        private void Awake()
        {
            characterManager.animator.speed = animatorSpeed;
        }

        private void Start()
        {
            OverrideAnimatorClips();
        }

        void OverrideAnimatorClips()
        {
            foreach (var entry in clipOverrides)
            {
                characterManager.UpdateAnimatorOverrideControllerClips(entry.Key, entry.Value);
            }
        }

        private void OnAnimatorMove()
        {

            if (characterManager.isBusy)
            {
                characterManager.animator.SetFloat(speedParameter, 0f);
                return;
            }

            characterManager.animator.SetFloat(speedParameter, Mathf.Clamp01(characterManager.agent.speed / characterManager.chaseSpeed));
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
            onLeftFootHitboxClose?.Invoke();
        }

        public void OpenRightFootHitbox()
        {
            onRightFootHitboxOpen?.Invoke();
        }

        public void CloseRightFootHitbox()
        {
            onRightFootHitboxOpen?.Invoke();

        }

        public void DisableRotation()
        {
        }

        public void FaceTarget()
        {
            if (characterManager.targetManager.currentTarget == null)
            {
                return;
            }

            Utils.FaceTarget(characterManager.transform, characterManager.targetManager.currentTarget.transform);
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

        public void OnCloth()
        {
            onCloth?.Invoke();
        }

        public void OnImpact()
        {
            onImpact?.Invoke();
        }

        public void OnBuff()
        {
            onBuff?.Invoke();
        }

        public void OpenCombo()
        {
            onOpenCombo?.Invoke();
        }

        public void OnThrow()
        {

        }
    }
}
