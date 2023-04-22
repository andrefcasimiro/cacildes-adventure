using UnityEngine;

namespace AF
{
    public class AnimationEventHandler : MonoBehaviour
    {
        [HideInInspector] public EnemyManager enemy;
        [HideInInspector] public Animator animator => GetComponent<Animator>();
        [HideInInspector] public FootstepListener footstepListener => GetComponentInParent<FootstepListener>();

        private void Start()
        {
            enemy = GetComponentInParent<EnemyManager>();
        }

        public void FacePlayer()
        {
            enemy.facePlayer = true;
        }

        public void CheckCombo()
        {
            var dice = Random.Range(0, 100);
            if (dice < 50)
            {
                // Interrupt combo
                enemy.animator.Play(enemy.hashWaiting);
            }
        }

        public void PlayFootstep(AnimationEvent animationEvent)
        {
            if (footstepListener == null)
            {
                return;
            }

            footstepListener.PlayFootstep(animationEvent);
        }

        public void CastBuff()
        {
            if (enemy.enemyBuffController == null || enemy.enemyBuffController.buffs.Length <= 0)
            {
                return;
            }

            enemy.enemyBuffController.OnBuffStart();
        }

        public void OnBuffCast()
        {
            if (enemy.enemyBuffController == null || enemy.enemyBuffController.buffs.Length <= 0)
            {
                return;
            }

            enemy.enemyBuffController.OnBuffStart();
        }

        public void OnBuffStart()
        {
            if (enemy.enemyBuffController == null || enemy.enemyBuffController.buffs.Length <= 0)
            {
                return;
            }

            enemy.enemyBuffController.OnBuffStart();
        }

        public void OnBuffEnd( )
        {
            if (enemy.enemyBuffController == null || enemy.enemyBuffController.buffs.Length <= 0)
            {
                return;
            }

            enemy.enemyBuffController.OnBuffEnd();
        }
        public void HitStart()
        {
            if (enemy.enemyWeaponController.leftHandWeapon != null)
            {
                enemy.enemyWeaponController.ActivateLeftHandHitbox();
            }
            if (enemy.enemyWeaponController.rightHandWeapon != null)
            {
                enemy.enemyWeaponController.ActivateRightHandHitbox();
            }
        }
        public void HitEnd()
        {
            if (enemy.enemyWeaponController.leftHandWeapon != null)
            {
                enemy.enemyWeaponController.DeactivateLeftHandHitbox();
            }
            if (enemy.enemyWeaponController.rightHandWeapon != null)
            {
                enemy.enemyWeaponController.DeactivateRightHandHitbox();
            }
        }

        public void ActivateHitbox()
        {
            enemy.enemyWeaponController.ActivateLeftHandHitbox();
        }
        public void DeactivateHitbox()
        {
            enemy.enemyWeaponController.DeactivateLeftHandHitbox();
        }

        public void ActivateLeftHandHitbox(AnimationEvent animationEvent)
        {
            enemy.enemyWeaponController.ActivateLeftHandHitbox();
        }

        public void ActivateLeftWeaponHitbox()
        {
            enemy.enemyWeaponController.ActivateLeftHandHitbox();
        }

        public void DeactivateLeftHandHitbox()
        {
            enemy.enemyWeaponController.DeactivateLeftHandHitbox();
        }

        public void DeactivateLeftWeaponHitbox()
        {
            DeactivateRightHandHitbox();
        }

        public void ActivateRightHandHitbox()
        {
            enemy.enemyWeaponController.ActivateRightHandHitbox();
        }

        public void ActivateRightWeaponHitbox()
        {
            enemy.enemyWeaponController.ActivateRightHandHitbox();
        }

        public void DeactivateRightHandHitbox()
        {
            enemy.enemyWeaponController.DeactivateRightHandHitbox();
        }

        public void DeactivateRightWeaponHitbox()
        {
            DeactivateRightHandHitbox();
        }

        public void ActivateLeftLegHitbox()
        {
            enemy.enemyWeaponController.ActivateLeftLegHitbox();
        }

        public void DeactivateLeftLegHitbox()
        {
            enemy.enemyWeaponController.DeactivateLeftLegHitbox();
        }

        public void ActivateRightLegHitbox()
        {
            enemy.enemyWeaponController.ActivateRightLegHitbox();
        }

        public void DeactivateRightLegHitbox()
        {
            enemy.enemyWeaponController.DeactivateRightLegHitbox();
        }

        public void ActivateHeadHitbox()
        {
            enemy.enemyWeaponController.ActivateHeadHitbox();
        }

        public void DeactivateHeadHitbox()
        {
            enemy.enemyWeaponController.DeactivateHeadHitbox();
        }

        public void ActivateHeadWeaponHitbox()
        {
            enemy.enemyWeaponController.ActivateHeadHitbox();
        }

        public void DeactivateHeadWeaponHitbox()
        {
            enemy.enemyWeaponController.DeactivateHeadHitbox();
        }

        public void ActivateAreaOfImpactHitbox()
        {
            enemy.enemyWeaponController.ActivateAreaOfImpactHitbox();
        }

        public void DeactivateAreaOfImpactHitbox()
        {
            enemy.enemyWeaponController.DeactivateAreaOfImpactHitbox();
        }

        public void ShowBow()
        {
            if (enemy.enemyProjectileController == null)
            {
                return;
            }

            enemy.enemyProjectileController.ShowBow();
        }

        public void HideBow()
        {
            if (enemy.enemyProjectileController == null)
            {
                return;
            }

            enemy.enemyProjectileController.HideBow();
        }
        public void FireProjectile()
        {
            enemy.enemyProjectileController.FireProjectile();
        }

        public void ActivateDodge()
        {
            if (enemy.enemyDodgeController == null)
            {
                return;
            }

            enemy.enemyDodgeController.ActivateDodge();
        }

        public void DeactivateDodge()
        {
            if (enemy.enemyDodgeController == null)
            {
                return;
            }

            enemy.enemyDodgeController.DeactivateDodge();
        }

        public void EvaluateDecisionAfterBlockHit()
        {

            if (enemy.enemyBlockController == null)
            {
                return;
            }

            enemy.enemyBlockController.EvaluateDecisionAfterBlockHit();
        }

        public void ActivateBlock()
        {
            if (enemy.enemyBlockController == null)
            {
                return;
            }

            enemy.enemyBlockController.ActivateBlock();
        }

        public void DeactivateBlock()
        {
            if (enemy.enemyBlockController == null)
            {
                return;
            }

            enemy.enemyBlockController.DeactivateBlock();
        }

        public void EnableParry()
        {
            if (enemy.enemyPostureController == null)
            {
                return;
            }

            enemy.enemyPostureController.isParriable = true;
        }

        public void DisableParry()
        {
            if (enemy.enemyPostureController == null)
            {
                return;
            }

            enemy.enemyPostureController.isParriable = false;
        }
    }
}
