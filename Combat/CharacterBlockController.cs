using System.Collections;
using AF.Health;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class CharacterBlockController : MonoBehaviour
    {
        [Header("Components")]
        public CharacterBaseManager characterManager;
        public readonly int hashParried = Animator.StringToHash("Parried");

        [Header("Parrying Settings")]
        public UnityEvent onParryEvent;
        public float unarmedParryWindow = .4f;
        float parryTimer = Mathf.Infinity;
        Coroutine parryTimerCoroutine;

        public UnityEvent onParriedEvent;
        public int postureDamageFromParry = 20;

        [Header("Counter Attack Settings")]
        [Tooltip("The amount that multiplier the current attack power if we attack immediately after a parry")]
        public float counterAttackMultiplier = 1.5f;
        public float maxCounterAttackWindowAfterParry = 0.85f;

        float currentCounterAttackWindow = Mathf.Infinity;

        Coroutine counterAttackWindowCoroutine;

        [Header("Blocking Settings")]
        [Tooltip("The effectivness of the shield. If 1f, the shield will not give any bonus. If higher, the shield is less effective.")]
        public float blockMultiplier = 1.1f;
        public int unarmedStaminaCostPerBlock = 50;
        public float unarmedDefenseAbsorption = 15;

        [Header("Unity Events")]
        public UnityEvent onBlockDamageEvent;

        // Flags
        public bool isBlocking = false;

        public UnityAction onBlockChanged;

        [Header("Settings")]
        public bool shouldFaceTargetWhenBlockingAttack = true;

        public void SetIsBlocking(bool value)
        {
            isBlocking = value;

            onBlockChanged?.Invoke();
        }

        public void BlockAttack(Damage damage)
        {
            if (shouldFaceTargetWhenBlockingAttack)
            {
                (characterManager as CharacterManager)?.FaceTarget();
            }

            characterManager.characterPosture.TakePostureDamage((int)(damage.postureDamage * blockMultiplier));

            onBlockDamageEvent?.Invoke();

            isBlocking = false;
        }

        public bool CanBlockDamage(Damage damage)
        {
            if (damage.ignoreBlocking)
            {
                return false;
            }

            if (!isBlocking)
            {
                return false;
            }

            return (characterManager.characterPosture.currentPostureDamage + (int)(damage.postureDamage * blockMultiplier)) < characterManager.characterPosture.GetMaxPostureDamage();
        }

        public void BeginParrying()
        {
            parryTimer = 0f;

            if (parryTimerCoroutine != null)
            {
                StopCoroutine(parryTimerCoroutine);
            }

            parryTimerCoroutine = StartCoroutine(HandleParryTimer());
        }

        IEnumerator HandleParryTimer()
        {
            while (parryTimer < unarmedParryWindow)
            {
                parryTimer += Time.deltaTime;
                yield return null;
            }

            parryTimer = Mathf.Infinity;
        }

        public bool IsWithinParryingWindow()
        {
            return parryTimer < unarmedParryWindow;
        }

        public bool CanParry(Damage damage)
        {
            return damage.canNotBeParried == false && IsWithinParryingWindow();
        }

        public void HandleParryEvent()
        {
            onParryEvent?.Invoke();

            currentCounterAttackWindow = 0f;
            if (counterAttackWindowCoroutine != null)
            {
                StopCoroutine(counterAttackWindowCoroutine);
            }
            counterAttackWindowCoroutine = StartCoroutine(HandleCounterAttackWindowCoroutine());
        }

        public void HandleParriedEvent(int receivedPostureDamageFromParry)
        {
            onParriedEvent?.Invoke();

            characterManager.PlayBusyHashedAnimationWithRootMotion(hashParried);

            characterManager.characterPosture.TakePostureDamage(
                receivedPostureDamageFromParry
            );
        }

        public bool IsWithinCounterAttackWindow()
        {
            return currentCounterAttackWindow < maxCounterAttackWindowAfterParry;
        }

        IEnumerator HandleCounterAttackWindowCoroutine()
        {
            yield return new WaitForSeconds(maxCounterAttackWindowAfterParry);
            currentCounterAttackWindow = maxCounterAttackWindowAfterParry;
        }
    }
}
