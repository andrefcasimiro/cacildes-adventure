using AF.Companions;
using AF.Events;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
        public string bonfireId;
        public string bonfireName;

        [HideInInspector]
        public bool canBeTravelledTo = true;
        public bool canUseTravelToOtherMaps = true;

        [Header("Databases")]
        public BonfiresDatabase bonfiresDatabase;

        [Header("Components")]
        PlayerManager _playerManager;
        CursorManager _cursorManager;
        SaveManager _saveManager;
        UIDocumentBonfireMenu _uiDocumentBonfireMenu;
        CompanionsSceneManager _companionsSceneManager;


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

        PlayerManager GetPlayerManager()
        {
            if (_playerManager == null) { _playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include); }
            return _playerManager;
        }

        SaveManager GetSaveManager()
        {
            if (_saveManager == null) { _saveManager = FindAnyObjectByType<SaveManager>(FindObjectsInactive.Include); }
            return _saveManager;
        }

        CursorManager GetCursorManager()
        {
            if (_cursorManager == null) { _cursorManager = FindAnyObjectByType<CursorManager>(FindObjectsInactive.Include); }
            return _cursorManager;
        }

        UIDocumentBonfireMenu GetUIDocumentBonfireMenu()
        {
            if (_uiDocumentBonfireMenu == null) { _uiDocumentBonfireMenu = FindAnyObjectByType<UIDocumentBonfireMenu>(FindObjectsInactive.Include); }
            return _uiDocumentBonfireMenu;
        }

        CompanionsSceneManager GetCompanionsSceneManager()
        {

            if (_companionsSceneManager == null) { _companionsSceneManager = FindAnyObjectByType<CompanionsSceneManager>(FindObjectsInactive.Include); }
            return _companionsSceneManager;
        }

        void CurePlayer()
        {
            GetPlayerManager().health.RestoreFullHealth();
            GetPlayerManager().staminaStatManager.RestoreStaminaPercentage(100);
            GetPlayerManager().statusController.RemoveAllStatuses();
            GetPlayerManager().manaManager.RestoreManaPercentage(100);
        }

        bool CanUseBonfire()
        {
            if (GetPlayerManager().IsBusy())
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

            bonfiresDatabase.lastBonfireSceneId = SceneManager.GetActiveScene().name;

            onBonfire_Enter?.Invoke();
            GetPlayerManager().ResetStates();
            GetPlayerManager().PlayBusyHashedAnimationWithRootMotion(sittingAtBonfireHash);
            CurePlayer();

            GetPlayerManager().playerInventory.ReplenishItems();

            GetCompanionsSceneManager()?.TeleportCompanionsNearPlayer();

            if (canBeTravelledTo)
            {
                UnlockBonfire(bonfireId);
            }

            SetPlayerLockState(true);

            GetPlayerManager().playerComponentManager.transform.position = playerTransformRef.transform.position;
            var rot = transform.position - GetPlayerManager().transform.position;
            rot.y = 0;
            GetPlayerManager().playerComponentManager.transform.rotation = Quaternion.LookRotation(rot);

            GetUIDocumentBonfireMenu().SetCurrentBonfire(this);
            GetUIDocumentBonfireMenu().SetCurrentBonfire(this);
            GetUIDocumentBonfireMenu().gameObject.SetActive(true);

            EventManager.EmitEvent(EventMessages.ON_LEAVING_BONFIRE);
        }

        public void ExitBonfire()
        {
            GetSaveManager().SaveGameData(null);

            GetUIDocumentBonfireMenu().gameObject.SetActive(false);

            CurePlayer();

            onBonfire_Exit?.Invoke();
            SetPlayerLockState(false);
            GetPlayerManager().PlayBusyHashedAnimationWithRootMotion(exitingBonfireHash);

            GetCursorManager().HideCursor();
        }

        void SetPlayerLockState(bool isLocked)
        {
            if (isLocked)
            {
                GetPlayerManager().playerComponentManager.DisableCharacterController();
                GetPlayerManager().playerComponentManager.DisableComponents();
            }
            else
            {
                GetPlayerManager().playerComponentManager.EnableCharacterController();
                GetPlayerManager().playerComponentManager.EnableComponents();
            }

            GetPlayerManager().thirdPersonController.LockCameraPosition = isLocked;
            GetPlayerManager().thirdPersonController.canRotateCharacter = !isLocked;

            GetPlayerManager().playerComponentManager.isInBonfire = isLocked;
        }

        public string GetBonfireName()
        {
            return bonfireName;
        }

    }
}
