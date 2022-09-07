using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{
    public enum Language
    {
        ENGLISH,
        PORTUGUESE_BR,
        PORTUGUESE_PT,
    }

    public class GameSettingsManager : MonoBehaviour
    {
        public float gameVersion = 0.1f;
        public Language gameLanguage;

        public static GameSettingsManager instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
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
                Enum.TryParse(PlayerPrefs.GetString("language"), out Language language);
                this.gameLanguage = language;
                return;
            }

            if (Application.systemLanguage == SystemLanguage.Portuguese)
            {
                this.gameLanguage = Language.PORTUGUESE_PT;
                return;
            }

            // Default
            this.gameLanguage = Language.ENGLISH;
        }

        public void SetGameLanguage(Language language)
        {
            PlayerPrefs.SetString("language", language.ToString());
            this.gameLanguage = language;
        }

    }

}

