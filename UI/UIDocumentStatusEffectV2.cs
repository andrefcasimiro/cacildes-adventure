using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using AF.StatusEffects;

namespace AF
{
    public class UIDocumentStatusEffectV2 : MonoBehaviour, IStatusEffectUI
    {
        public VisualTreeAsset statusEntryPrefab;
        VisualElement positiveContainer;
        VisualElement negativeContainer;
        UIDocument uiDocument => GetComponent<UIDocument>();
        VisualElement root;

        public float minimumStatusBarWidth = 50f;

        // Start is called before the first frame update
        void Awake()
        {
            SetupRoot();
        }

        void SetupRoot()
        {
            root = uiDocument.rootVisualElement;

            positiveContainer = root.Q<VisualElement>("Positive");
            negativeContainer = root.Q<VisualElement>("Negative");

            positiveContainer.Clear();
            negativeContainer.Clear();
        }

        public void AddEntry(AppliedStatusEffect appliedStatusEffect, float currentMaximumResistanceToStatusEffect)
        {
            VisualElement clone = statusEntryPrefab.CloneTree();
            clone.name = appliedStatusEffect.hasReachedTotalAmount
                ? appliedStatusEffect.statusEffect.appliedName
                : appliedStatusEffect.statusEffect.builtUpName;

            clone.viewDataKey = appliedStatusEffect.statusEffect.name;

            UpdateBarFill(
                clone.Q<VisualElement>("Bar"),
                clone.Q<VisualElement>("BarFill"),
                appliedStatusEffect.currentAmount,
                Mathf.Clamp(appliedStatusEffect.currentAmount, 0, currentMaximumResistanceToStatusEffect));

            clone.Q<VisualElement>("BarFill").style.backgroundColor = appliedStatusEffect.statusEffect.barColor;
            clone.Q<IMGUIContainer>("PositiveIndicator").style.display = appliedStatusEffect.statusEffect.isPositive
                ? DisplayStyle.Flex : DisplayStyle.None;
            clone.Q<IMGUIContainer>("NegativeIndicator").style.display = appliedStatusEffect.statusEffect.isPositive
                ? DisplayStyle.None : DisplayStyle.Flex;

            if (appliedStatusEffect.statusEffect.isPositive)
            {
                positiveContainer.Add(clone);
            }
            else
            {
                negativeContainer.Add(clone);
            }
        }

        void UpdateBarFill(VisualElement backgroundBar, VisualElement barFill, float currentAmount, float maximumWidth)
        {
            float maxWidth = minimumStatusBarWidth + maximumWidth;
            backgroundBar.style.width = new Length(maxWidth, LengthUnit.Pixel);

            float fillPercentage = Mathf.Clamp01(currentAmount / maximumWidth) * 100f;
            barFill.style.width = new Length(fillPercentage, LengthUnit.Percent);
        }

        public void UpdateEntry(AppliedStatusEffect appliedStatusEffect, float currentMaximumResistanceToStatusEffect)
        {
            VisualElement statusEffectEntry = GetStatusEffectEntry(appliedStatusEffect);
            if (statusEffectEntry == null)
            {
                return;
            }

            UpdateBarFill(
                statusEffectEntry.Q<VisualElement>("Bar"),
                statusEffectEntry.Q<VisualElement>("BarFill"),
                appliedStatusEffect.currentAmount,
                currentMaximumResistanceToStatusEffect);

            var appliedStatusLabel = statusEffectEntry.Q<Label>("AppliedStatusLabel");
            appliedStatusLabel.text = appliedStatusEffect.hasReachedTotalAmount
                ? appliedStatusEffect.statusEffect.GetAppliedName() : appliedStatusEffect.statusEffect.GetName();
        }

        VisualElement GetStatusEffectEntry(AppliedStatusEffect appliedStatusEffect)
        {
            string statusEffectName = appliedStatusEffect.statusEffect.name;
            VisualElement container = appliedStatusEffect.statusEffect.isPositive ? positiveContainer : negativeContainer;

            return container.Children().FirstOrDefault(entry => entry.viewDataKey == statusEffectName);
        }

        public void RemoveEntry(AppliedStatusEffect appliedStatusEffect)
        {
            VisualElement statusEffectEntry = GetStatusEffectEntry(appliedStatusEffect);
            if (statusEffectEntry != null)
            {
                VisualElement container = appliedStatusEffect.statusEffect.isPositive ? positiveContainer : negativeContainer;
                container.Remove(statusEffectEntry);
            }
        }
    }
}
