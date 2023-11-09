using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentLoadMenu : MonoBehaviour
    {
        public VisualTreeAsset loadButtonEntryPrefab;


        [Header("Localization")]
        public LocalizedText openSavesFolderText;
        public LocalizedText loadText;
        public LocalizedText deleteText;

        [Header("Components")]
        public UIManager uIManager;

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            DrawUI(root, true);
        }

        public void DrawUI(VisualElement targetRoot, bool showDeleteButton)
        {
        }

        void ShowLoadInfo(VisualElement root, object saveFileEntry)
        {
            /*root.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(saveFileEntry.screenshot);
            root.Q<Label>("Title").text = saveFileEntry.sceneName + " / Level " + saveFileEntry.level;
            root.Q<Label>("SubTitle").text = LocalizedTerms.SavedAt() + ": " + DateTime.Parse(saveFileEntry.creationDate).ToString();

            var totalGameTime = System.TimeSpan.FromSeconds(saveFileEntry.gameTime);
            root.Q<Label>("Description").text = LocalizedTerms.TotalPlayTime() + ": " + totalGameTime.Hours + "h " + totalGameTime.Minutes + "m " + totalGameTime.Seconds + "s";
            root.Q<Label>("CurrentObjectiveText").text = ""; //+ saveFileEntry.currentObjective;

            root.Q<VisualElement>("SaveGameItemPreview").style.opacity = 1;*/
        }
    }
}
