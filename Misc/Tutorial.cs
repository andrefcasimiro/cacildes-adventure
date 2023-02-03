using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.Video;

namespace AF
{
    public class Tutorial : MonoBehaviour
    {
        [System.Serializable]
        public class TutorialEntry
        {
            public VideoPlayer videoPlayer;

            public Sprite[] keyboardIcons;
            public Sprite[] gamepadIcons;

            [TextArea]
            public string keyboardDescription;

            [TextArea] 
            public string gamepadDescription;
        }

        public string tutorialTitle;

        public TutorialEntry[] tutorialEntries;

        UIDocument uiDocument => GetComponent<UIDocument>();
        public VisualTreeAsset inputSprite; 

        int currentIndex = 0;

        private void Awake()
        {
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            Utils.ShowCursor();

            currentIndex = 0;

            var root = this.uiDocument.rootVisualElement;

            var previousButton = root.Q<Button>("PreviousButton");
            var nextButton = root.Q<Button>("NextButton");

            previousButton.style.opacity = 0.5f;
            nextButton.style.opacity = 0.5f;

            if (currentIndex != 0)
            {
                previousButton.style.opacity = 1;
            }

            if (currentIndex < tutorialEntries.Length - 1)
            {
                nextButton.style.opacity = 1f;
            }

            previousButton.RegisterCallback<ClickEvent>(ev =>
            {
                tutorialEntries[currentIndex].videoPlayer.gameObject.SetActive(false);

                currentIndex = currentIndex - 1;
                if (currentIndex <= 0) currentIndex = 0;
                DrawUI(root);
            });

            nextButton.RegisterCallback<ClickEvent>(ev =>
            {
                tutorialEntries[currentIndex].videoPlayer.gameObject.SetActive(false);

                currentIndex = currentIndex + 1;
                if (currentIndex >= tutorialEntries.Length - 1) currentIndex = tutorialEntries.Length - 1;
                DrawUI(root);
            });

            root.Q<Button>("ContinueButton").RegisterCallback<ClickEvent>(ev =>
            {
                FindObjectOfType<GamepadCursor>(true).gameObject.SetActive(false);
                Utils.HideCursor();
                this.gameObject.SetActive(false);
            });


            DrawUI(root);

            FindObjectOfType<GamepadCursor>(true).gameObject.SetActive(true);
        }

        void DrawUI(VisualElement root)
        {
            var previousButton = root.Q<Button>("PreviousButton");
            var nextButton = root.Q<Button>("NextButton");

            previousButton.style.opacity = 0.1f;
            nextButton.style.opacity = 0.1f;

            if (tutorialEntries.Length <= 1)
            {
                previousButton.style.display = DisplayStyle.None;
                nextButton.style.display = DisplayStyle.None;
            }

            previousButton.SetEnabled(false);
            nextButton.SetEnabled(false);

            if (currentIndex != 0)
            {
                previousButton.SetEnabled(true);
                previousButton.style.opacity = 1;
            }

            if (currentIndex < tutorialEntries.Length - 1)
            {
                nextButton.SetEnabled(true);

                nextButton.style.opacity = 1f;
            }

            var currentPage = this.tutorialEntries[currentIndex];

            root.Q<Label>("Title").text = tutorialTitle;

            var videoContainer = root.Q<IMGUIContainer>("VideoContainer");

            videoContainer.style.display = DisplayStyle.Flex;

            if (currentPage.videoPlayer == null)
            {
                videoContainer.style.display = DisplayStyle.None;
            }

            var keysToShowContainer = root.Q<VisualElement>("KeysToShow");
            keysToShowContainer.Clear();

            if (Gamepad.current == null)
            {
                foreach (var input in currentPage.keyboardIcons)
                {
                    var asset = inputSprite.CloneTree();
                    asset.Q<IMGUIContainer>("Key").style.backgroundImage = new StyleBackground(input);

                    asset.style.marginRight = 5;


                    keysToShowContainer.Add(asset);
                }

                root.Q<Label>("Description").text = currentPage.keyboardDescription;
            }
            else
            {
                foreach (var input in currentPage.gamepadIcons)
                {
                    var asset = inputSprite.CloneTree();
                    asset.Q<IMGUIContainer>("Key").style.backgroundImage = new StyleBackground(input);

                    keysToShowContainer.Add(asset);
                }

                root.Q<Label>("Description").text = currentPage.gamepadDescription;
            }

        }

        private void Update()
        {

            var video = tutorialEntries[currentIndex].videoPlayer;
            if (video != null)
            {
                tutorialEntries[currentIndex].videoPlayer.gameObject.SetActive(true);
                Texture2D tex = new Texture2D(280, 160, TextureFormat.RGB24, false);
                // ReadPixels looks at the active RenderTexture.
                RenderTexture.active = video.targetTexture;
                tex.ReadPixels(new Rect(0, 0, 280, 160), 0, 0);
                tex.Apply();
                uiDocument.rootVisualElement.Q<IMGUIContainer>("VideoContainer").style.backgroundImage = new StyleBackground(tex);
            }
        }

    }

}
