using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{
    public class ViewComponent_GameSettings : MonoBehaviour
    {

        // Gameplay
        RadioButtonGroup gameDifficulty;

        // Language
        RadioButtonGroup languageOptions;
        public UnityAction onLanguageChanged;

        UIDocumentPlayerHUDV2 uiDocumentPlayerHUDV2;

        // Graphics
        RadioButtonGroup graphicsOptions;

        // Audio
        Slider musicVolumeSlider;

        public void SetupRefs(VisualElement root)
        {

            if (uiDocumentPlayerHUDV2 == null)
            {
                uiDocumentPlayerHUDV2 = FindAnyObjectByType<UIDocumentPlayerHUDV2>(FindObjectsInactive.Include);
            }

            gameDifficulty = root.Q<Foldout>("FoldoutGameplay").Q<RadioButtonGroup>();

            languageOptions = root.Q<Foldout>("FoldoutLanguage").Q<RadioButtonGroup>();

            root.Q("RestartToSeeChanges").style.display = DisplayStyle.None;

            graphicsOptions = root.Q<Foldout>("FoldoutGraphics").Q<RadioButtonGroup>();
            musicVolumeSlider = root.Q<Slider>("MusicVolumeSlider");

            SetupDefaultValues();

            gameDifficulty.RegisterValueChangedCallback(ev =>
            {
                var newValue = ev.newValue;

                if (newValue == 0)
                {
                    GamePreferences.instance.SetGameDifficulty(GamePreferences.GameDifficulty.EASY);
                }
                else if (newValue == 1)
                {
                    GamePreferences.instance.SetGameDifficulty(GamePreferences.GameDifficulty.MEDIUM);
                }
                else if (newValue == 2)
                {
                    GamePreferences.instance.SetGameDifficulty(GamePreferences.GameDifficulty.HARD);
                }
            });

            languageOptions.RegisterValueChangedCallback(ev =>
            {
                var newValue = ev.newValue;

                if (newValue == 0)
                {
                    GamePreferences.instance.SetGameLanguage(GamePreferences.GameLanguage.ENGLISH);
                }
                else if (newValue == 1)
                {
                    GamePreferences.instance.SetGameLanguage(GamePreferences.GameLanguage.PORTUGUESE);
                }

                uiDocumentPlayerHUDV2.UpdateFavoriteItems();
                onLanguageChanged?.Invoke();


                root.Q<Label>("RestartToSeeChanges").text = GamePreferences.instance.IsEnglish() ? "* Reopen the menu to see the changes" : "* Reabre o menu para ver as alterações";
                root.Q("RestartToSeeChanges").style.display = DisplayStyle.Flex;

            });

            graphicsOptions.RegisterValueChangedCallback(ev =>
            {
                var newValue = ev.newValue;

                if (newValue == 0)
                {
                    GamePreferences.instance.SetGraphicsQuality(GamePreferences.GraphicsQuality.LOW);
                }
                else if (newValue == 1)
                {
                    GamePreferences.instance.SetGraphicsQuality(GamePreferences.GraphicsQuality.MEDIUM);
                }
                else if (newValue == 2)
                {
                    GamePreferences.instance.SetGraphicsQuality(GamePreferences.GraphicsQuality.GOOD);
                }
                else if (newValue == 3)
                {
                    GamePreferences.instance.SetGraphicsQuality(GamePreferences.GraphicsQuality.ULTRA);
                }
            });

            musicVolumeSlider.RegisterValueChangedCallback(ev =>
            {
                var newValue = ev.newValue;
                PlayerPrefs.SetFloat("musicVolume", newValue);

                GamePreferences.instance.UpdateAudio();
            });

        }

        public void SetupDefaultValues()
        {

            if (GamePreferences.instance.gameDifficulty == GamePreferences.GameDifficulty.EASY)
            {
                gameDifficulty.value = 0;
            }
            else if (GamePreferences.instance.gameDifficulty == GamePreferences.GameDifficulty.MEDIUM)
            {
                gameDifficulty.value = 1;
            }
            else
            {
                gameDifficulty.value = 2;
            }

            languageOptions.value = GamePreferences.instance.gameLanguage == GamePreferences.GameLanguage.ENGLISH ? 0 : 1;
            if (GamePreferences.instance.graphicsQuality == GamePreferences.GraphicsQuality.LOW)
            {
                graphicsOptions.value = 0;
            }
            else if (GamePreferences.instance.graphicsQuality == GamePreferences.GraphicsQuality.MEDIUM)
            {
                graphicsOptions.value = 1;
            }
            else if (GamePreferences.instance.graphicsQuality == GamePreferences.GraphicsQuality.GOOD)
            {
                graphicsOptions.value = 2;
            }
            else
            {
                graphicsOptions.value = 3;
            }

            musicVolumeSlider.value = PlayerPrefs.HasKey("musicVolume") ? PlayerPrefs.GetFloat("musicVolume") : BGMManager.instance.bgmAudioSource.volume;

        }


        public void TranslateSettingsUI(VisualElement root)
        {
            bool isEnglish = GamePreferences.instance.IsEnglish();

            #region Headers
            root.Q<Foldout>("FoldoutGameplay").text = isEnglish ? "Gameplay" : "Jogabilidade";
            root.Q<Foldout>("FoldoutLanguage").text = isEnglish ? "Language" : "Idioma";
            root.Q<Foldout>("FoldoutGraphics").text = isEnglish ? "Graphics" : "Gráficos";
            root.Q<Foldout>("FoldoutAudio").text = isEnglish ? "Audio" : "Áudio";
            #endregion

            gameDifficulty.Q<Label>().text = isEnglish ? "Set game difficulty" : "Selecionar dificuldade";
            gameDifficulty.choices = new[] {
                isEnglish ? "Easy (Enemies deal less damage, have less health and are slower)" : "Fácil (Inimigos com vida reduzida, ataque reduzido e mais lentos)",
                isEnglish ? "Medium (Enemies deal less damage)" : "Média (Inimigos dão menos dano)",
                isEnglish ? "Hard" : "Difícil",
            };

            languageOptions.Q<Label>().text = isEnglish ? "Select language" : "Selecionar idioma";
            languageOptions.choices = new[] {
                isEnglish ? "English" : "Inglês",
                isEnglish ? "Portuguese" : "Português"
            };
            languageOptions.value = GamePreferences.instance.gameLanguage == GamePreferences.GameLanguage.ENGLISH ? 0 : 1;

            graphicsOptions.Q<Label>().text = isEnglish ? "Select video quality" : "Selecionar qualidade de vídeo";
            graphicsOptions.choices = new[] {
                isEnglish ? "Low" : "Baixa",
                isEnglish ? "Medium" : "Média",
                isEnglish ? "High" : "Alta",
                isEnglish ? "Ultra" : "Ultra",
            };

            musicVolumeSlider.Q<Label>().text = isEnglish ? "Set music volume" : "Mudar volume de música";

            SetupDefaultValues();
        }
    }
}
