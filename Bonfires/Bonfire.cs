using AF.Events;
using AF.Health;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Bonfires
{
    public class Bonfire : MonoBehaviour
    {
        // Animations
        public readonly int sittingAtBonfireHash = Animator.StringToHash("Sitting At Bonfire");
        public readonly int exitingBonfireHash = Animator.StringToHash("Exit Bonfire");

        [Header("Events")]
        public UnityEvent onBonfire_Enter;
        public UnityEvent onBonfire_Exit;

        [Header("UI")]
        public UIDocumentBonfireMenu uiDocumentBonfireMenu;
        public string bonfireName;

        [HideInInspector]
        public bool canBeTravelledTo = true;

        [Header("Databases")]
        public BonfiresDatabase bonfiresDatabase;
        public SaveManager saveManager;

        [Header("Components")]
        public PlayerManager playerManager;
        public CursorManager cursorManager;

        [Header("References")]
        public Transform playerTransformRef;


        public void UnlockBonfire(string bonfireName)
        {
            if (bonfiresDatabase.unlockedBonfires.Contains(bonfireName))
            {
                return;
            }

            bonfiresDatabase.unlockedBonfires.Add(bonfireName);
        }

        void CurePlayer()
        {
            playerManager.health.RestoreFullHealth();
            playerManager.staminaStatManager.RestoreStaminaPercentage(100);
            playerManager.statusController.RemoveAllStatuses();
            playerManager.manaManager.RestoreManaPercentage(100);
        }

        bool CanUseBonfire()
        {
            if (playerManager.IsBusy())
            {
                return false;
            }

            return true;
        }

        public void ActivateBonfire()
        {
            if (!CanUseBonfire())
            {
                return;
            }

            onBonfire_Enter?.Invoke();
            playerManager.ResetStates();
            playerManager.PlayBusyHashedAnimationWithRootMotion(sittingAtBonfireHash);
            CurePlayer();

            playerManager.playerInventory.ReplenishItems();

            if (canBeTravelledTo)
            {
                UnlockBonfire(bonfireName);
            }

            // Find all active enemies in scene
            var allEnemiesInScene = FindObjectsByType<CharacterManager>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            foreach (var enemy in allEnemiesInScene)
            {
                if (enemy.GetComponent<CharacterBossController>() != null)
                {
                    continue;
                }

                enemy.enabled = true;
                enemy.ResetStates();

                CharacterHealth characterHealth = enemy.health as CharacterHealth;
                if (characterHealth != null)
                {
                    characterHealth.Revive();
                }
            }

            SetPlayerLockState(true);

            playerManager.playerComponentManager.transform.position = playerTransformRef.transform.position;
            var rot = transform.position - playerManager.transform.position;
            rot.y = 0;
            playerManager.playerComponentManager.transform.rotation = Quaternion.LookRotation(rot);

            uiDocumentBonfireMenu.SetCurrentBonfire(this);
            uiDocumentBonfireMenu.gameObject.SetActive(true);

            EventManager.EmitEvent(EventMessages.ON_LEAVING_BONFIRE);
        }

        public void ExitBonfire()
        {
            saveManager.SaveGameData();

            uiDocumentBonfireMenu.gameObject.SetActive(false);

            CurePlayer();

            onBonfire_Exit?.Invoke();
            SetPlayerLockState(false);
            playerManager.PlayBusyHashedAnimationWithRootMotion(exitingBonfireHash);

            cursorManager.HideCursor();
        }

        void SetPlayerLockState(bool isLocked)
        {
            if (isLocked)
            {
                playerManager.playerComponentManager.DisableCharacterController();
                playerManager.playerComponentManager.DisableComponents();
            }
            else
            {
                playerManager.playerComponentManager.EnableCharacterController();
                playerManager.playerComponentManager.EnableComponents();
            }

            playerManager.thirdPersonController.LockCameraPosition = isLocked;
            playerManager.thirdPersonController.canRotateCharacter = !isLocked;

            playerManager.playerComponentManager.isInBonfire = isLocked;
        }

    }
}
