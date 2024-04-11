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
        public bool ignoreAnimatorSpeed = false;
        public float overrideChaseSpeed = -1f;

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
        public UnityEvent onBlood;

        float defaultAnimatorSpeed;

        private void Awake()
        {
            characterManager.animator.speed = animatorSpeed;
            defaultAnimatorSpeed = animatorSpeed;
        }

        private void Start()
        {
            OverrideAnimatorClips();

            if (ignoreAnimatorSpeed)
            {
                characterManager.animator.SetFloat(speedParameter, 0f);
            }
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
            if (ignoreAnimatorSpeed)
            {
                return;
            }

            if (characterManager.isBusy)
            {
                characterManager.animator.SetFloat(speedParameter, 0f);
                return;
            }

            if (overrideChaseSpeed >= 0 && characterManager.agent.speed > 0)
            {
                characterManager.animator.SetFloat(speedParameter, overrideChaseSpeed);
            }
            else
            {
                characterManager.animator.SetFloat(speedParameter, Mathf.Clamp01(characterManager.agent.speed / characterManager.chaseSpeed));
            }

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

            characterManager.FaceTarget();
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

        public void OnBlood()
        {
            onBlood?.Invoke();
        }

        public void RestoreDefaultAnimatorSpeed()
        {
            this.animatorSpeed = defaultAnimatorSpeed;
            characterManager.animator.speed = animatorSpeed;
        }

        public void SetAnimatorSpeed(float speed)
        {
            this.animatorSpeed = speed;
            characterManager.animator.speed = animatorSpeed;
        }

        public void OnShakeCamera()
        {
            throw new System.NotImplementedException();
        }

        public void DropIKHelper()
        {
        }

        public void UseIKHelper()
        {
        }

        public void SetCanTakeDamage_False()
        {
            if (characterManager == null || characterManager.damageReceiver == null)
            {
                return;
            }
            characterManager.damageReceiver.SetCanTakeDamage(false);
        }
    }
}
