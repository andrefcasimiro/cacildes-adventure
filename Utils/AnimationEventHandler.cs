using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    /// <summary>
    /// Animation script util for custom animation clips where we need to use the editor to call the scripts
    /// </summary>
    public class AnimationEventHandler : MonoBehaviour
    {
        public Enemy enemy;
        public EnemyHealthbox enemyHealthbox;

        // BUFFS
        public void CastBuff()
        {
            enemy.CastBuff();
        }

        // Adapter
        public void ActivateHitbox()
        {
            enemy.ActivateLeftHandHitbox();
        }
        public void DeactivateHitbox()
        {
            enemy.DeactivateLeftHandHitbox();
        }

        // HANDS
        public void ActivateLeftHandHitbox()
        {
            enemy.ActivateLeftHandHitbox();
        }
        
        public void DeactivateLeftHandHitbox()
        {
            enemy.DeactivateLeftHandHitbox();
        }
        
        public void ActivateRightHandHitbox()
        {
            enemy.ActivateRightHandHitbox();
        }

        public void DeactivateRightHandHitbox()
        {
            enemy.DeactivateRightHandHitbox();
        }

        // LEGS
        public void ActivateLeftLegHitbox()
        {
            enemy.ActivateLeftLegHitbox();
        }

        public void DeactivateLeftLegHitbox()
        {
            enemy.DeactivateLeftLegHitbox();
        }

        public void ActivateRightLegHitbox()
        {
            enemy.ActivateRightLegHitbox();
        }

        public void DeactivateRightLegHitbox()
        {
            enemy.DeactivateRightLegHitbox();
        }

        // HEAD

        public void ActivateHeadHitbox()
        {
            enemy.ActivateHeadHitbox();
        }

        public void DeactivateHeadHitbox()
        {
            enemy.DeactivateHeadHitbox();
        }

        // AOE

        public void ActivateAreaOfImpactHitbox()
        {
            enemy.ActivateAreaOfImpactHitbox();
        }

        public void DeactivateAreaOfImpactHitbox()
        {
            enemy.DeactivateAreaOfImpactHitbox();
        }

        // SHOOTING

        public void FireProjectile()
        {
            enemy.FireProjectile();
        }

        public void ActivateDodge()
        {
            if (enemy.dodgeSfx != null && enemy.combatAudioSource != null)
            {
                enemy.combatAudioSource.PlayOneShot(enemy.dodgeSfx);
            }

            enemyHealthbox.ActivateDodge();
        }
        public void DeactivateDodge()
        {
            if (enemy.dodgeSfx != null && enemy.combatAudioSource != null)
            {
                enemy.combatAudioSource.PlayOneShot(enemy.dodgeSfx);
            }

            enemyHealthbox.DeactivateDodge();
        }

        public void ActivateBlock()
        {
            enemyHealthbox.ActivateBlock();
        }
        public void DeactivateBlock()
        {
            enemyHealthbox.DeactivateBlock();
        }

        public void ActivateParriable()
        {
            enemyHealthbox.ActivateParriable();
        }
        public void DeactivateParriable()
        {
            enemyHealthbox.DeactivateParriable();
        }

        public void PlayGroundImpact()
        {
            enemy.PlayGroundImpact();
        }
    }

}
