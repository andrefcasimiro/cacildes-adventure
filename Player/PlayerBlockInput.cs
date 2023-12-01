using System.Collections;
using AF.Ladders;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class PlayerBlockInput : MonoBehaviour
    {
        public readonly int hashIsBlocking = Animator.StringToHash("IsBlocking");
        public readonly int hashParryBlockHit = Animator.StringToHash("Parry Block Hit");


        [Header("Components")]
        public PlayerManager playerManager;

        public float unarmedParryWindow = .4f;
        float parryTimer = 0f;

        [Tooltip("Once we stop blocking, what's the cooldown before we can block again")]
        public float maxBlockCooldown = .5f;
        float blockCooldown = Mathf.Infinity;

        public DestroyableParticle parryFx;

        public int unarmedDefenseAbsorption = 20;

        [Tooltip("The amount that multiplier the current attack power if we attack immediately after a parry")]
        public float counterAttackMultiplier = 1.5f;
        public float maxCounterAttackWindowAfterParry = 0.85f;
        float currentCounterAttackWindow = Mathf.Infinity;

        Coroutine parryTimerCoroutine;
        Coroutine blockCooldownCoroutine;
        Coroutine counterAttackWindowCoroutine;

        [Header("Events")]
        public UnityEvent onBlockStart;
        public UnityEvent onBlockEnd;

        // Queued input
        public bool isBlockInput_Performed = false;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;

        public bool IsWithinCounterAttackWindow()
        {
            return currentCounterAttackWindow < maxCounterAttackWindowAfterParry;
        }

        public void BeginBlock()
        {
            if (!CanBlock())
            {
                return;
            }

            playerManager.animator.SetBool(hashIsBlocking, true);
            playerManager.characterBlockController.SetIsBlocking(true);

            onBlockStart?.Invoke();

            ResetTimers();
        }

        public void CancelBlock()
        {
            playerManager.animator.SetBool(hashIsBlocking, false);
            playerManager.characterBlockController.SetIsBlocking(false);

            onBlockEnd?.Invoke();
        }

        public void CheckQueuedInput()
        {
            if (!isBlockInput_Performed)
            {
                return;
            }

            BeginBlock();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnBlockInput_Performed()
        {
            isBlockInput_Performed = true;

            BeginBlock();
        }
        public void OnBlockInput_Cancelled()
        {
            isBlockInput_Performed = false;

            CancelBlock();
        }

        void ResetTimers()
        {
            parryTimer = 0f;
            blockCooldown = 0f;

            if (parryTimerCoroutine != null)
            {
                StopCoroutine(parryTimerCoroutine);
            }
            parryTimerCoroutine = StartCoroutine(HandleParryTimer());

            if (blockCooldownCoroutine != null)
            {
                StopCoroutine(blockCooldownCoroutine);
            }
            parryTimerCoroutine = StartCoroutine(HandleBlockDownCoroutine());
        }

        IEnumerator HandleParryTimer()
        {
            yield return new WaitForSeconds(unarmedParryWindow);
            parryTimer = unarmedParryWindow;
        }

        IEnumerator HandleBlockDownCoroutine()
        {
            yield return new WaitForSeconds(maxBlockCooldown);
            blockCooldown = maxBlockCooldown;
        }

        bool CanBlock()
        {
            if (equipmentDatabase.IsBowEquipped())
            {
                return false;
            }

            if (equipmentDatabase.IsStaffEquipped())
            {
                return false;
            }

            if (playerManager.IsBusy())
            {
                return false;
            }

            if (playerManager.characterPosture.IsStunned())
            {
                return false;
            }

            if (playerManager.playerShootingManager.isAiming)
            {
                return false;
            }

            if (playerManager.starterAssetsInputs.sprint)
            {
                return false;
            }

            if (playerManager.playerCombatController.isCombatting)
            {
                return false;
            }

            if (blockCooldown < maxBlockCooldown)
            {
                return false;
            }

            if (playerManager.climbController.climbState != ClimbState.NONE)
            {
                return false;
            }

            return true;
        }

        public bool IsParrying()
        {
            if (!CanBlock())
            {
                return false;
            }


            return parryTimer < unarmedParryWindow;
        }

        public void InstantiateParryFx()
        {
            //Instantiate(parryFx, tf.transform.position, Quaternion.identity);
            playerManager.PlayBusyHashedAnimationWithRootMotion(hashParryBlockHit);
            currentCounterAttackWindow = 0f;

            if (counterAttackWindowCoroutine != null)
            {
                StopCoroutine(counterAttackWindowCoroutine);
            }
            counterAttackWindowCoroutine = StartCoroutine(HandleCounterAttackWindowCoroutine());
        }

        IEnumerator HandleCounterAttackWindowCoroutine()
        {
            yield return new WaitForSeconds(maxCounterAttackWindowAfterParry);
            currentCounterAttackWindow = maxCounterAttackWindowAfterParry;
        }

    }

}
