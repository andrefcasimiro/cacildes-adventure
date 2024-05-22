using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class PlayerBlockController : CharacterAbstractBlockController
    {
        public PlayerManager playerManager;

        bool canCounterAttack = false;

        public const string counterAttackAnimation = "CounterAttack";

        public UnityEvent onCounterAttack;

        public bool isCounterAttacking = false;

        public void ResetStates()
        {
            canCounterAttack = false;
            isCounterAttacking = false;
        }

        public override float GetUnarmedParryWindow()
        {
            return baseUnarmedParryWindow + playerManager.statsBonusController.parryPostureWindowBonus;
        }

        public override int GetPostureDamageFromParry()
        {
            return basePostureDamageFromParry + playerManager.statsBonusController.parryPostureDamageBonus;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnCounterAttack()
        {
            if (this.canCounterAttack)
            {
                this.canCounterAttack = false;

                playerManager.PlayBusyAnimationWithRootMotion(counterAttackAnimation);

                isCounterAttacking = true;
            }
        }

        public void SetCanCounterAttack(bool value)
        {
            this.canCounterAttack = value;
        }

    }
}
