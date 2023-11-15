using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;

namespace AF
{
    public class GamePreferences : MonoBehaviour
    {
        public enum GameLanguage
        {
            PORTUGUESE,
            ENGLISH,
        }

        public enum GraphicsQuality
        {
            LOW,
            MEDIUM,
            GOOD,
            ULTRA
        }

        public enum GameDifficulty
        {
            EASY,
            MEDIUM,
            HARD
        }

        public static GamePreferences instance;

        public GameLanguage gameLanguage;
        public GraphicsQuality graphicsQuality = GraphicsQuality.GOOD;
        public GameDifficulty gameDifficulty = GameDifficulty.HARD;


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


            InitializeGameLanguage();
        }

        private void Start()
        {
            InitializeGraphicsQuality();

            InitializeGameDifficulty();

            UpdateAudio();
        }


        void InitializeGraphicsQuality()
        {
            if (PlayerPrefs.HasKey("graphicsQuality"))
            {
                var graphicsQuality = PlayerPrefs.GetInt("graphicsQuality");

                if (graphicsQuality == 0)
                {
                    SetGraphicsQuality(GraphicsQuality.LOW);
                }
                else if (graphicsQuality == 1)
                {
                    SetGraphicsQuality(GraphicsQuality.MEDIUM);
                }
                else if (graphicsQuality == 2)
                {
                    SetGraphicsQuality(GraphicsQuality.GOOD);
                }
                else if (graphicsQuality == 3)
                {
                    SetGraphicsQuality(GraphicsQuality.GOOD);
                }
                return;
            }

            // Default to good quality
            SetGraphicsQuality(GraphicsQuality.GOOD);
        }

        void InitializeGameDifficulty()
        {
            if (PlayerPrefs.HasKey("gameDifficulty"))
            {
                var gameDifficulty = PlayerPrefs.GetInt("gameDifficulty");

                if (gameDifficulty == 0)
                {
                    SetGameDifficulty(GameDifficulty.EASY);
                }
                else if (gameDifficulty == 1)
                {
                    SetGameDifficulty(GameDifficulty.MEDIUM);
                }
                else if (gameDifficulty == 2)
                {
                    SetGameDifficulty(GameDifficulty.HARD);
                }
                return;
            }

            // Default to hard difficulty
            SetGameDifficulty(GameDifficulty.HARD);
        }
        void InitializeGameLanguage()
        {
            if (PlayerPrefs.HasKey("language"))
            {
                Enum.TryParse(PlayerPrefs.GetString("language"), out GameLanguage playerPrefsLanguage);

                SetGameLanguage(playerPrefsLanguage);
                return;
            }

            if (Application.systemLanguage == SystemLanguage.Portuguese)
            {
                SetGameLanguage(GameLanguage.ENGLISH);
                return;
            }

            // Default

            SetGameLanguage(GameLanguage.ENGLISH);
        }

        public void SetGameLanguage(GameLanguage language)
        {
            PlayerPrefs.SetString("language", language.ToString());
            PlayerPrefs.Save();
            gameLanguage = language;
        }

        public void SetGraphicsQuality(GraphicsQuality graphicsQuality)
        {
            this.graphicsQuality = graphicsQuality;

            var value = 0;
            if (graphicsQuality == GraphicsQuality.ULTRA) value = 3;
            if (graphicsQuality == GraphicsQuality.GOOD) value = 2;
            if (graphicsQuality == GraphicsQuality.MEDIUM) value = 1;

            PlayerPrefs.SetInt("graphicsQuality", value);
            PlayerPrefs.Save();

            UpdateGraphics();
        }

        public void SetGameDifficulty(GameDifficulty gameDifficulty)
        {
            this.gameDifficulty = gameDifficulty;

            var value = 0;
            if (gameDifficulty == GameDifficulty.HARD) value = 2;
            if (gameDifficulty == GameDifficulty.MEDIUM) value = 1;

            PlayerPrefs.SetInt("gameDifficulty", value);
            PlayerPrefs.Save();

            UpdateDifficulty();
        }

        public void UpdateGraphics()
        {
            Camera.main.TryGetComponent<PostProcessLayer>(out var postProcessLayer);

            if (graphicsQuality == GraphicsQuality.LOW)
            {
                QualitySettings.SetQualityLevel(0);
            }
            if (graphicsQuality == GraphicsQuality.MEDIUM)
            {
                QualitySettings.SetQualityLevel(2);
            }
            if (graphicsQuality == GraphicsQuality.GOOD)
            {
                QualitySettings.SetQualityLevel(4);
            }
            if (graphicsQuality == GraphicsQuality.ULTRA)
            {
                QualitySettings.SetQualityLevel(5);
            }

            var postProcessVolumes = FindObjectsByType<PostProcessVolume>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            foreach (var postProcessVolume in postProcessVolumes)
            {
                postProcessVolume.profile.GetSetting<Vignette>().active = false;
                postProcessVolume.profile.GetSetting<Bloom>().active = false;
                postProcessVolume.profile.GetSetting<AmbientOcclusion>().active = false;
                postProcessVolume.profile.GetSetting<MotionBlur>().active = false;
                postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;

                if (graphicsQuality == GraphicsQuality.MEDIUM)
                {
                    postProcessVolume.profile.GetSetting<Vignette>().active = true;
                    postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
                }

                if (graphicsQuality == GraphicsQuality.GOOD || graphicsQuality == GraphicsQuality.ULTRA)
                {
                    postProcessVolume.profile.GetSetting<Vignette>().active = true;
                    postProcessVolume.profile.GetSetting<AmbientOcclusion>().active = true;
                    postProcessVolume.profile.GetSetting<MotionBlur>().active = true;
                    postProcessVolume.profile.GetSetting<Bloom>().active = true;
                }

                if (graphicsQuality == GraphicsQuality.GOOD)
                {
                    postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
                }
                else if (graphicsQuality == GraphicsQuality.ULTRA)
                {
                    postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
                }


                var ultraGraphicsParentInScene = FindAnyObjectByType<UltraGraphics>(FindObjectsInactive.Include);
                if (ultraGraphicsParentInScene != null)
                {
                    ultraGraphicsParentInScene.gameObject.SetActive(graphicsQuality == GraphicsQuality.ULTRA);
                }


                var highGraphicsParentInScene = FindAnyObjectByType<HighGraphics>(FindObjectsInactive.Include);
                if (highGraphicsParentInScene != null)
                {
                    highGraphicsParentInScene.gameObject.SetActive(graphicsQuality == GraphicsQuality.ULTRA || graphicsQuality == GraphicsQuality.GOOD);
                }

            }
        }

        void UpdateDifficulty()
        {
            foreach (var characterManager in FindObjectsByType<CharacterManager>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
            }
        }

        public void UpdateAudio()
        {
            if (PlayerPrefs.HasKey("musicVolume"))
            {
                BGMManager.instance.bgmAudioSource.volume = PlayerPrefs.GetFloat("musicVolume");
            }
            else
            {
                BGMManager.instance.bgmAudioSource.volume = 1f;
            }

            if (PlayerPrefs.HasKey("ambienceVolume"))
            {
                BGMManager.instance.ambienceAudioSource.volume = PlayerPrefs.GetFloat("ambienceVolume");
            }
            else
            {
                BGMManager.instance.ambienceAudioSource.volume = 1f;
            }

            if (PlayerPrefs.HasKey("sfxVolume"))
            {
                BGMManager.instance.sfxAudioSource.volume = PlayerPrefs.GetFloat("sfxVolume");
            }
            else
            {
                BGMManager.instance.sfxAudioSource.volume = 1f;
            }

        }

        public float GetCurrentMusicVolume()
        {
            if (PlayerPrefs.HasKey("musicVolume"))
            {
                return PlayerPrefs.GetFloat("musicVolume");
            }

            return 1;
        }

        public bool ShouldSlowDownTimeWhenParrying()
        {
            return PlayerPrefs.HasKey("slowDownTimeWhenParrying") ? PlayerPrefs.GetInt("slowDownTimeWhenParrying") == 1 : false;
        }


        public bool IsEnglish()
        {
            return GamePreferences.instance.gameLanguage == GamePreferences.GameLanguage.ENGLISH;
        }

    }
}
