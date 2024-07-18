namespace AF
{
    using System.Collections;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.AI;
    using UnityEngine.Events;

    public class Portal : MonoBehaviour
    {
        int index = 0;

        public Transform teleportRef;

        public UnityEvent onTeleportEvent;

        bool isActive = false;

        private void Start()
        {
            Portal[] currentPortals = FindObjectsByType<Portal>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            if (currentPortals.Length > 0)
            {
                index = currentPortals.OrderBy(p => p.index).Last().index + 1;
            }

            DisableTemporarily();
        }

        void DisableTemporarily()
        {
            isActive = false;

            Invoke(nameof(AllowActive), 1f);
        }

        void AllowActive()
        {
            isActive = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isActive)
            {
                return;
            }

            if (other.CompareTag("Player") || other.CompareTag("Enemy"))
            {
                TeleportEntityToNextPortal(other.gameObject);
            }
        }

        public void TeleportEntityToNextPortal(GameObject entityToTeleport)
        {
            onTeleportEvent?.Invoke();

            Portal[] orderedPortals = FindObjectsByType<Portal>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .OrderBy(p => p.index).ToArray();

            Portal nextPortal = orderedPortals.FirstOrDefault(x => x.index == index + 1);

            if (nextPortal == null || nextPortal.index == index)
            {
                nextPortal = orderedPortals.First();
            }

            nextPortal.DisableTemporarily();

            Transform desiredTransformDestination = nextPortal == null ? teleportRef : nextPortal.teleportRef;

            if (entityToTeleport.TryGetComponent<PlayerManager>(out var playerManager))
            {
                playerManager.playerComponentManager.TeleportPlayer(desiredTransformDestination);
            }
            else if (entityToTeleport.TryGetComponent<CharacterManager>(out var characterManager))
            {
                entityToTeleport.transform.position = desiredTransformDestination.position;
            }
        }
    }
}
