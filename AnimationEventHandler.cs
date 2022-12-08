using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AF
{
    /// <summary>
    /// Animation script util for custom animation clips where we need to use the editor to call the scripts
    /// </summary>
    public class AnimationEventHandler : MonoBehaviour
    {
        [HideInInspector] public Enemy enemy;
        [HideInInspector] public EnemyBuffController enemyBuffController;
        [HideInInspector] public EnemyCombatController enemyCombatController;
        [HideInInspector] public EnemyDodgeController enemyDodgeController;
        [HideInInspector] public EnemyBlockController enemyBlockController;
        [HideInInspector] public EnemyHealthController enemyHealthController;
        [HideInInspector] public EnemyProjectileController enemyProjectileController;

        [HideInInspector] public Animator animator => GetComponent<Animator>();

        private void Start()
        {
            enemy = GetComponentInParent<Enemy>();
            enemyBuffController = GetComponentInParent<EnemyBuffController>();
            enemyCombatController = GetComponentInParent<EnemyCombatController>();
            enemyDodgeController = GetComponentInParent<EnemyDodgeController>();
            enemyBlockController = GetComponentInParent<EnemyBlockController>();
            enemyHealthController = GetComponentInParent<EnemyHealthController>();
            enemyProjectileController = GetComponentInParent<EnemyProjectileController>();
        }

        #region Attack Hitboxes
        // BUFFS
        public void CastBuff()
        {
            if (enemyBuffController == null)
            {
                return;
            }

            enemyBuffController.CastBuff();
        }

        // Adapter
        public void ActivateHitbox()
        {
            enemyCombatController.ActivateLeftHandHitbox();
        }
        public void DeactivateHitbox()
        {
            enemyCombatController.DeactivateLeftHandHitbox();
        }

        // HANDS
        public void ActivateLeftHandHitbox(AnimationEvent animationEvent)
        {
            enemyCombatController.ActivateLeftHandHitbox();
        }

        public void ActivateLeftWeaponHitbox()
        {
            enemyCombatController.ActivateLeftHandHitbox();
        }

        public void DeactivateLeftHandHitbox()
        {
            enemyCombatController.DeactivateLeftHandHitbox();
        }

        public void DeactivateLeftWeaponHitbox()
        {
            DeactivateRightHandHitbox();
        }


        public void ActivateRightHandHitbox()
        {
            enemyCombatController.ActivateRightHandHitbox();
        }

        public void ActivateRightWeaponHitbox()
        {
            enemyCombatController.ActivateRightHandHitbox();
        }

        public void DeactivateRightHandHitbox()
        {
            enemyCombatController.DeactivateRightHandHitbox();
        }

        public void DeactivateRightWeaponHitbox()
        {
            DeactivateRightHandHitbox();
        }

        // LEGS
        public void ActivateLeftLegHitbox()
        {
            enemyCombatController.ActivateLeftLegHitbox();
        }

        public void DeactivateLeftLegHitbox()
        {
            enemyCombatController.DeactivateLeftLegHitbox();
        }

        public void ActivateRightLegHitbox()
        {
            enemyCombatController.ActivateRightLegHitbox();
        }

        public void DeactivateRightLegHitbox()
        {
            enemyCombatController.DeactivateRightLegHitbox();
        }

        // HEAD

        public void ActivateHeadHitbox()
        {
            enemyCombatController.ActivateHeadHitbox();
        }

        public void DeactivateHeadHitbox()
        {
            enemyCombatController.DeactivateHeadHitbox();
        }

        // AOE

        public void ActivateAreaOfImpactHitbox()
        {
            enemyCombatController.ActivateAreaOfImpactHitbox();
        }

        public void DeactivateAreaOfImpactHitbox()
        {
            enemyCombatController.DeactivateAreaOfImpactHitbox();
        }
        #endregion


        #region Range Weapons
        // SHOOTING
        public void ShowBow()
        {
            enemyProjectileController.ShowBow();
        }
        public void HideBow()
        {
            enemyProjectileController.HideBow();
        }
        public void FireProjectile()
        {
            enemyProjectileController.FireProjectile();
        }
        #endregion

        #region Iframe Events
        // IFRAMING
        public void ActivateDodge()
        {
            enemyDodgeController.ActivateDodge();
        }
        public void DeactivateDodge()
        {
            enemyDodgeController.DeactivateDodge();
        }

        public void ActivateBlock()
        {
            enemyBlockController.ActivateBlock();
        }
        public void DeactivateBlock()
        {
            enemyBlockController.DeactivateBlock();
        }
        #endregion
    }

}
