using UnityEngine;
using NUnit.Framework;
using AF.Combat;

namespace AF.Tests
{
    public class CharacterCombatController_Tests : MonoBehaviour
    {
        CharacterCombatController characterCombatController;

        [SetUp]
        public void SetUp()
        {
            characterCombatController = new GameObject().AddComponent<CharacterCombatController>();
            CharacterManager characterManager = characterCombatController.gameObject.AddComponent<CharacterManager>();
            characterCombatController.characterManager = characterManager;
            Animator animator = characterCombatController.characterManager.gameObject.AddComponent<Animator>();
            characterCombatController.characterManager.animator = animator;
            characterCombatController.characterManager.animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

            CombatAction reactionAction1 = new GameObject().AddComponent<CombatAction>();
            characterCombatController.reactionsToTarget.Add(reactionAction1);
            reactionAction1.onAttack_Start = new UnityEngine.Events.UnityEvent();
            reactionAction1.onAttack_HitboxOpen = new UnityEngine.Events.UnityEvent();
            reactionAction1.onAttack_End = new UnityEngine.Events.UnityEvent();

            CombatAction combatAction1 = new GameObject().AddComponent<CombatAction>();
            characterCombatController.combatActions.Add(combatAction1);
            combatAction1.onAttack_Start = new UnityEngine.Events.UnityEvent();
            combatAction1.onAttack_HitboxOpen = new UnityEngine.Events.UnityEvent();
            combatAction1.onAttack_End = new UnityEngine.Events.UnityEvent();
        }

        [Test]
        public void ShouldCall_CombatAction_OnAttackStartEvent()
        {
            bool wasCalled = false;

            characterCombatController.combatActions[0].onAttack_Start.AddListener(() =>
            {
                wasCalled = true;
            });

            characterCombatController.currentCombatAction = characterCombatController.combatActions[0];

            Assert.IsFalse(wasCalled);
            characterCombatController.ExecuteCurrentCombatAction(0f);
            Assert.IsTrue(wasCalled);
        }

    }

}
