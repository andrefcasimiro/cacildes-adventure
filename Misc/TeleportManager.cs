using AF.Companions;
using AF.Music;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace AF
{
    public class TeleportManager : MonoBehaviour
    {
        [Header("Game Session")]
        public GameSession gameSession;

        [Header("Databases")]
        public CompanionsDatabase companionsDatabase;

        public UnityAction onChangingScene;

        [Header("Components")]
        public PlayerManager playerManager;
        public FadeManager fadeManager;
        public BGMManager bGMManager;
        public CompanionsSceneManager companionsSceneManager;

        public float teleportFadeOutDuration = 1f;

        void Start()
        {
            SpawnPlayer();

            companionsSceneManager.SpawnCompanions();
        }

        public void Teleport(string sceneName)
        {
            Teleport(sceneName, "A");
        }

        public void Teleport(string sceneName, string spawnGameObjectNameRef)
        {
            gameSession.nextMap_SpawnGameObjectName = spawnGameObjectNameRef;

            bGMManager.StopMusic();

            onChangingScene?.Invoke();

            fadeManager.FadeIn(1f, () =>
            {
                SceneManager.LoadSceneAsync(sceneName);
            });
        }

        void SpawnPlayer()
        {
            if (gameSession.loadSavedPlayerPositionAndRotation)
            {
                gameSession.loadSavedPlayerPositionAndRotation = false;
                playerManager.playerComponentManager.UpdatePosition(gameSession.savedPlayerPosition, gameSession.savedPlayerRotation);
            }
            else if (!string.IsNullOrEmpty(gameSession.nextMap_SpawnGameObjectName))
            {

                GameObject spawnGameObject = GameObject.Find(gameSession.nextMap_SpawnGameObjectName);
                gameSession.nextMap_SpawnGameObjectName = "";

                if (spawnGameObject != null)
                {
                    playerManager.playerComponentManager.TeleportPlayer(spawnGameObject.transform);
                }
            }
        }
    }
}
