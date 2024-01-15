using AF.Companions;
using AF.Music;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{
    public class TeleportManager : MonoBehaviour
    {
        [Header("Game Session")]
        public GameSession gameSession;

        [Header("Databases")]
        public CompanionsDatabase companionsDatabase;

        [Header("Components")]
        public PlayerManager playerManager;
        public FadeManager fadeManager;
        public BGMManager bGMManager;

        public float teleportFadeOutDuration = 1f;

        private void Awake()
        {
            if (gameSession.rememberPlayerPosition && gameSession.lastPlayerPosition != Vector3.zero)
            {
                playerManager.playerComponentManager.UpdatePosition(gameSession.lastPlayerPosition, Quaternion.identity);

                gameSession.lastPlayerPosition = Vector3.zero;
            }
        }

        void Start()
        {
            SpawnPlayerAndCompanions();
        }

        public void Teleport(string sceneName, string spawnGameObjectNameRef)
        {
            gameSession.nextMap_SpawnGameObjectName = spawnGameObjectNameRef;

            bGMManager.StopMusic();

            fadeManager.FadeIn(1f, () =>
            {
                SceneManager.LoadSceneAsync(sceneName);
            });
        }

        void SpawnPlayerAndCompanions()
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

            foreach (CompanionID companionID in FindObjectsByType<CompanionID>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                if (companionsDatabase.IsCompanionAndIsActivelyInParty(companionID.companionId))
                {
                    companionID.SpawnCompanion(spawnGameObject.transform.position);
                }
            }
        }
    }
}
