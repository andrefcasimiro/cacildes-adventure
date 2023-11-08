using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF
{
    public class AnimationEventHandler : MonoBehaviour
    {
        [HideInInspector] public EnemyManager enemy;
        [HideInInspector] public CompanionManager companion;
        [HideInInspector] public Animator animator => GetComponent<Animator>();
        [HideInInspector] public FootstepListener footstepListener => GetComponentInParent<FootstepListener>();

        AudioSource customAudioSource => GetComponent<AudioSource>();
        public AudioClip customSfx;

        public bool playCustomSfxOnAttackAnimation = false;

        CinemachineImpulseSource impulseSource => GetComponent<CinemachineImpulseSource>();

        public GameObject prefabTest;

        private void Awake()
        {


            enemy = GetComponentInParent<EnemyManager>();
            companion = GetComponentInParent<CompanionManager>();
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

        public void PlayLargeFootstep(AnimationEvent animationEvent)
        {
            if (footstepListener == null)
            {
                return;
            }

            ShakeCameraLight();

            footstepListener.PlayFootstep(animationEvent);
        }

        public void CastBuff()
        {
            if (enemy == null)
            {
                return;
            }
            if (enemy.enemyBuffController == null || enemy.enemyBuffController.buffs.Length <= 0)
            {
                return;
            }

            enemy.enemyBuffController.OnBuffStart();
        }

        public void OnBuffCast()
        {
            if (enemy == null)
            {
                return;
            }
            if (enemy.enemyBuffController == null || enemy.enemyBuffController.buffs.Length <= 0)
            {
                return;
            }

            enemy.enemyBuffController.OnBuffStart();
        }

        public void OnBuffStart()
        {
            if (enemy == null)
            {
                return;
            }
            if (enemy.enemyBuffController == null || enemy.enemyBuffController.buffs.Length <= 0)
            {
                return;
            }

            enemy.enemyBuffController.OnBuffStart();
        }

        public void OnBuffEnd( )
        {
            if (enemy == null)
            {
                return;
            }
            if (enemy.enemyBuffController == null || enemy.enemyBuffController.buffs.Length <= 0)
            {
                return;
            }

            enemy.enemyBuffController.OnBuffEnd();
        }




        public void HitStart()
        {
            if (playCustomSfxOnAttackAnimation)
            {
                customAudioSource.PlayOneShot(customSfx);
                return;
            }

            if (enemy != null && enemy.enemyWeaponController.leftHandWeapon != null)
            {
                enemy.enemyWeaponController.ActivateLeftHandHitbox();
            }
            if (enemy != null && enemy.enemyWeaponController.rightHandWeapon != null)
            {
                enemy.enemyWeaponController.ActivateRightHandHitbox();
            }

            if (companion != null)
            {
                if (companion.leftWeapon != null)
                {
                    companion.ActivateLeftWeaponHitbox();
                }
                else if (companion.rightWeapon != null)
                {
                    companion.ActivateRightWeaponHitbox();
                }
            }
        }

        public void DualHitStart()
        {
            if (enemy != null && enemy.enemyWeaponController.leftHandWeapon != null)
            {
                enemy.enemyWeaponController.ActivateLeftHandHitbox();
            }
            if (enemy != null && enemy.enemyWeaponController.rightHandWeapon != null)
            {
                enemy.enemyWeaponController.ActivateRightHandHitbox();
            }
        }

        public void HitEnd()
        {
            if (enemy != null)
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
            if (companion != null)
            {
                if (companion.leftWeapon != null)
                {
                    companion.DeactivateLeftWeaponHitbox();
                }
                if (companion.rightWeapon != null)
                {
                    companion.DeactivateRightWeaponHitbox();
                }
            }
        }

        public void ActivateWeaponSpecial()
        {
            ActivateHitboxSpecial();
        }

        public void ActivateHitbox()
        {
            if (playCustomSfxOnAttackAnimation)
            {
                customAudioSource.PlayOneShot(customSfx);
                return;

            }

            if (enemy != null)
            {
                enemy.enemyWeaponController.ActivateLeftHandHitbox();
            }

            if (companion != null)
            {
                companion.ActivateLeftHandHitbox();
            }
        }

        public void DeactivateHitbox()
        {
            if (enemy != null)
            {
                enemy.enemyWeaponController.DeactivateLeftHandHitbox();
            }


            if (companion != null)
            {
                companion.ActivateRightHandHitbox();
            }
        }

        public void ActivateLeftHandHitbox(AnimationEvent animationEvent)
        {
            if (enemy != null)
            {
                enemy.enemyWeaponController.ActivateLeftHandHitbox();
            }
            if (companion != null)
            {
                companion.ActivateLeftHandHitbox();
            }
        }

        public void ActivateLeftWeaponHitbox()
        {
            if (enemy != null)
            {
                enemy.enemyWeaponController.ActivateLeftHandHitbox();
            }
            if (companion != null)
            {
                companion.ActivateRightHandHitbox();
            }
        }

        public void DeactivateLeftHandHitbox()
        {
            if (enemy != null)
            {
                enemy.enemyWeaponController.DeactivateLeftHandHitbox();
            }


            if (companion != null)
            {
                companion.DeactivateLeftHandHitbox();
            }
        }

        public void DeactivateLeftWeaponHitbox()
        {
            DeactivateRightHandHitbox();
        }

        public void ActivateRightHandHitbox()
        {
            if (playCustomSfxOnAttackAnimation)
            {
                customAudioSource.PlayOneShot(customSfx);
                return;

            }

            if (enemy != null)
            {
                enemy.enemyWeaponController.ActivateRightHandHitbox();
            }

            if (companion != null)
            {
                companion.ActivateRightHandHitbox();
            }

        }

        public void ActivateRightWeaponHitbox()
        {
            if (playCustomSfxOnAttackAnimation)
            {
                customAudioSource.PlayOneShot(customSfx);
                return;

            }

            if (enemy != null)
            {
                enemy.enemyWeaponController.ActivateRightHandHitbox();
            }

            if (companion != null)
            {
                companion.ActivateRightWeaponHitbox();
            }
        }

        public void DeactivateRightHandHitbox()
        {
            if (enemy != null)
            {
                enemy.enemyWeaponController.DeactivateRightHandHitbox();
            }
            if (companion != null)
            {
                companion.DeactivateRightWeaponHitbox();
            }
        }

        public void DeactivateRightWeaponHitbox()
        {
            DeactivateRightHandHitbox();
        }

        public void ActivateLeftLegHitbox()
        {
            if (enemy != null)
            {
                enemy.enemyWeaponController.ActivateLeftLegHitbox();
            }
        }

        public void DeactivateLeftLegHitbox()
        {
            if (enemy != null)
            {
                enemy.enemyWeaponController.DeactivateLeftLegHitbox();
            }
        }

        public void ActivateRightLegHitbox()
        {
            if (enemy != null)
            {
                enemy.enemyWeaponController.ActivateRightLegHitbox();
            }
        }

        public void DeactivateRightLegHitbox()
        {
            if (enemy != null)
            {
                enemy.enemyWeaponController.DeactivateRightLegHitbox();
            }
        }

        public void ActivateHeadHitbox()
        {
            if (enemy == null)
            {
                return;
            }

            enemy.enemyWeaponController.ActivateHeadHitbox();
        }

        public void DeactivateHeadHitbox()
        {
            if (enemy == null)
            {
                return;
            }
            enemy.enemyWeaponController.DeactivateHeadHitbox();
        }

        public void ActivateHeadWeaponHitbox()
        {
            if (enemy == null)
            {
                return;
            }
            enemy.enemyWeaponController.ActivateHeadHitbox();
        }

        public void DeactivateHeadWeaponHitbox()
        {
            if (enemy == null)
            {
                return;
            }
            enemy.enemyWeaponController.DeactivateHeadHitbox();
        }

        public void ActivateAreaOfImpactHitbox()
        {
            if (enemy == null)
            {
                return;
            }
            enemy.enemyWeaponController.ActivateAreaOfImpactHitbox();
        }

        public void DeactivateAreaOfImpactHitbox()
        {
            if (enemy == null)
            {
                return;
            }
            enemy.enemyWeaponController.DeactivateAreaOfImpactHitbox();
        }

        public void ShakeCameraLight()
        {
            if (impulseSource == null)
            {
                return;
            }
            impulseSource.GenerateImpulseWithForce(.2f);
        }

        public void ShakeCamera()
        {
            if (impulseSource == null)
            {
                return;
            }

            impulseSource.GenerateImpulse();
        }

        public void ActivateGolemRightLegHitbox(AnimationEvent animationEvent)
        {
            PlayFootstep(animationEvent);

            ActivateRightLegHitbox();

            ShakeCamera();

            ActivateHitboxSpecial();
        }

        public void ActivateGolemLeftLegHitbox(AnimationEvent animationEvent)
        {
            PlayFootstep(animationEvent);

            ActivateLeftLegHitbox();

            ShakeCamera();

            ActivateHitboxSpecial();
        }

        public void ActivateHitboxSpecial()
        {
            if (enemy.enemyWeaponController.leftHandWeapon != null && enemy.enemyWeaponController.leftHandWeapon.IsActive())
            {
                enemy.enemyWeaponController.leftHandWeapon.ActivateHitboxSpecial();
            }
            else if (enemy.enemyWeaponController.rightHandWeapon != null && enemy.enemyWeaponController.rightHandWeapon.IsActive())
            {
                enemy.enemyWeaponController.rightHandWeapon.ActivateHitboxSpecial();
            }
            else if (enemy.enemyWeaponController.leftLegWeapon != null && enemy.enemyWeaponController.leftLegWeapon.IsActive())
            {
                enemy.enemyWeaponController.leftLegWeapon.ActivateHitboxSpecial();
            }
            else if (enemy.enemyWeaponController.rightLegWeapon != null && enemy.enemyWeaponController.rightLegWeapon.IsActive())
            {
                enemy.enemyWeaponController.rightLegWeapon.ActivateHitboxSpecial();
            }
            else if (enemy.enemyWeaponController.headWeapon != null && enemy.enemyWeaponController.headWeapon.IsActive())
            {
                enemy.enemyWeaponController.headWeapon.ActivateHitboxSpecial();
            }
        }

        public void ActivateHitboxSpecialWithoutOpeningHitboxes()
        {
            if (enemy.enemyWeaponController.leftHandWeapon != null && enemy.enemyWeaponController.leftHandWeapon.hitboxSpecialFx != null)
            {
                enemy.enemyWeaponController.leftHandWeapon.ActivateHitboxSpecial();
            }
            else if (enemy.enemyWeaponController.rightHandWeapon != null && enemy.enemyWeaponController.rightHandWeapon.hitboxSpecialFx != null)
            {
                enemy.enemyWeaponController.rightHandWeapon.ActivateHitboxSpecial();
            }
            else if (enemy.enemyWeaponController.leftLegWeapon != null && enemy.enemyWeaponController.leftLegWeapon.hitboxSpecialFx != null)
            {
                enemy.enemyWeaponController.leftLegWeapon.ActivateHitboxSpecial();
            }
            else if (enemy.enemyWeaponController.rightLegWeapon != null && enemy.enemyWeaponController.rightLegWeapon.hitboxSpecialFx != null)
            {
                enemy.enemyWeaponController.rightLegWeapon.ActivateHitboxSpecial();
            }
            else if (enemy.enemyWeaponController.headWeapon != null && enemy.enemyWeaponController.headWeapon.hitboxSpecialFx != null)
            {
                enemy.enemyWeaponController.headWeapon.ActivateHitboxSpecial();
            }
        }

        public void ShowBow()
        {
            if (enemy == null || enemy.enemyProjectileController == null)
            {
                return;
            }

            enemy.enemyProjectileController.ShowBow();
        }

        public void HideBow()
        {
            if (enemy == null || enemy.enemyProjectileController == null)
            {
                return;
            }

            enemy.enemyProjectileController.HideBow();
        }
        public void FireProjectile()
        {
            if (playCustomSfxOnAttackAnimation)
            {
                customAudioSource.PlayOneShot(customSfx);
                return;

            }

            enemy.enemyProjectileController.FireProjectile();
        }

        public void FireMultipleProjectiles()
        {
            enemy.enemyProjectileController.FireMultipleProjectiles();
        }

        public void ActivateDodge()
        {
            if (enemy == null)
            {
                return;
            }
            if (enemy.enemyDodgeController == null)
            {
                return;
            }

            //enemy.enemyDodgeController.ActivateDodge();
        }

        public void DeactivateDodge()
        {
            if (enemy == null)
            {
                return;
            }
            if (enemy.enemyDodgeController == null)
            {
                return;
            }

            enemy.enemyDodgeController.DeactivateDodge();
        }

        public void EvaluateDecisionAfterBlockHit()
        {

            if (enemy == null)
            {
                return;
            }
            if (enemy.enemyBlockController == null)
            {
                return;
            }

            enemy.enemyBlockController.EvaluateDecisionAfterBlockHit();
        }

        public void ActivateBlock()
        {
            if (enemy == null)
            {
                return;
            }
            if (enemy.enemyBlockController == null)
            {
                return;
            }

            enemy.enemyBlockController.ActivateBlock();
        }

        public void DeactivateBlock()
        {
            if (enemy == null)
            {
                return;
            }
            if (enemy.enemyBlockController == null)
            {
                return;
            }

            enemy.enemyBlockController.DeactivateBlock();
        }

        public void EnableParry()
        {
            if (enemy == null)
            {
                return;
            }
            if (enemy.enemyPostureController == null)
            {
                return;
            }

            enemy.enemyPostureController.isParriable = true;
        }

        public void DisableParry()
        {
            if (enemy == null)
            {
                return;
            }
            if (enemy.enemyPostureController == null)
            {
                return;
            }

            enemy.enemyPostureController.isParriable = false;
        }

        public void OnGenericSfx()
        {
            if (customAudioSource != null)
            {
                if (customSfx != null)
                {
                    customAudioSource.PlayOneShot(customSfx);
                }
            }
        }

        public void LogSomething()
        {
            prefabTest.gameObject.GetComponent<ParticleSystem>().Stop();
            prefabTest.gameObject.GetComponent<ParticleSystem>().Play();
        }
    }
}
