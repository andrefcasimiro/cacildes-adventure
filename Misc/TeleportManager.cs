using System.Collections;
using AF.Bonfires;
using AF.Companions;
using AF.Loading;
using AF.Music;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

namespace AF
{
    public class TeleportManager : MonoBehaviour
    {
        [Header("Game Session")]
        public GameSession gameSession;

        [Header("Databases")]
        public BonfiresDatabase bonfiresDatabase;
        public UnityAction onChangingScene;

        [Header("Components")]
        public PlayerManager playerManager;
        public FadeManager fadeManager;
        public BGMManager bGMManager;
        public CompanionsSceneManager companionsSceneManager;
        public NotificationManager notificationManager;

        void Start()
        {
            SpawnPlayer();

            companionsSceneManager.SpawnCompanions();

            LoadingManager.Instance.EndLoading();
        }

        public void TeleportToLastRestedBonfire()
        {
            if (string.IsNullOrEmpty(bonfiresDatabase.lastBonfireSceneId))
            {
                notificationManager.ShowNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "No bonfire to travel to. Rest at one first."),
                 null);
                return;
            }

            Teleport(bonfiresDatabase.lastBonfireSceneId, "Bonfire Spawnref");
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
                LoadingManager.Instance.BeginLoading(sceneName);
                //                SceneManager.LoadScene(sceneName);
                //StartCoroutine(LoadSceneAsync(sceneName));
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

                    if (spawnGameObject.transform.childCount > 0)
                    {
                        var targetRot = spawnGameObject.transform.GetChild(0).transform.position - spawnGameObject.transform.position;
                        targetRot.y = 0;
                        playerManager.transform.rotation = Quaternion.LookRotation(targetRot);
                    }
                }
            }
        }
    }
}
