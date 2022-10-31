using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public enum CombatAction
    {
        Shoot,
        LightAttack,
        HeavyAttack,
        Dodge,
        Block,
    }

    public class EnemyCombatController : MonoBehaviour
    {
        public readonly int hashCombatting = Animator.StringToHash("Combatting");
        public readonly int hashWaiting = Animator.StringToHash("Waiting");
        public readonly int hashIsWaiting = Animator.StringToHash("IsWaiting");

        [Header("Weapon Hitboxes")]
        public EnemyWeaponHitbox leftHandWeapon;
        public EnemyWeaponHitbox rightHandWeapon;
        public EnemyWeaponHitbox leftLegWeapon;
        public EnemyWeaponHitbox rightLegWeapon;
        public EnemyWeaponHitbox headWeapon;
        public EnemyWeaponHitbox areaOfImpactWeapon;

        [Header("Audio Settings")]
        public AudioSource combatAudioSource;

        [Header("Area Of Impact FX")]
        public GameObject areaOfImpactFX;
        public Transform areaOfImpactTransform;

        [Header("Stats affected by animation clips")]
        public float weaponDamage = 100f;
        public StatusEffect weaponStatusEffect = null;
        public float statusEffectAmount = 0f;
        public float bonusBlockStaminaCost = 0f;

        [Header("Light Attacks")]
        [Range(0, 100)] public int lightAttackWeight = 75;

        [Header("Heavy Attacks")]
        [Range(0, 100)] public int heavyAttackWeight = 25;

        [Header("Waiting Settings")]
        [Tooltip("The higher, the more idle wait time is chanced out in combat")] public int passivityWeight = 25;
        public float minWaitingTimeBeforeResumingCombat = 0.25f;
        public float maxWaitingTimeBeforeResumingCombat = 2f;
        [HideInInspector] public float turnWaitingTime = 0f;
        protected float waitingCounter = 0f;

        [Header("Combat Flow - Action Sequences")]
        public CombatAction[] respondToPlayer;
        public CombatAction[] attackPlayer;

        private Enemy enemy => GetComponent<Enemy>();

        private void Update()
        {
            if (IsWaiting())
            {
                waitingCounter += Time.deltaTime;

                if (waitingCounter >= turnWaitingTime)
                {
                    waitingCounter = 0f;
                    turnWaitingTime = 0f;

                    enemy.animator.SetBool(hashIsWaiting, false);
                }
            }
        }

        public bool IsWaiting()
        {
            return enemy.animator.GetBool(hashIsWaiting);
        }

        public bool IsCombatting()
        {
            return enemy.animator.GetBool(hashCombatting);
        }

        public void DisableAllWeaponHitboxes()
        {
            DeactivateLeftHandHitbox();
            DeactivateRightHandHitbox();
            DeactivateLeftLegHitbox();
            DeactivateRightLegHitbox();
            DeactivateAreaOfImpactHitbox();
            DeactivateHeadHitbox();
        }

        #region Animation Events
        public void ActivateLeftHandHitbox()
        {
            if (leftHandWeapon == null)
            {
                return;
            }

            leftHandWeapon.EnableHitbox();
        }
        public void DeactivateLeftHandHitbox()
        {
            if (leftHandWeapon == null)
            {
                return;
            }

            leftHandWeapon.DisableHitbox();
        }

        public void ActivateRightHandHitbox()
        {
            if (rightHandWeapon == null)
            {
                return;
            }

            rightHandWeapon.EnableHitbox();
        }

        public void DeactivateRightHandHitbox()
        {
            if (rightHandWeapon == null)
            {
                return;
            }

            rightHandWeapon.DisableHitbox();
        }

        public void ActivateRightLegHitbox()
        {
            if (rightLegWeapon == null)
            {
                return;
            }

            rightLegWeapon.EnableHitbox();
        }

        public void DeactivateRightLegHitbox()
        {
            if (rightLegWeapon == null)
            {
                return;
            }

            rightLegWeapon.DisableHitbox();
        }

        public void ActivateLeftLegHitbox()
        {
            if (leftLegWeapon == null)
            {
                return;
            }

            leftLegWeapon.EnableHitbox();
        }

        public void DeactivateLeftLegHitbox()
        {
            if (leftLegWeapon == null)
            {
                return;
            }

            leftLegWeapon.DisableHitbox();
        }

        public void ActivateHeadHitbox()
        {
            if (headWeapon == null)
            {
                return;
            }

            headWeapon.EnableHitbox();
        }

        public void DeactivateHeadHitbox()
        {
            if (headWeapon == null)
            {
                return;
            }

            headWeapon.DisableHitbox();
        }

        public void ActivateAreaOfImpactHitbox()
        {
            if (areaOfImpactWeapon == null)
            {
                return;
            }

            Instantiate(areaOfImpactFX, areaOfImpactTransform);

            areaOfImpactWeapon.EnableHitbox();
        }

        public void DeactivateAreaOfImpactHitbox()
        {
            if (areaOfImpactWeapon == null)
            {
                return;
            }

            areaOfImpactWeapon.DisableHitbox();
        }
        #endregion
    }

}