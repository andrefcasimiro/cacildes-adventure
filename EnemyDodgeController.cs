using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class EnemyDodgeController : MonoBehaviour
    {
        [Header("Dodge Settings")]
        public AudioClip dodgeSfx;

        [Header("Trigger Settings")]
        [Range(0, 100)]
        [Tooltip("0 means never, 100 means always")] public int weight = 100;

        // Components
        private Enemy enemy => GetComponent<Enemy>();
        private EnemyHealthController enemyHealthController => GetComponent<EnemyHealthController>();
        private EnemyCombatController enemyCombatController => GetComponent<EnemyCombatController>();

        #region Animation Events
        public void ActivateDodge()
        {
            if (dodgeSfx != null && enemyCombatController.combatAudioSource != null)
            {
                BGMManager.instance.PlaySoundWithPitchVariation(dodgeSfx, enemyCombatController.combatAudioSource);
            }

            enemyHealthController.DisableHealthHitboxes();
        }
        public void DeactivateDodge()
        {
            enemyHealthController.EnableHealthHitboxes();
        }
        #endregion

    }

}