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

        [Header("Directional")]
        public CombatAction reactionToTargetBehindBack;
        public CombatAction currentCombatAction = null;

        [Header("Combat Options")]
        [Range(0, 100f)] public float chanceToReact = 90f;

        public List<CombatAction> usedCombatActions = new();

        [Header("Animation Settings")]
        public string ANIMATION_CLIP_TO_OVERRIDE_NAME = "Cacildes - Light Attack - 1";
        public string COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK = "Cacildes - Combo Attack";
        public string COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_FOLLOWUP_ATTACK = "Cacildes - Combo Attack - Follow Up";
        public string hashLightAttack1 = "Light Attack 1";
        public string hashComboAttack = "Combo Attack Initiator";

        [Header("Unity Events")]
        public UnityEvent onResetState;

        public const string AttackSpeedHash = "AttackSpeed";

        private void Awake()
        {
            characterManager.animator.SetFloat(AttackSpeedHash, 1f);

        }

        public void ResetStates()
        {
            characterManager.animator.SetFloat(AttackSpeedHash, 1f);

            onResetState?.Invoke();

            OnAttackEnd();
        }

        bool CanReact()
        {
            if (reactionsToTarget.Count <= 0)
            {
                return false;
            }

            if (Random.Range(0, 100) > chanceToReact)
            {
                return false;
            }

            return characterManager.targetManager.IsTargetBusy() || characterManager.targetManager.IsTargetShooting();
        }

        bool IsTargetBehind()
        {
            if (characterManager.targetManager == null || characterManager.targetManager.currentTarget == null)
            {
                return false;
            }

            // Calculate vector from enemy to player
            Vector3 toPlayer = characterManager.targetManager.currentTarget.transform.position - characterManager.transform.position;

            // Calculate angle between enemy's forward direction and vector to player
            float angle = Vector3.Angle(characterManager.transform.forward, toPlayer);

            return angle > 90f;
        }

        CombatAction GetCombatAction()
        {
            if (CanReact())
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

            if (reactionToTargetBehindBack != null && IsTargetBehind())
            {
                return reactionToTargetBehindBack;
            }

            if (combatActions.Count > 0)
            {
                var shuffledCombatActions = Randomize(combatActions.ToArray());

                foreach (CombatAction possibleCombatAction in shuffledCombatActions)
                {
                    if (possibleCombatAction != null && possibleCombatAction.CanUseCombatAction())
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
            ExecuteCurrentCombatAction(0f);
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
                ExecuteCurrentCombatAction(0f);
            }
        }

        public void ExecuteCurrentCombatAction(float crossFade)
        {
            if (currentCombatAction != reactionToTargetBehindBack)
            {
                characterManager.FaceTarget();
            }

            if (currentCombatAction.attackAnimationClip != null)
            {

                if (currentCombatAction.comboClip != null)
                {
                    characterManager.UpdateAnimatorOverrideControllerClips(COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_ATTACK, currentCombatAction.attackAnimationClip);
                    characterManager.UpdateAnimatorOverrideControllerClips(COMBO_ANIMATION_CLIP_TO_OVERRIDE_NAME_FOLLOWUP_ATTACK, currentCombatAction.comboClip);
                }
                else
                {
                    characterManager.UpdateAnimatorOverrideControllerClips(ANIMATION_CLIP_TO_OVERRIDE_NAME, currentCombatAction.attackAnimationClip);
                }

                characterManager.animator.ForceStateNormalizedTime(0f);

                if (currentCombatAction.animationSpeed != 1f)
                {
                    characterManager.animator.SetFloat(AttackSpeedHash, currentCombatAction.animationSpeed);
                }

                if (currentCombatAction.comboClip != null)
                {
                    characterManager.PlayBusyAnimationWithRootMotion(hashComboAttack);
                }
                else if (crossFade > 0)
                {
                    characterManager.PlayAnimationWithCrossFade(hashLightAttack1, true, true, crossFade);
                }
                else
                {
                    characterManager.PlayBusyAnimationWithRootMotion(hashLightAttack1);
                }
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

        public IEnumerable<CombatAction> Randomize(CombatAction[] source)
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
