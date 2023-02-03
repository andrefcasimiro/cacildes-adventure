using System;
using UnityEngine;

namespace AF
{
    public class GamePreferences : MonoBehaviour
    {
        public enum GameLanguage
        {
            PORTUGUESE,
            ENGLISH,
        }

        public static GamePreferences instance;

        public GameLanguage gameLanguage;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
            }
        }

        private void Start()
        {
            if (PlayerPrefs.HasKey("language"))
            {
                Enum.TryParse(PlayerPrefs.GetString("language"), out GameLanguage language);
                gameLanguage = language;
                return;
            }

            if (Application.systemLanguage == SystemLanguage.Portuguese)
            {
                gameLanguage = GameLanguage.PORTUGUESE;
                return;
            }

            // Default
            gameLanguage = GameLanguage.ENGLISH;
        }

        private void Update()
        {
            // For debugging purposes
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (gameLanguage == GameLanguage.ENGLISH)
                {
                    SetGameLanguage(GameLanguage.PORTUGUESE);
                }
                else
                {
                    SetGameLanguage(GameLanguage.ENGLISH);
                }
            }
        }

        public void SetGameLanguage(GameLanguage language)
        {
            PlayerPrefs.SetString("language", language.ToString());
            gameLanguage = language;
        }

    }

}
