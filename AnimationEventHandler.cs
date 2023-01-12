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
        [HideInInspector] public EnemyManager enemy;
        [HideInInspector] public Animator animator => GetComponent<Animator>();

        private void Start()
        {
            enemy = GetComponentInParent<EnemyManager>();
        }

        #region Attack Hitboxes
        // BUFFS
        public void CastBuff()
        {
            enemy.OnBuffStart();
        }
        public void OnBuffStart()
        {
            enemy.OnBuffStart();
        }
        public void OnBuffEnd( )
        {
            enemy.OnBuffEnd();
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
        public void ActivateLeftHandHitbox(AnimationEvent animationEvent)
        {
            enemy.ActivateLeftHandHitbox();
        }

        public void ActivateLeftWeaponHitbox()
        {
            enemy.ActivateLeftHandHitbox();
        }

        public void DeactivateLeftHandHitbox()
        {
            enemy.DeactivateLeftHandHitbox();
        }

        public void DeactivateLeftWeaponHitbox()
        {
            DeactivateRightHandHitbox();
        }


        public void ActivateRightHandHitbox()
        {
            enemy.ActivateRightHandHitbox();
        }

        public void ActivateRightWeaponHitbox()
        {
            enemy.ActivateRightHandHitbox();
        }

        public void DeactivateRightHandHitbox()
        {
            enemy.DeactivateRightHandHitbox();
        }

        public void DeactivateRightWeaponHitbox()
        {
            DeactivateRightHandHitbox();
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
        #endregion


        #region Range Weapons
        // SHOOTING
        public void ShowBow()
        {
            enemy.ShowBow();
        }
        public void HideBow()
        {
            enemy.HideBow();
        }
        public void FireProjectile()
        {
            enemy.FireProjectile();
        }
        #endregion

        #region Iframe Events
        // IFRAMING
        public void ActivateDodge()
        {
            enemy.ActivateDodge();
        }
        public void DeactivateDodge()
        {
            enemy.DeactivateDodge();
        }

        public void ActivateBlock()
        {
            enemy.ActivateBlock();
        }
        public void DeactivateBlock()
        {
            enemy.DeactivateBlock();
        }
        #endregion
    }

}
