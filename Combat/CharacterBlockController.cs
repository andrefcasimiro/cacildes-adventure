using AF.Health;
using UnityEngine;

namespace AF
{
    public class CharacterBlockController : CharacterAbstractBlockController
    {

        [Header("Settings")]
        public bool shouldFaceTargetWhenBlockingAttack = true;

        public new void BlockAttack(Damage damage)
        {
            if (shouldFaceTargetWhenBlockingAttack)
            {
                (characterManager as CharacterManager)?.FaceTarget();
            }

            base.BlockAttack(damage);
        }

        public override int GetPostureDamageFromParry()
        {
            throw new System.NotImplementedException();
        }

        public override float GetUnarmedParryWindow()
        {
            return baseUnarmedParryWindow;
        }
    }
}
