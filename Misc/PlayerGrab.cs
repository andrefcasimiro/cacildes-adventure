using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class PlayerGrab : MonoBehaviour
    {
        Transform originalParent;
        PlayerManager _playerManager;

        public UnityEvent onUnparentPlayer;

        public bool isPlayerCaught = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (Physics.autoSyncTransforms == false)
                {
                    originalParent = GetPlayerManager().transform.parent;

                    GetPlayerManager().transform.SetParent(this.transform);
                    Physics.autoSyncTransforms = true;

                    LockPlayerMovement();

                    isPlayerCaught = true;
                }
            }
        }

        public void LockPlayerMovement()
        {
            GetPlayerManager().characterController.enabled = false;
            GetPlayerManager().playerComponentManager.LockPlayerControl();
            GetPlayerManager().transform.localPosition = Vector3.zero;
        }

        public void UnparentPlayer()
        {
            GetComponent<BoxCollider>().enabled = false;

            if (isPlayerCaught == false)
            {
                return;
            }
            onUnparentPlayer?.Invoke();

            Physics.autoSyncTransforms = false;

            GetPlayerManager().transform.parent = null;

            GetPlayerManager().transform.rotation = originalParent.rotation;

            GetPlayerManager().transform.SetParent(originalParent);

            GetPlayerManager().playerComponentManager.EnablePlayerControl();
            isPlayerCaught = false;

        }

        PlayerManager GetPlayerManager()
        {
            if (_playerManager == null)
            {
                _playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include);

            }

            return _playerManager;
        }
    }

}
