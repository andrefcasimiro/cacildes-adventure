using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_Teleport : EventBase
    {
        [Header("Teleport Settings")]
        public string sceneName;
        public string spawnGameObjectName;

        // Scene Refs
        TeleportManager teleportManager;

        public override IEnumerator Dispatch()
        {
            yield return null;
            Teleport();
        }

        public void Teleport()
        {
            GetTeleportManager().Teleport(sceneName, spawnGameObjectName);
        }

        TeleportManager GetTeleportManager()
        {
            if (teleportManager == null)
            {
                teleportManager = FindAnyObjectByType<TeleportManager>(FindObjectsInactive.Include);
            }

            return teleportManager;
        }
    }
}
