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

        private void Start()
        {
            if (gameSession.hasShownTitleScreen)
            {
                ifPlayerHasSeenTitleScreen_Event?.Invoke();
                gameObject.SetActive(false);
                return;
            }

            onAwake_Event?.Invoke();
        }

        public void StartGame()
        {
            gameSession.hasShownTitleScreen = true;
            onPlayerBeginsGame_Event?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
