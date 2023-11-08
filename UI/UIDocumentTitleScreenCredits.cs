using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentTitleScreenCredits : MonoBehaviour
    {
        VisualElement root;

        public Credits credits;

        public VisualTreeAsset sectionTitle;
        public VisualTreeAsset sectionEntry;

        [Header("Localization")]
        public LocalizedText creditsText;
        public LocalizedText creditsRequestNoteText;

        [Header("Components")]
        public UIManager uiManager;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;

            root.Q<Label>("CreditsTitle").text = creditsText.GetText();
            root.Q<Label>("RequestCreditsLabel").text = creditsRequestNoteText.GetText();

            root.RegisterCallback<NavigationCancelEvent>(ev =>
            {
                Close();
            });

            DrawUI();
        }

        void DrawUI()
        {
            var closeBtn = root.Q<Button>("CloseBtn");

            var scrollPanel = root.Q<ScrollView>();

            scrollPanel.Clear();

            foreach (var creditSection in credits.creditsSections)
            {
                var sectionTitleClone = sectionTitle.CloneTree();
                sectionTitleClone.Q<Label>("SectionTitle").text = creditSection.sectionTitle.GetText();
                scrollPanel.Add(sectionTitleClone);

                foreach (var creditEntry in creditSection.creditEntry)
                {

                    var sectionEntryClone = sectionEntry.CloneTree();

                    var urlButton = sectionEntryClone.Q<Button>("UrlButton");
                    urlButton.style.display = DisplayStyle.None;

                    sectionEntryClone.Q<Label>("Author").text = creditEntry.author;
                    sectionEntryClone.Q<Label>("Description").text = creditEntry.contribution;

                    sectionTitleClone.Add(sectionEntryClone);
                }
            }

            uiManager.SetupButton(closeBtn, () =>
            {
                Close();
            });
        }

        void Close()
        {
            FindObjectOfType<UIDocumentTitleScreen>(true).gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

    }
}
