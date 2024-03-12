using System.Collections;
using AF.Events;
using AF.Music;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{
    public class ViewComponent_GameSettings : MonoBehaviour
    {
        [Header("Components")]
        public BGMManager bgmManager;

        public UnityAction onLanguageChanged;

        UIDocumentPlayerHUDV2 uiDocumentPlayerHUDV2;
        public readonly string graphicsQualityLabel = "GraphicsQuality";
        public readonly string cameraSensitivityLabel = "CameraSensitivity";

        [Header("Databases")]
        public GameSession gameSession;

        public void SetupRefs(VisualElement root)
        {

            if (uiDocumentPlayerHUDV2 == null)
            {
                uiDocumentPlayerHUDV2 = FindAnyObjectByType<UIDocumentPlayerHUDV2>(FindObjectsInactive.Include);
            }

            RadioButtonGroup graphicsOptions = root.Q<RadioButtonGroup>(graphicsQualityLabel);
            Slider cameraSensitivity = root.Q<Slider>(cameraSensitivityLabel);

            int graphicsValue = 0;
            if (gameSession.graphicsQuality == GameSession.GraphicsQuality.MEDIUM) graphicsValue = 1;
            else if (gameSession.graphicsQuality == GameSession.GraphicsQuality.GOOD) graphicsValue = 2;
            else if (gameSession.graphicsQuality == GameSession.GraphicsQuality.ULTRA) graphicsValue = 3;
            graphicsOptions.value = graphicsValue;
            graphicsOptions.Focus();

            graphicsOptions.RegisterValueChangedCallback(ev =>
            {
                gameSession.SetGameQuality(ev.newValue);
            });

            cameraSensitivity.RegisterValueChangedCallback(ev =>
            {
                gameSession.mouseSensitivity = ev.newValue;
            });

            cameraSensitivity.lowValue = gameSession.minimumMouseSensitivity;
            cameraSensitivity.highValue = gameSession.maximumMouseSensitivity;
            cameraSensitivity.value = gameSession.mouseSensitivity;
            cameraSensitivity.label = $"Camera Sensitivity ({gameSession.mouseSensitivity})";
        }

    }
}
