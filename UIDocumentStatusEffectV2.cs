using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

namespace AF
{

    public class UIDocumentStatusEffectV2 : MonoBehaviour
    {
        UIDocument uiDocument => GetComponent<UIDocument>();

        VisualElement root;

        public VisualTreeAsset statusEntryPrefab;

        VisualElement positiveContainer;
        VisualElement negativeContainer;

        // Start is called before the first frame update
        void Awake()
        {
            this.root = uiDocument.rootVisualElement;

            positiveContainer = root.Q<VisualElement>("Positive");
            negativeContainer = root.Q<VisualElement>("Negative");

            positiveContainer.Clear();
            negativeContainer.Clear();
        }

        private void Update()
        {
            if (negativeContainer.childCount > 0)
            {
                foreach (var statusEntry in negativeContainer.Children())
                {
                    var isDisplayed = statusEntry.style.display;
                    HandleNegativeStatusEffects(statusEntry);
                }
            }
        }

        void HandleNegativeStatusEffects(VisualElement statusEntry)
        {
            var appliedStatus = Player.instance.appliedStatus.Find(x => x.statusEffect.name == statusEntry.name);

            float percentage = appliedStatus.currentAmount * 100 / statusEntry.Q<VisualElement>("Bar").style.width.value.value; //100px is the width of the status bar
            statusEntry.Q<VisualElement>("BarFill").style.width = new Length(percentage, LengthUnit.Percent);

            var appliedStatusLabel = statusEntry.Q<Label>("AppliedStatusLabel");
            appliedStatusLabel.text = appliedStatus.hasReachedTotalAmount ? appliedStatus.statusEffect.appliedStatusDisplayName : "";

            if (appliedStatus.currentAmount <= 0)
            {
                RemoveNegativeStatusEntry(appliedStatus.statusEffect);
            }
        }

        public void AddNegativeStatusEntry(StatusEffect statusEffect, float maxAmountBeforeDamage)
        {
            if (negativeContainer.Children().Any(x => x.name == statusEffect.name))
            {
                return;
            }

            VisualElement clone = statusEntryPrefab.CloneTree();
            clone.name = statusEffect.name;
            clone.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(statusEffect.spriteIndicator);
            clone.Q<Label>("AppliedStatusLabel").text = "";
            clone.Q<VisualElement>("Bar").style.width = new Length(maxAmountBeforeDamage, LengthUnit.Pixel);
            clone.Q<VisualElement>("BarFill").style.backgroundColor = statusEffect.barColor;

            negativeContainer.Add(clone);
        }

        public void RemoveNegativeStatusEntry(StatusEffect statusEffect)
        {
            VisualElement targetToRemove = negativeContainer.Children().FirstOrDefault(x => x.name == statusEffect.name);
            if (targetToRemove == null)
            {
                return;
            }

            negativeContainer.Remove(targetToRemove);
        }

        public void ClearAllNegativeStatusEntry()
        {
            negativeContainer.Clear();
        }
    }

}