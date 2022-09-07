using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{

    public class UIDocumentStatusEffect : UIDocumentBase
    {
        VisualElement poisonedStatusElement;
        VisualElement fatigueStatusElement;

        List<StatusEffect> statusEntries = new List<StatusEffect>();

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            poisonedStatusElement = this.root.Q<VisualElement>("PoisonStatusEntry");
            poisonedStatusElement.AddToClassList("hide");
            fatigueStatusElement = this.root.Q<VisualElement>("FatigueStatusEntry");
            fatigueStatusElement.AddToClassList("hide");
        }

        public void AddStatusEntry(StatusEffect statusEffect)
        {
            if (statusEntries.Contains(statusEffect))
            {
                return;
            }

            statusEntries.Add(statusEffect);
        }

        public void ClearAllStatusEntry()
        {
            poisonedStatusElement.AddToClassList("hide");
            fatigueStatusElement.AddToClassList("hide");
            this.statusEntries.Clear();
        }

        public void RemoveStatusEntry(StatusEffect statusEffect)
        {
            if (statusEffect.name == "Poison")
            {
                poisonedStatusElement.AddToClassList("hide");
            }
            else if (statusEffect.name == "Fatigue")
            {
                fatigueStatusElement.AddToClassList("hide");
            }

            // Remove from dictionary
            statusEntries.Remove(statusEffect);
        }

        public void UpdateStatusEntry(StatusEffect statusEffect, float currentAmount, bool hasReachedTotalAmount)
        {
            VisualElement visualElement = new VisualElement();

            if (statusEffect.name == "Poison")
            {
                visualElement = poisonedStatusElement;
            }
            else if (statusEffect.name == "Fatigue")
            {
                visualElement = fatigueStatusElement;
            }

            visualElement.Q<VisualElement>("StatusVisualIndicator").style.display = DisplayStyle.Flex;
            visualElement.Q<VisualElement>("StatusVisualIndicator").style.alignItems = Align.Center;
            visualElement.Q<VisualElement>("StatusVisualIndicator").style.justifyContent = Justify.Center;

            visualElement.Q<IMGUIContainer>("StatusIcon").style.backgroundImage = new StyleBackground(statusEffect.spriteIndicator);

            // Could be influenced by a stat of the player that grants bonus resistance added to maxAmountBeforeDamage;
            visualElement.Q<ProgressBar>("StatusProgressBar").highValue = statusEffect.maxAmountBeforeDamage;

            var statusEntryVisualElementProgressBar = visualElement.Q(className: "unity-progress-bar__progress");
            statusEntryVisualElementProgressBar.style.backgroundColor = statusEffect.barColor;

            visualElement.Q<ProgressBar>("StatusProgressBar").value = currentAmount;

            Label statusName = visualElement.Q<Label>("StatusName");
            if (hasReachedTotalAmount)
            {
                statusName.RemoveFromClassList("hide");
            }
            else
            {
                statusName.AddToClassList("hide");
            }

            visualElement.RemoveFromClassList("hide");
        }

    }
}
