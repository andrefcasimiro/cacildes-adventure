using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIControlsLocalization : MonoBehaviour
    {
        [Header("Localization")]
        public LocalizedText controlsTitle;

        public LocalizedText walkLabel;
        public LocalizedText walkKeys;

        public LocalizedText lightAttackLabel;
        public LocalizedText lightAttackKeys;

        public LocalizedText heavyAttackLabel;
        public LocalizedText heavyAttackKeys;

        public LocalizedText blockAndParryLabel;
        public LocalizedText blockAndParryKeys;

        public LocalizedText sprintLabel;
        public LocalizedText sprintKeys;

        public LocalizedText jumpLabel;
        public LocalizedText jumpKeys;

        public LocalizedText dodgeRollLabel;
        public LocalizedText dodgeRollKeys;

        public LocalizedText interactLabel;
        public LocalizedText interactKeys;

        public LocalizedText useFavoriteItemLabel;
        public LocalizedText useFavoriteItemKeys;

        public LocalizedText switchFavoriteItemLabel;
        public LocalizedText switchFavoriteItemKeys;

        public LocalizedText openMenuLabel;
        public LocalizedText openMenuText;

        public LocalizedText toggleAndWalkLabel;
        public LocalizedText toggleAndWalkText;

        public void Translate(VisualElement root)
        {
            root.Q<Label>("ControlsTitle").text = controlsTitle.GetEnglishText();

            SetupKey(root, "Walk", walkLabel, walkKeys);
            SetupKey(root, "LightAttack", lightAttackLabel, lightAttackKeys);
            SetupKey(root, "HeavyAttack", heavyAttackLabel, heavyAttackKeys);
            SetupKey(root, "BlockAndParry", blockAndParryLabel, blockAndParryKeys);
            SetupKey(root, "Sprint", sprintLabel, sprintKeys);
            SetupKey(root, "Jump", jumpLabel, jumpKeys);
            SetupKey(root, "DodgeRoll", dodgeRollLabel, dodgeRollKeys);
            SetupKey(root, "Interact", interactLabel, interactKeys);
            SetupKey(root, "UseFavoriteItem", useFavoriteItemLabel, useFavoriteItemKeys);
            SetupKey(root, "SwitchFavoriteItem", switchFavoriteItemLabel, switchFavoriteItemKeys);
            SetupKey(root, "OpenMenu", openMenuLabel, openMenuText);
            SetupKey(root, "ToggleWalkAndRun", toggleAndWalkLabel, toggleAndWalkText);
        }

        void SetupKey(VisualElement root, string path, LocalizedText label, LocalizedText text)
        {
            var labelElement = root.Q<VisualElement>(path).ElementAt(0) as Label;
            labelElement.text = label.GetEnglishText();
            var valueElement = root.Q<VisualElement>(path).ElementAt(1).Q<Label>();
            valueElement.text = text.GetEnglishText();
        }
    }
}
