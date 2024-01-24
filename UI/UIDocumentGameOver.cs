using System.Collections;
using AF.Music;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentGameOver : MonoBehaviour
    {
        [Header("Components")]
        public BGMManager bgmManager;
        public Soundbank soundbank;
        public PlayerManager playerManager;
        public SaveManager saveManager;
        public UIDocumentPlayerGold uIDocumentPlayerGold;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;

        [Header("Settings")]
        public float gameOverDuration = 2.5f;

        private void Awake()
        {
            this.gameObject.SetActive(false);
        }

        public void DisplayGameOver()
        {
            this.gameObject.SetActive(true);
            StartCoroutine(GameOver_Coroutine());
        }

        IEnumerator GameOver_Coroutine()
        {
            GetComponent<UIDocument>().rootVisualElement.Q<Label>("YouDiedText").text = "You Died!";

            bgmManager.StopMusic();
            soundbank.PlaySound(soundbank.gameOverFanfare);

            playerManager.playerComponentManager.DisableCharacterController();
            playerManager.playerComponentManager.DisableComponents();

            if (playerStatsDatabase.HasLostGoldToRecover())
            {
                playerStatsDatabase.ClearLostGold();
            }
            else
            {
                playerStatsDatabase.SetLostGold(playerManager.transform.position);
            }

            uIDocumentPlayerGold.LoseGold(playerStatsDatabase.gold);

            yield return new WaitForSeconds(gameOverDuration);

            saveManager.LoadLastSavedGame(true);
        }
    }
}
