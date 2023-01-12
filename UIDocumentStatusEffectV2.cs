using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using static AF.Consumable;

namespace AF
{

    public class UIDocumentStatusEffectV2 : MonoBehaviour
    {
        UIDocument uiDocument => GetComponent<UIDocument>();

        VisualElement root;

        public VisualTreeAsset statusEntryPrefab;

        VisualElement positiveContainer;
        VisualElement negativeContainer;

        DefenseStatManager defenseStatManager;

        // Start is called before the first frame update
        void Awake()
        {
            this.root = uiDocument.rootVisualElement;

            positiveContainer = root.Q<VisualElement>("Positive");
            negativeContainer = root.Q<VisualElement>("Negative");

            positiveContainer.Clear();
            negativeContainer.Clear();
        }

        private void Start()
        {
            ClearAllNegativeStatusEntry();
            ClearAllConsumableEntries();

            this.defenseStatManager = FindObjectOfType<DefenseStatManager>(true);
            foreach (var negativeStatus in Player.instance.appliedStatus)
            {
                AddNegativeStatusEntry(negativeStatus.statusEffect, defenseStatManager.GetMaximumStatusResistanceBeforeSufferingStatusEffect(negativeStatus.statusEffect));
            }

            foreach (var consumable in Player.instance.appliedConsumables)
            {
                AddConsumableEntry(consumable.consumableEffect);
            }
        }

        private void Update()
        {
            if (positiveContainer.childCount > 0)
            {
                var positiveContainerChildren = positiveContainer.Children().ToArray();
                foreach (var statusEntry in positiveContainerChildren)
                {
                    HandleConsumableEffects(statusEntry);
                }
            }
            if (negativeContainer.childCount > 0)
            {
                var negativeContainerChildren = negativeContainer.Children().ToArray();

                foreach (var statusEntry in negativeContainerChildren)
                {
                    HandleNegativeStatusEffects(statusEntry);
                }
            }
        }

        #region Negative Status Effect
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

        void HandleNegativeStatusEffects(VisualElement statusEntry)
        {
            var appliedStatus = Player.instance.appliedStatus.Find(x => x.statusEffect.name == statusEntry.name);

            float percentage = appliedStatus.currentAmount * 100 / statusEntry.Q<VisualElement>("Bar").style.width.value.value; //100px is the width of the status bar

            statusEntry.Q<VisualElement>("Bar").style.width = new Length(defenseStatManager.GetMaximumStatusResistanceBeforeSufferingStatusEffect(appliedStatus.statusEffect), LengthUnit.Pixel);


            statusEntry.Q<VisualElement>("BarFill").style.width = new Length(percentage, LengthUnit.Percent);

            var appliedStatusLabel = statusEntry.Q<Label>("AppliedStatusLabel");
            appliedStatusLabel.text = appliedStatus.hasReachedTotalAmount ? appliedStatus.statusEffect.appliedStatusDisplayName : "";

            if (appliedStatus.currentAmount <= 0)
            {
                RemoveNegativeStatusEntry(appliedStatus.statusEffect);
            }
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
        #endregion

        #region Consumable Effects
        public void AddConsumableEntry(Consumable.ConsumableEffect consumableEffect)
        {
            if (positiveContainer.Children().Any(x => x.name == consumableEffect.consumablePropertyName.ToString()))
            {
                return;
            }

            VisualElement clone = statusEntryPrefab.CloneTree();
            clone.name = consumableEffect.consumablePropertyName.ToString();
            clone.Q<IMGUIContainer>("Icon").style.backgroundImage = new StyleBackground(consumableEffect.sprite);
            clone.Q<Label>("AppliedStatusLabel").text = "";
            clone.Q<VisualElement>("BarFill").style.backgroundColor = consumableEffect.barColor;

            positiveContainer.Add(clone);
        }

        void HandleConsumableEffects(VisualElement statusEntry)
        {
            var appliedConsumable = Player.instance.appliedConsumables.Find(x => x.consumableEffect.consumablePropertyName.ToString() == statusEntry.name);

            statusEntry.Q<VisualElement>("Bar").style.width = appliedConsumable.currentDuration;

            statusEntry.Q<VisualElement>("BarFill").style.width = new StyleLength(new Length(100, LengthUnit.Percent));

            var appliedStatusLabel = statusEntry.Q<Label>("AppliedStatusLabel");
            appliedStatusLabel.text = appliedConsumable.consumableEffect.displayName;

            if (appliedConsumable.currentDuration <= 0)
            {
                RemoveConsumableEntry(appliedConsumable);
            }
        }

        public void RemoveConsumableEntry(AppliedConsumable appliedConsumable)
        {
            VisualElement targetToRemove = positiveContainer.Children().FirstOrDefault(x => x.name == appliedConsumable.consumableEffect.consumablePropertyName.ToString());
            if (targetToRemove == null)
            {
                return;
            }

            positiveContainer.Remove(targetToRemove);
        }

        public void ClearAllConsumableEntries()
        {
            positiveContainer.Clear();
        }
        #endregion
    }

}