using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_Teleport : EventBase
    {
        [Header("Teleport Settings")]
        public string sceneName;
        public string spawnGameObjectName;

        [Header("Components")]
        public TeleportManager teleportManager;

        public override IEnumerator Dispatch()
        {
            yield return null;
            Teleport();
        }

        public void Teleport()
        {
            teleportManager.Teleport(sceneName, spawnGameObjectName);
        }
    }
}
