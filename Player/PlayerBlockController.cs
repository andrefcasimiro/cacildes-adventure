using UnityEngine;

namespace AF
{
    public class PlayerBlockController : CharacterAbstractBlockController
    {
        public PlayerManager playerManager;

        public override float GetUnarmedParryWindow()
        {
            return baseUnarmedParryWindow + playerManager.statsBonusController.parryPostureWindowBonus;
        }

        public override int GetPostureDamageFromParry()
        {
            return basePostureDamageFromParry + playerManager.statsBonusController.parryPostureDamageBonus;
        }
    }
}
