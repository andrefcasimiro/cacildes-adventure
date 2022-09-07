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

        public void ActivateHitbox()
        {
            enemy.ActivateHitbox();
        }
        public void DeactivateHitbox()
        {
            enemy.DeactivateHitbox();
        }

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
