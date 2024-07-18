using System.Collections;
using AF.Health;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public abstract class CharacterAbstractBlockController : MonoBehaviour
    {
        [Header("Components")]
        public CharacterBaseManager characterManager;
        public string hashParried = "Parried";

        [Header("Parrying Settings")]
        public UnityEvent onParryEvent;
        public float baseUnarmedParryWindow = .4f;
        public float parryTimer = Mathf.Infinity;
        Coroutine parryTimerCoroutine;

        public UnityEvent onParriedEvent;
        public int basePostureDamageFromParry = 20;

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
        [Range(0, 1f)] public float unarmedDefenseAbsorption = .8f;

        [Header("Unity Events")]
        public UnityEvent onBlockDamageEvent;

        // Flags
        public bool isBlocking = false;

        public UnityAction onBlockChanged;

        public void SetIsBlocking(bool value)
        {
            isBlocking = value;

            onBlockChanged?.Invoke();
        }

        public void BlockAttack(Damage damage)
        {
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
            if (characterManager is CharacterManager)
            {
                parryTimer = 0f;
            }

            if (parryTimerCoroutine != null)
            {
                StopCoroutine(parryTimerCoroutine);
            }

            parryTimerCoroutine = StartCoroutine(HandleParryTimer());
        }

        IEnumerator HandleParryTimer()
        {
            while (parryTimer < GetUnarmedParryWindow())
            {
                parryTimer += Time.deltaTime;
                yield return null;
            }

            parryTimer = Mathf.Infinity;
        }

        public bool IsWithinParryingWindow()
        {
            return parryTimer < GetUnarmedParryWindow();
        }

        public bool CanParry(Damage damage)
        {
            if (damage.canNotBeParried)
            {
                return false;
            }

            return IsWithinParryingWindow();
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

            characterManager.PlayBusyAnimationWithRootMotion(hashParried);

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

        public abstract float GetUnarmedParryWindow();

        public abstract int GetPostureDamageFromParry();
    }
}
