using AF.Events;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Conditions
{
    public class ArmorDependant : MonoBehaviour
    {
        [Header("Equipment Conditions")]
        public Helmet helmet;
        public Armor armor;
        public Gauntlet gauntlet;
        public Legwear legwear;

        [Header("Settings")]
        public bool requireOnlyTorsoArmorToBeEquipped = false;
        public bool requireAllPiecesToBeEquipped = false;
        public bool requireNoneOfThePiecesToBeEquipped = false;

        [Header("Naked Conditions")]
        public bool requirePlayerToBeNaked = false;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;

        [Header("Events")]
        public UnityEvent onTrue;
        public UnityEvent onFalse;

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_EQUIPMENT_CHANGED, Evaluate);

            Evaluate();
        }

        public void Evaluate()
        {
            bool evaluationResult = false;

            if (requirePlayerToBeNaked)
            {
                evaluationResult = equipmentDatabase.IsPlayerNaked();
            }
            else if (requireAllPiecesToBeEquipped)
            {
                evaluationResult = equipmentDatabase.helmet == helmet
                && equipmentDatabase.armor == armor
                && equipmentDatabase.legwear == legwear
                && equipmentDatabase.gauntlet == gauntlet;
            }
            else if (requireNoneOfThePiecesToBeEquipped)
            {
                evaluationResult =
                    !(equipmentDatabase.helmet != helmet
                    || equipmentDatabase.armor != armor
                    || equipmentDatabase.legwear != legwear
                    || equipmentDatabase.gauntlet != gauntlet);
            }
            else if (requireOnlyTorsoArmorToBeEquipped)
            {
                evaluationResult = equipmentDatabase.armor == armor;
            }

            Utils.UpdateTransformChildren(transform, evaluationResult);

            if (evaluationResult)
            {
                onTrue?.Invoke();
            }
            else
            {
                onFalse?.Invoke();
            }
        }
    }
}
