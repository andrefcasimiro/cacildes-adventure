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

        EnemyManager enemyManager => GetComponent<EnemyManager>();

        // Start is called before the first frame update
        void Start()
        {
            DisableAllWeaponHitboxes();

            ShowWeapons();
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

            if (areaOfImpactFX != null)
            {
                Instantiate(areaOfImpactFX, areaOfImpactTransform);
            }

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
