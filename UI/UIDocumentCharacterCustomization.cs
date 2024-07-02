using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentCharacterCustomization : MonoBehaviour
    {
        UIDocument document => GetComponent<UIDocument>();

        [Header("Components")]
        public PlayerManager playerManager;
        public PlayerAppearance playerAppearance;

        public Soundbank soundbank;
        public CursorManager cursorManager;
        VisualElement root;

        Button saveChangesButton, resetSettingsButton;

        public UnityEvent onEnable;
        public UnityEvent onDisable;

        readonly List<string> availableColors = new()
        {
    "#000000",   // Black
    "#333333",   // Dark Gray
    "#666666",   // Gray
    "#999999",   // Light Gray
    "#FFFFFF",   // White
    "#FF0000",   // Red
    "#800000",   // Dark Red
    "#CC3333",   // Medium Red
    "#FF6666",   // Light Red
    "#FF8800",   // Orange
    "#FF6600",   // Dark Orange
    "#FFAA33",   // Medium Orange
    "#FFCC66",   // Light Orange
    "#FFFF00",   // Yellow
    "#CCCC00",   // Dark Yellow
    "#FFFF66",   // Medium Yellow
    "#FFFF99",   // Light Yellow
    "#00FF00",   // Green
    "#008000",   // Dark Green
    "#00CC00",   // Medium Green
    "#99FF99",   // Light Green
    "#0000FF",   // Blue
    "#000080",   // Dark Blue
    "#3333FF",   // Medium Blue
    "#99CCFF",   // Light Blue
    "#800080",   // Purple
    "#660066",   // Dark Purple
    "#9933FF",   // Medium Purple
    "#CC99FF",   // Light Purple
    "#FFC0CB",   // Pink
    "#FF69B4",   // Dark Pink
    "#FFB6C1",   // Medium Pink
    "#FFCCCC",   // Light Pink
    "#FFCCAE",   // Cacildes Color
    "#8B4513",   // Brown
    "#654321",   // Dark Brown
    "#A0522D",   // Medium Brown
    "#CD853F",   // Light Brown
    "#4F412D",   // Cacildes Hair
    "#E6E6FA",   // Lavender
    "#FFFACD",   // Lemon Chiffon
    "#D3D3D3",   // Light Grey
    "#F5F5DC",   // Beige
    "#FFD700",   // Gold
    "#D2691E",   // Chocolate
    "#DC143C",   // Crimson
    "#ADFF2F",   // Green Yellow
    "#FF4500",   // Orange Red
    "#DA70D6",   // Orchid
    "#7FFFD4",   // Aquamarine
    "#20B2AA",   // Light Sea Green
    "#87CEEB",   // Sky Blue
    "#778899",   // Light Slate Gray
    "#B0C4DE",   // Light Steel Blue
    "#00FA9A",   // Medium Spring Green
    "#4682B4",   // Steel Blue
    "#D8BFD8",   // Thistle
    "#FF6347",   // Tomato
    "#40E0D0",   // Turquoise
    "#EE82EE",   // Violet
    "#F5DEB3",   // Wheat
    "#9ACD32"    // Yellow Green
        };

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            root = document.rootVisualElement;

            saveChangesButton = root.Q<Button>("SaveButton");
            resetSettingsButton = root.Q<Button>("ResetToDefaultButton");

            SetupUI();

            UIUtils.SetupButton(saveChangesButton, () =>
            {
                this.gameObject.SetActive(false);
            }, soundbank);

            UIUtils.SetupButton(resetSettingsButton, () =>
            {
                playerAppearance.ResetDefaults();
            }, soundbank);


            cursorManager.ShowCursor();

            // Delay the focus until the next frame, required as an hack for now
            Invoke(nameof(GiveFocus), 0f);

            onEnable?.Invoke();

            playerManager.playerComponentManager.DisablePlayerControl();
        }

        private void OnDisable()
        {

            playerManager.playerComponentManager.EnablePlayerControl();

            onDisable?.Invoke();
        }

        void GiveFocus()
        {
            saveChangesButton.Focus();
        }

        /// <summary>
        ///  Unity Event
        /// </summary>
        public void Close()
        {
            if (this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(false);
            }
        }

        void SetupUI()
        {
            SetupNameInput();
            SetupTypeSliders();
            SetupColorSliders();
        }

        void SetupNameInput()
        {
            TextField nameInput = root.Q<TextField>("CharacterName");
            nameInput.value = playerAppearance.GetPlayerName();
            nameInput.RegisterValueChangedCallback(ev =>
            {
                playerAppearance.UpdatePlayerName(ev.newValue);
            });
        }

        void SetupColorSliders()
        {
            SliderInt hairColorSlider = root.Q<SliderInt>("HairColorSlider");
            hairColorSlider.value = availableColors.IndexOf(playerAppearance.GetHairColor());
            hairColorSlider.lowValue = 0;
            hairColorSlider.highValue = availableColors.Count - 1;

            hairColorSlider.RegisterValueChangedCallback(ev =>
            {
                int indx = (int)ev.newValue;

                playerAppearance.UpdateHairColor(availableColors[indx]);
            });

            SliderInt bodyColorSlider = root.Q<SliderInt>("BodyColorSlider");
            bodyColorSlider.value = availableColors.IndexOf(playerAppearance.GetBodyColor());
            bodyColorSlider.lowValue = 0;
            bodyColorSlider.highValue = availableColors.Count - 1;
            bodyColorSlider.RegisterValueChangedCallback(ev =>
            {
                int indx = (int)ev.newValue;

                playerAppearance.UpdateBodyColor(availableColors[indx]);
            });


            SliderInt eyeColorSlider = root.Q<SliderInt>("EyeColorSlider");
            eyeColorSlider.value = availableColors.IndexOf(playerAppearance.GetEyeColor());
            eyeColorSlider.lowValue = 0;
            eyeColorSlider.highValue = availableColors.Count - 1;
            eyeColorSlider.RegisterValueChangedCallback(ev =>
            {
                int indx = (int)ev.newValue;

                playerAppearance.UpdateEyeColor(availableColors[indx]);
            });

            SliderInt tattooColorSlider = root.Q<SliderInt>("TattooColorSlider");
            tattooColorSlider.value = availableColors.IndexOf(playerAppearance.GetTattooColor());
            tattooColorSlider.lowValue = 0;
            tattooColorSlider.highValue = availableColors.Count - 1;
            tattooColorSlider.RegisterValueChangedCallback(ev =>
            {
                int indx = (int)ev.newValue;

                playerAppearance.UpdateTattooColor(availableColors[indx]);
            });

            VisualElement portraitPreview = root.Q<VisualElement>("PortraitPreview");
            portraitPreview.style.backgroundImage = new StyleBackground(playerAppearance.GetPlayerPortrait());

            SliderInt portrairSlider = root.Q<SliderInt>("PortraitSlider");
            portrairSlider.value = playerAppearance.GetPlayerPortraitIndex();
            portrairSlider.lowValue = 0;
            portrairSlider.highValue = playerAppearance.portraits.Length - 1;
            portrairSlider.RegisterValueChangedCallback(ev =>
            {
                int indx = (int)ev.newValue;

                playerAppearance.SetPlayerPortrait(indx);
                portraitPreview.style.backgroundImage = new StyleBackground(playerAppearance.GetPlayerPortrait());
            });
        }

        void SetupTypeSliders()
        {
            SliderInt bodyTypeSlider = root.Q<SliderInt>("BodyTypeSlider");
            bodyTypeSlider.value = playerAppearance.GetBodyType();
            bodyTypeSlider.lowValue = 0;
            bodyTypeSlider.highValue = 1;
            bodyTypeSlider.RegisterValueChangedCallback(ev =>
            {
                playerAppearance.SetBodyType((int)ev.newValue);
            });

            SliderInt faceTypeSlider = root.Q<SliderInt>("FaceTypeSlider");
            faceTypeSlider.value = playerAppearance.GetFaceType();
            faceTypeSlider.lowValue = 0;
            faceTypeSlider.highValue = playerAppearance.headContainer.transform.childCount;
            faceTypeSlider.RegisterValueChangedCallback(ev =>
            {
                playerAppearance.SetFaceType((int)ev.newValue);
            });

            SliderInt hairTypeSlider = root.Q<SliderInt>("HairTypeSlider");
            hairTypeSlider.value = playerAppearance.GetHairType();
            hairTypeSlider.lowValue = 0;
            hairTypeSlider.highValue = playerAppearance.hairContainer.transform.childCount;
            hairTypeSlider.RegisterValueChangedCallback(ev =>
            {
                playerAppearance.SetHairType((int)ev.newValue);
            });

            SliderInt eyebrowTypeSlider = root.Q<SliderInt>("EyebrowTypeSlider");
            eyebrowTypeSlider.value = playerAppearance.GetEyebrowType();
            eyebrowTypeSlider.lowValue = 0;
            eyebrowTypeSlider.highValue = playerAppearance.eyebrowContainer.transform.childCount;
            eyebrowTypeSlider.RegisterValueChangedCallback(ev =>
            {
                playerAppearance.SetEyebrowType((int)ev.newValue);
            });

            SliderInt beardTypeSlider = root.Q<SliderInt>("BeardTypeSlider");
            beardTypeSlider.value = playerAppearance.GetEyebrowType();
            beardTypeSlider.lowValue = 0;
            beardTypeSlider.highValue = playerAppearance.beardContainer.transform.childCount;
            beardTypeSlider.RegisterValueChangedCallback(ev =>
            {
                playerAppearance.SetBeardType((int)ev.newValue);
            });
        }
    }
}
