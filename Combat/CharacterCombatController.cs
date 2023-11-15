using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF.Combat
{
    public class CharacterCombatController : MonoBehaviour
    {
        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Combat Actions")]

        public Transform reactionsToTargetContainer;
        public Transform combatActionsContainer;
        List<CombatAction> reactionsToTarget = new();
        List<CombatAction> combatActions = new();
        public CombatAction currentCombatAction;

        [Header("Cooldowns")]
        [HideInInspector] public List<CombatAction> usedCombatActions = new();

        [Header("Animation Settings")]
        public string ANIMATION_CLIP_TO_OVERRIDE_NAME = "Light Attack 1";

        private void Awake()
        {
            SetupReferences();

            EventManager.StartListening(EventMessages.ON_CHARACTER_RESET_STATE, () =>
            {
                if (EventManager.GetGameObject(EventMessages.ON_CHARACTER_RESET_STATE) == this.characterManager)
                {
                    OnAttackEnd();
                }
            });
        }

        void SetupReferences()
        {
            foreach (Transform reaction in reactionsToTargetContainer.transform)
            {
                if (reaction.TryGetComponent<CombatAction>(out var reactionToAdd))
                {
                    reactionsToTarget.Add(reactionToAdd);
                }
            }
            foreach (Transform combatAction in combatActionsContainer.transform)
            {
                if (combatAction.TryGetComponent<CombatAction>(out var combatActionToAdd))
                {
                    reactionsToTarget.Add(combatActionToAdd);
                }
            }

        }

        CombatAction GetCombatAction()
        {
            if (characterManager.targetManager.IsTargetBusy() && reactionsToTarget.Count > 0)
            {
                var shuffledReactions = Randomize(reactionsToTarget.ToArray());

                foreach (CombatAction possibleReaction in shuffledReactions)
                {
                    if (possibleReaction.CanUseCombatAction())
                    {
                        return possibleReaction;
                    }
                }
            }

            if (combatActions.Count > 0)
            {
                var shuffledCombatActions = Randomize(combatActions.ToArray());

                foreach (CombatAction possibleCombatAction in shuffledCombatActions)
                {
                    if (possibleCombatAction.CanUseCombatAction())
                    {
                        return possibleCombatAction;
                    }
                }
            }

            return null;
        }

        public void UseCombatAction()
        {
            CombatAction newCombatAction = GetCombatAction();
            if (newCombatAction == null)
            {
                return;
            }

            this.usedCombatActions.Add(newCombatAction);
            StartCoroutine(ClearCombatActionFromCooldownList(newCombatAction));

            characterManager.animator.Play(ANIMATION_CLIP_TO_OVERRIDE_NAME);
            OnAttackStart();
        }

        IEnumerator ClearCombatActionFromCooldownList(CombatAction combatActionToClear)
        {
            yield return new WaitForSeconds(combatActionToClear.maxCooldown);

            if (usedCombatActions.Contains(combatActionToClear))
            {
                usedCombatActions.Remove(combatActionToClear);
            }
        }

        public void OnAttackStart()
        {
            if (currentCombatAction != null)
            {
                currentCombatAction.onAttack_Start?.Invoke();
            }
        }
        public void OnAttack_HitStart()
        {
            if (currentCombatAction != null)
            {
                currentCombatAction.onAttack_HitboxOpen?.Invoke();
            }
        }
        public void OnAttackEnd()
        {
            if (currentCombatAction != null)
            {
                currentCombatAction.onAttack_End?.Invoke();
                currentCombatAction = null;
            }
        }

        IEnumerable<CombatAction> Randomize(CombatAction[] source)
        {
            System.Random rnd = new System.Random();
            return source.OrderBy((item) => rnd.Next());
        }

    }
}
