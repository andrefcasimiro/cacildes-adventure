using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentTitleScreenCredits : MonoBehaviour
    {
        VisualElement root;

        public Credits credits;

        public VisualTreeAsset sectionTitle;
        public VisualTreeAsset sectionEntry;
        ScrollView scrollPanel;

        [Header("Components")]
        public UIManager uiManager;
        public Soundbank soundbank;

        [Header("UI Components")]
        public UIDocumentTitleScreen uIDocumentTitleScreen;

        [Header("Localization")]
        public LocalizedString ReturnToTitleScreen_LocalizedString;

        private void Awake()
        {
            gameObject.SetActive(false);
        }


        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnClose()
        {
            if (this.isActiveAndEnabled)
            {
                Close();
            }
        }

        private void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            scrollPanel = root.Q<ScrollView>();

            DrawUI();
        }

        void DrawUI()
        {
            scrollPanel.Clear();

            Button exitButton = new()
            {
                text = ReturnToTitleScreen_LocalizedString.GetLocalizedString()
            };
            scrollPanel.Add(exitButton);

            exitButton.AddToClassList("primary-button");
            UIUtils.SetupButton(exitButton, () =>
            {
                Close();
            }, soundbank);

            exitButton.RegisterCallback<FocusInEvent>((ev) =>
            {
                scrollPanel.ScrollTo(exitButton);
            });

            exitButton.Focus();

            foreach (var creditSection in credits.creditsSections)
            {
                var sectionTitleClone = sectionTitle.CloneTree();
                sectionTitleClone.Q<Label>("SectionTitle").text = creditSection.sectionTitle;
                scrollPanel.Add(sectionTitleClone);

                foreach (var creditEntry in creditSection.creditEntry)
                {

                    var sectionEntryClone = sectionEntry.CloneTree();

                    var urlButton = sectionEntryClone.Q<Button>("UrlButton");
                    urlButton.style.display = DisplayStyle.None;

                    sectionEntryClone.Q<Label>("Author").text = creditEntry.author;
                    sectionEntryClone.Q<Label>("Description").text = creditEntry.contribution;

                    sectionTitleClone.Add(sectionEntryClone);

                    sectionEntryClone.RegisterCallback<FocusInEvent>((ev) =>
                    {
                        scrollPanel.ScrollTo(sectionEntryClone);
                    });
                }
            }
        }

        void Close()
        {
            uIDocumentTitleScreen.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

    }
}
