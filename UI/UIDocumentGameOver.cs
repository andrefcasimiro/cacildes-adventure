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

        [Header("Settings")]
        public float gameOverDuration = 3.5f;

        private void Awake()
        {
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            GetComponent<UIDocument>().rootVisualElement.Q<Label>("YouDiedText").text = "You died!";

            bgmManager.StopMusic();
            soundbank.PlaySound(soundbank.gameOverFanfare);

            playerManager.playerComponentManager.DisableCharacterController();
            playerManager.playerComponentManager.DisableComponents();

            LostCoinsManager.instance.SetCoinsToRecover(playerManager.transform);

            StartCoroutine(Reload());
        }

        IEnumerator Reload()
        {
            yield return new WaitForSeconds(gameOverDuration);

            // Reload Game
        }
    }
}
