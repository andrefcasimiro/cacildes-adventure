using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class EnemyWeaponController : MonoBehaviour
    {
        [Header("Weapon Hitboxes")]
        public EnemyWeaponHitbox leftHandWeapon;
        public EnemyWeaponHitbox rightHandWeapon;
        public EnemyWeaponHitbox leftLegWeapon;
        public EnemyWeaponHitbox rightLegWeapon;
        public EnemyWeaponHitbox headWeapon;
        public EnemyWeaponHitbox areaOfImpactWeapon;

        [Header("Area Of Impact FX")]
        public GameObject areaOfImpactFX;
        public Transform areaOfImpactTransform;

        [Header("Options")]
        public bool hideWeaponsOnStart = false;

        CharacterManager characterManager => GetComponent<CharacterManager>();

        // Start is called before the first frame update
        void Start()
        {

            if (hideWeaponsOnStart)
            {
                HideWeapons();
            }
            else
            {
                ShowWeapons();
            }

            DisableAllWeaponHitboxes();
        }

        public void ShowWeapons()
        {
            if (leftHandWeapon != null)
            {
                leftHandWeapon.gameObject.SetActive(true);
            }

            if (rightHandWeapon != null)
            {
                rightHandWeapon.gameObject.SetActive(true);
            }
        }

        public void HideWeapons()
        {
            if (leftHandWeapon != null)
            {
                leftHandWeapon.gameObject.SetActive(false);
            }

            if (rightHandWeapon != null)
            {
                rightHandWeapon.gameObject.SetActive(false);
            }
        }

        #region Weapon Hitboxes

        public void DisableAllWeaponHitboxes()
        {
            DeactivateLeftHandHitbox();
            DeactivateRightHandHitbox();
            DeactivateLeftLegHitbox();
            DeactivateRightLegHitbox();
            DeactivateAreaOfImpactHitbox();
            DeactivateHeadHitbox();
        }
        public void ActivateLeftHandHitbox()
        {
            if (leftHandWeapon == null)
            {
                return;
            }

            leftHandWeapon.EnableHitbox();

            HandleWeaponBonus(leftHandWeapon, true);
        }
        public void DeactivateLeftHandHitbox()
        {
            if (leftHandWeapon == null)
            {
                return;
            }

            leftHandWeapon.DisableHitbox();

            HandleWeaponBonus(leftHandWeapon, false);
        }

        public void ActivateRightHandHitbox()
        {
            if (rightHandWeapon == null)
            {
                return;
            }

            rightHandWeapon.EnableHitbox();

            HandleWeaponBonus(rightHandWeapon, true);
        }

        public void DeactivateRightHandHitbox()
        {
            if (rightHandWeapon == null)
            {
                return;
            }

            rightHandWeapon.DisableHitbox();

            HandleWeaponBonus(rightHandWeapon, false);
        }

        public void ActivateRightLegHitbox()
        {
            if (rightLegWeapon == null)
            {
                return;
            }

            rightLegWeapon.EnableHitbox();
            HandleWeaponBonus(rightLegWeapon, true);
        }

        public void DeactivateRightLegHitbox()
        {
            if (rightLegWeapon == null)
            {
                return;
            }

            rightLegWeapon.DisableHitbox();
            HandleWeaponBonus(rightLegWeapon, false);
        }

        public void ActivateLeftLegHitbox()
        {
            if (leftLegWeapon == null)
            {
                return;
            }

            leftLegWeapon.EnableHitbox();
            HandleWeaponBonus(leftLegWeapon, true);
        }

        public void DeactivateLeftLegHitbox()
        {
            if (leftLegWeapon == null)
            {
                return;
            }

            leftLegWeapon.DisableHitbox();
            HandleWeaponBonus(leftLegWeapon, false);
        }

        public void ActivateHeadHitbox()
        {
            if (headWeapon == null)
            {
                return;
            }

            headWeapon.EnableHitbox();
            HandleWeaponBonus(headWeapon, true);
        }

        public void DeactivateHeadHitbox()
        {
            if (headWeapon == null)
            {
                return;
            }

            headWeapon.DisableHitbox();
            HandleWeaponBonus(headWeapon, false);
        }

        public void ActivateAreaOfImpactHitbox()
        {
            if (areaOfImpactWeapon == null)
            {
                return;
            }

            if (areaOfImpactFX != null)
            {
                Instantiate(areaOfImpactFX, areaOfImpactTransform);
            }

            areaOfImpactWeapon.EnableHitbox();
            HandleWeaponBonus(areaOfImpactWeapon, true);
        }

        public void DeactivateAreaOfImpactHitbox()
        {
            if (areaOfImpactWeapon == null)
            {
                return;
            }

            areaOfImpactWeapon.DisableHitbox();
            HandleWeaponBonus(areaOfImpactWeapon, false);
        }
        #endregion


        public void HandleWeaponBonus(EnemyWeaponHitbox weapon, bool isAttacking)
        {/*
            if (weapon.poiseDamage != 0)
            {
                characterManager.enemyCombatController.currentAttackPoiseDamage = isAttacking
                        ? characterManager.enemyCombatController.currentAttackPoiseDamage + weapon.poiseDamage
                        : characterManager.enemyCombatController.currentAttackPoiseDamage - weapon.poiseDamage;
            }

            if (weapon.statusEffect != null && weapon.statusEffectAmountPerHit != 0)
            {
                characterManager.enemyCombatController.weaponStatusEffect = isAttacking ? weapon.statusEffect : null;
                characterManager.enemyCombatController.statusEffectAmount = isAttacking
                        ? characterManager.enemyCombatController.statusEffectAmount + weapon.statusEffectAmountPerHit
                        : characterManager.enemyCombatController.statusEffectAmount - weapon.statusEffectAmountPerHit;
            }
            if (weapon.blockStaminaCost != 0)
            {
                characterManager.enemyCombatController.bonusBlockStaminaCost = isAttacking
                        ? characterManager.enemyCombatController.bonusBlockStaminaCost + weapon.blockStaminaCost
                        : characterManager.enemyCombatController.bonusBlockStaminaCost - weapon.blockStaminaCost;
            }*/
        }

    }
}
