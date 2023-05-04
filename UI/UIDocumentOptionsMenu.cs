using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentOptionsMenu : MonoBehaviour
    {

        [Header("Localization")]
        public LocalizedText gameLanguageLabel;
        public LocalizedText englishLabel;
        public LocalizedText portugueseLabel;
        public LocalizedText graphicsQualityLabel;
        public LocalizedText lowLabel;
        public LocalizedText mediumLabel;
        public LocalizedText goodLabel;

        public LocalizedText musicVolumeLabel;
        public LocalizedText ambienceVolumeLabel;
        public LocalizedText sfxVolumeLabel;
        public LocalizedText playParryTimeScaleLabel;
        public LocalizedText systemOptionsLabel;
        public LocalizedText audioOptionsLabel;
        public LocalizedText combatOptionsLabel;

        public UnityAction onLanguageChanged;

        public void Activate(VisualElement root)
        {
            var languageOptions = root.Q<RadioButtonGroup>("LanguageOptions");

            languageOptions.value = GamePreferences.instance.gameLanguage == GamePreferences.GameLanguage.ENGLISH ? 0 : 1;

            languageOptions.RegisterValueChangedCallback(ev =>
            {
                if (ev.newValue == 0)
                {
                    GamePreferences.instance.SetGameLanguage(GamePreferences.GameLanguage.ENGLISH);
                }
                else if (ev.newValue == 1)
                {
                    GamePreferences.instance.SetGameLanguage(GamePreferences.GameLanguage.PORTUGUESE);
                }

                TranslateUI(root);

                FindObjectOfType<UIDocumentPlayerHUDV2>(true).UpdateFavoriteItems();

                if (onLanguageChanged != null)
                {
                    onLanguageChanged.Invoke();
                }
            });

            var graphicsOptions = root.Q<RadioButtonGroup>("GraphicsOptions");

            if (GamePreferences.instance.graphicsQuality == GamePreferences.GraphicsQuality.LOW)
            {
                graphicsOptions.value = 0;
            }
            else if (GamePreferences.instance.graphicsQuality == GamePreferences.GraphicsQuality.MEDIUM)
            {
                graphicsOptions.value = 1;
            }
            else
            {
                graphicsOptions.value = 2;
            }

            graphicsOptions.RegisterValueChangedCallback(ev =>
            {
                if (ev.newValue == 0)
                {
                    GamePreferences.instance.SetGraphicsQuality(GamePreferences.GraphicsQuality.LOW);
                }
                else if (ev.newValue == 1)
                {
                    GamePreferences.instance.SetGraphicsQuality(GamePreferences.GraphicsQuality.MEDIUM);
                }
                else if (ev.newValue == 2)
                {
                    GamePreferences.instance.SetGraphicsQuality(GamePreferences.GraphicsQuality.GOOD);
                }
            });

            var musicVolumeSlider = root.Q<Slider>("MusicVolume");
            musicVolumeSlider.value = PlayerPrefs.HasKey("musicVolume") ? PlayerPrefs.GetFloat("musicVolume") : BGMManager.instance.bgmAudioSource.volume;
            musicVolumeSlider.RegisterValueChangedCallback(ev =>
            {
                PlayerPrefs.SetFloat("musicVolume", ev.newValue);

                GamePreferences.instance.UpdateAudio();
            });

            var ambienceSlider = root.Q<Slider>("AmbienceVolume");
            ambienceSlider.value = PlayerPrefs.HasKey("ambienceVolume") ? PlayerPrefs.GetFloat("ambienceVolume") : BGMManager.instance.ambienceAudioSource.volume;
            ambienceSlider.RegisterValueChangedCallback(ev =>
            {
                PlayerPrefs.SetFloat("ambienceVolume", ev.newValue);

                GamePreferences.instance.UpdateAudio();
            });

            var sfxSlider = root.Q<Slider>("SFXVolume");
            sfxSlider.value = PlayerPrefs.HasKey("sfxVolume") ? PlayerPrefs.GetFloat("sfxVolume") : BGMManager.instance.sfxAudioSource.volume;
            sfxSlider.RegisterValueChangedCallback(ev =>
            {
                PlayerPrefs.SetFloat("sfxVolume", ev.newValue);

                GamePreferences.instance.UpdateAudio();
            });

            var slowDownTimeWhenParrying = root.Q<Toggle>("PlayParryTimescale");
            slowDownTimeWhenParrying.value = PlayerPrefs.HasKey("slowDownTimeWhenParrying") ? PlayerPrefs.GetInt("slowDownTimeWhenParrying") == 1 : false;
            slowDownTimeWhenParrying.RegisterValueChangedCallback(ev =>
            {
                PlayerPrefs.SetInt("slowDownTimeWhenParrying", ev.newValue ? 1 : 0);

            });


            TranslateUI(root);
        }

        void TranslateUI(VisualElement root)
        {
            var languageOptions = root.Q<RadioButtonGroup>("LanguageOptions");
            languageOptions.Q<Label>().text = gameLanguageLabel.GetText();
            languageOptions.choices = new[] { englishLabel.GetText(), portugueseLabel.GetText() };

            var graphicsOptions = root.Q<RadioButtonGroup>("GraphicsOptions");
            graphicsOptions.Q<Label>().text = graphicsQualityLabel.GetText();
            graphicsOptions.choices = new[] { lowLabel.GetText(), mediumLabel.GetText(), goodLabel.GetText() };

            root.Q<Slider>("MusicVolume").label = musicVolumeLabel.GetText();
            root.Q<Slider>("AmbienceVolume").label = ambienceVolumeLabel.GetText();
            root.Q<Slider>("SFXVolume").label = sfxVolumeLabel.GetText();

            root.Q<Toggle>("PlayParryTimescale").label = playParryTimeScaleLabel.GetText();

            root.Q<Label>("SystemOptions").text = systemOptionsLabel.GetText();
            root.Q<Label>("AudioOptions").text = audioOptionsLabel.GetText();
            root.Q<Label>("CombatOptions").text = combatOptionsLabel.GetText();

        }


    }



}
