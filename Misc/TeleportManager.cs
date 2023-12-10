using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{
    public class TeleportManager : MonoBehaviour
    {
        [Header("Game Session")]
        public GameSession gameSession;

        [Header("Components")]
        public PlayerManager playerManager;
        public FadeManager fadeManager;
        public float teleportFadeOutDuration = 1f;

        void Start()
        {
            SpawnPlayer();
        }

        public void Teleport(string sceneName, string spawnGameObjectNameRef)
        {
            gameSession.nextMap_SpawnGameObjectName = spawnGameObjectNameRef;

            fadeManager.FadeOut(1f, () =>
            {
                SceneManager.LoadScene(sceneName);
            });
        }

        void SpawnPlayer()
        {
            if (string.IsNullOrEmpty(gameSession.nextMap_SpawnGameObjectName))
            {
                return;
            }

            GameObject spawnGameObject = GameObject.Find(gameSession.nextMap_SpawnGameObjectName);
            gameSession.nextMap_SpawnGameObjectName = "";

            if (spawnGameObject == null)
            {
                return;
            }

            playerManager.playerComponentManager.TeleportPlayer(spawnGameObject.transform);
        }
    }
}
