using System;
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
            GOOD
        }

        public static GamePreferences instance;

        public GameLanguage gameLanguage;
        public GraphicsQuality graphicsQuality = GraphicsQuality.GOOD;

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

            UpdateAudio();
        }
        
        void InitializeGraphicsQuality()
        {
            SetGraphicsQuality(GraphicsQuality.GOOD);
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
                SetGameLanguage(GameLanguage.PORTUGUESE);
                return;
            }

            // Default

            SetGameLanguage(GameLanguage.ENGLISH);
        }

/*        private void Update()
        {
            // For debugging purposes
            if (Input.GetKeyDown(KeyCode.Alpha9))
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
        }*/

        public void SetGameLanguage(GameLanguage language)
        {
            PlayerPrefs.SetString("language", language.ToString());
            gameLanguage = language;
        }

        public void SetGraphicsQuality(GraphicsQuality graphicsQuality)
        {
            this.graphicsQuality = graphicsQuality;

            UpdateGraphics();
        }

        public void UpdateGraphics()
        {
            var postProcessVolumes = FindObjectsOfType<PostProcessVolume>(true);
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
                QualitySettings.SetQualityLevel(5);
            }

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

                if (graphicsQuality == GraphicsQuality.GOOD)
                {
                    postProcessVolume.profile.GetSetting<Vignette>().active = true;
                    postProcessVolume.profile.GetSetting<AmbientOcclusion>().active = true;
                    postProcessVolume.profile.GetSetting<MotionBlur>().active = true;
                    postProcessVolume.profile.GetSetting<Bloom>().active = true;
                    postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
                }
            }
        }

        public void UpdateAudio()
        {
            if (PlayerPrefs.HasKey("musicVolume"))
            {
                BGMManager.instance.bgmAudioSource.volume = PlayerPrefs.GetFloat("musicVolume");
            }
            if (PlayerPrefs.HasKey("ambienceVolume"))
            {
                BGMManager.instance.ambienceAudioSource.volume = PlayerPrefs.GetFloat("ambienceVolume");
            }
            if (PlayerPrefs.HasKey("sfxVolume"))
            {
                BGMManager.instance.sfxAudioSource.volume = PlayerPrefs.GetFloat("sfxVolume");
            }
        }

        public bool ShouldSlowDownTimeWhenParrying()
        {
            return PlayerPrefs.HasKey("slowDownTimeWhenParrying") ? PlayerPrefs.GetInt("slowDownTimeWhenParrying") == 1 : false;
        }

    }
}
