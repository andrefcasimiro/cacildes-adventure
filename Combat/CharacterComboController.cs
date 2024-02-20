using System.Linq;
using AF.Combat;
using UnityEngine;

namespace AF
{

    public class CharacterComboController : MonoBehaviour
    {
        public CharacterManager characterManager;

        public CombatAction[] comboActions;

        [Header("Settings")]
        public float crossFadeTime = 0.2f;

        [Range(0f, 1f)] public float chanceToUseCombo = 0.5f;


        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnCombo()
        {
            if (Random.Range(0f, 1f) < chanceToUseCombo)
            {
                return;
            }

            characterManager.isBusy = false;

            CombatAction combatAction = null;

            if (comboActions.Length > 0)
            {
                var shuffledActions = characterManager.characterCombatController.Randomize(comboActions.ToArray());

                foreach (CombatAction possibleAction in shuffledActions)
                {
                    if (possibleAction.CanUseCombatAction())
                    {
                        combatAction = possibleAction;
                        break;
                    }
                }
            }

            if (combatAction == null)
            {
                return;
            }

            characterManager.characterCombatController.currentCombatAction = combatAction;

            characterManager.characterCombatController.ExecuteCurrentCombatAction(crossFadeTime);
        }

    }
}
