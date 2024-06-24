using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class TitleScreenManager : MonoBehaviour
    {
        [Header("Events")]
        public UnityEvent onAwake_Event;
        public UnityEvent onPlayerBeginsGame_Event;
        public UnityEvent ifPlayerHasSeenTitleScreen_Event;

        [Header("Game Session")]
        public GameSession gameSession;
        public SaveManager saveManager;
        public StarterAssetsInputs starterAssetsInputs;
        public PlayerAppearance playerAppearance;

        public GameSettings gameSettings;

        private void Awake()
        {
            if (gameSession.gameState == GameSession.GameState.NOT_INITIALIZED)
            {
                saveManager.ResetGameState(false);
                gameSession.gameState = GameSession.GameState.INITIALIZED;
            }

            gameSettings.UpdatePlayerNameOnLocalizedAssets();
        }

        private void Start()
        {
            bool shouldBeginImmediately = false;

            if (gameSession.gameState == GameSession.GameState.BEGINNING_NEW_GAME_PLUS)
            {
                gameSession.gameState = GameSession.GameState.INITIALIZED_AND_SHOWN_TITLE_SCREEN;
                gameSession.currentGameIteration++;

                shouldBeginImmediately = true;
            }
            else if (gameSession.gameState == GameSession.GameState.INITIALIZED_AND_SHOWN_TITLE_SCREEN)
            {
                ifPlayerHasSeenTitleScreen_Event?.Invoke();
                gameObject.SetActive(false);
                return;
            }

            onAwake_Event?.Invoke();


            if (shouldBeginImmediately)
            {
                StartGame();
            }
        }

        public void StartGame()
        {
            gameSession.gameState = GameSession.GameState.INITIALIZED_AND_SHOWN_TITLE_SCREEN;
            onPlayerBeginsGame_Event?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
