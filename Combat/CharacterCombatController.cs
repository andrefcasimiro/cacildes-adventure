using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AF.Events;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Combat
{
    public class CharacterCombatController : MonoBehaviour
    {
        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Combat Actions")]
        public List<CombatAction> reactionsToTarget = new();
        public List<CombatAction> combatActions = new();
        public List<CombatAction> chaseActions = new();
        [HideInInspector] public CombatAction currentCombatAction;

        [Header("Cooldowns")]
        [HideInInspector] public List<CombatAction> usedCombatActions = new();

        [Header("Animation Settings")]
        public string ANIMATION_CLIP_TO_OVERRIDE_NAME = "Cacildes - Light Attack - 1";
        public readonly string hashLightAttack1 = "Light Attack 1";

        [Header("Unity Events")]
        public UnityEvent onResetState;


        public void ResetStates()
        {
            onResetState?.Invoke();

            OnAttackEnd();
        }

        CombatAction GetCombatAction()
        {
            if (reactionsToTarget.Count > 0 && (characterManager.targetManager.IsTargetBusy() || characterManager.targetManager.IsTargetShooting()))
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

            this.currentCombatAction = newCombatAction;
            ExecuteCurrentCombatAction();
        }

        public void UseChaseAction()
        {
            CombatAction newCombatAction = null;

            // If target is aiming, let us try to dodge the aim
            if (reactionsToTarget.Count > 0 && characterManager.targetManager.IsTargetShooting())
            {
                var shuffledReactions = Randomize(reactionsToTarget.ToArray());

                foreach (CombatAction possibleReaction in shuffledReactions)
                {
                    if (possibleReaction.CanUseCombatAction())
                    {
                        newCombatAction = possibleReaction;
                        break;
                    }
                }
            }
            else if (chaseActions.Count > 0)
            {
                var shuffledChaseActions = Randomize(chaseActions.ToArray());

                foreach (CombatAction possibleChaseAction in shuffledChaseActions)
                {
                    if (possibleChaseAction.CanUseCombatAction())
                    {
                        newCombatAction = possibleChaseAction;
                        break;
                    }
                }
            }

            if (newCombatAction != null)
            {
                this.currentCombatAction = newCombatAction;
                ExecuteCurrentCombatAction();
            }
        }

        public void ExecuteCurrentCombatAction()
        {
            Utils.FaceTarget(characterManager.transform, characterManager.targetManager?.currentTarget?.transform);

            if (currentCombatAction.attackAnimationClip != null)
            {
                characterManager.UpdateAnimatorOverrideControllerClips(ANIMATION_CLIP_TO_OVERRIDE_NAME, currentCombatAction.attackAnimationClip);
                characterManager.PlayBusyAnimationWithRootMotion(hashLightAttack1);
            }
            else if (!string.IsNullOrEmpty(currentCombatAction.attackAnimationName))
            {
                characterManager.PlayBusyAnimationWithRootMotion(currentCombatAction.attackAnimationName);
            }

            StartCoroutine(ClearCombatActionFromCooldownList(currentCombatAction));

            this.usedCombatActions.Add(currentCombatAction);

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
        public void OnAttack_HitboxOpen()
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

        public void SetCombatAction(CombatAction combatAction)
        {
            this.currentCombatAction = combatAction;
        }

    }
}
