using System.Collections;
using AF.Ladders;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class PlayerBlockInput : MonoBehaviour
    {
        public readonly int hashIsBlocking = Animator.StringToHash("IsBlocking");

        [Header("Components")]
        public PlayerManager playerManager;
        public NotificationManager notificationManager;

        [Tooltip("Once we stop blocking, what's the cooldown before we can block again")]
        public float maxBlockCooldown = .5f;
        float blockCooldown = Mathf.Infinity;
        public int unarmedDefenseAbsorption = 20;

        Coroutine blockCooldownCoroutine;

        [Header("Events")]
        public UnityEvent onBlockStart;
        public UnityEvent onBlockEnd;

        // Queued input
        public bool isBlockInput_Performed = false;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;

        private void Awake()
        {
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

            if (CanBlock())
            {
                playerManager.characterBlockController.parryTimer = 0f;
            }

            BeginBlock();
        }

        public void OnBlockInput_Cancelled()
        {
            isBlockInput_Performed = false;

            CancelBlock();
        }

        void ResetTimers()
        {
            blockCooldown = 0f;

            if (blockCooldownCoroutine != null)
            {
                StopCoroutine(blockCooldownCoroutine);
            }

            blockCooldownCoroutine = StartCoroutine(HandleBlockDownCoroutine());
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

            if (playerManager.characterPosture.isStunned)
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
    }
}
