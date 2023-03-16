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
        }
    }

}
