﻿using UnityEngine;

namespace AF
{

    public class PlayerCombatManager : MonoBehaviour, ICombatable
    {
        Animator animator => GetComponent<Animator>();
        Player player => GetComponent<Player>();

        public AudioSource combatAudioSource;

        [Header("Combat Sounds")]
        public AudioClip startBlockingSfx;

        public bool canCombat = true;

        int attackComboIndex = 0;

        // Unarmed Attacks
        public WeaponInstance unarmedLeftHand;
        public WeaponInstance unarmedRightHand;
        public WeaponInstance unarmedFoot;

        bool hasAttacked = false;
        float timeSinceLastAttack = 0f;
        float maxIdleCombo = 2f;

        EquipmentGraphicsHandler equipmentGraphicsHandler;

        private void Awake()
        {
            equipmentGraphicsHandler = FindObjectOfType<EquipmentGraphicsHandler>(true);
        }

        private void Start()
        {
            DeactivateUnarmedHitboxes();
        }

        private void Update()
        {
            if (hasAttacked == false) return;

            if (timeSinceLastAttack > maxIdleCombo)
            {
                attackComboIndex = 0;
                timeSinceLastAttack = 0;
                hasAttacked = false;
            }
            else
            {
                timeSinceLastAttack += Time.deltaTime;
            }
        }

        public void HandleAttack()
        {
            if (!CanCombat() || player.IsNotAvailable() || player.isAttacking)
            {
                return;
            }

            if (player.isBlocking)
            {
                StopGuard();
            }

            if (attackComboIndex > 2)
            {
                attackComboIndex = 0;
            }

            if (attackComboIndex == 0)
            {
                animator.CrossFade(player.hashAttacking1, 0.05f);
            }
            else if (attackComboIndex == 1)
            {
                animator.CrossFade(player.hashAttacking2, 0.05f);
            }
            else
            {
                animator.CrossFade(player.hashAttacking3, 0.05f);
            }

            attackComboIndex++;
            hasAttacked = true;
            timeSinceLastAttack = 0f;
        }

        public void Guard()
        {
            if (
                !CanCombat()
                || player.IsNotAvailable()
                || player.isSprinting
                || PlayerInventoryManager.instance.currentShield == null)
            {
                return;
            }

            animator.SetBool(player.hashBlocking, true);

            Utils.PlaySfx(combatAudioSource, startBlockingSfx);

            if (equipmentGraphicsHandler.shieldGraphic != null)
            {
                equipmentGraphicsHandler.gameObject.SetActive(true);
            }

            if (equipmentGraphicsHandler.weaponGraphic != null)
            {
                equipmentGraphicsHandler.weaponGraphic.SetActive(false);
            }
        }

        public void StopGuard()
        {
            animator.SetBool(player.hashBlocking, false);

            // parryManager.DisableParrying();

            if (equipmentGraphicsHandler.shieldGraphic != null)
            {
                equipmentGraphicsHandler.shieldGraphic.SetActive(false);
            }


            if (equipmentGraphicsHandler.weaponGraphic != null)
            {
                equipmentGraphicsHandler.weaponGraphic.SetActive(true);
            }
        }

        public void EnableCombat()
        {
            this.canCombat = true;
        }

        public void DisableCombat()
        {
            this.canCombat = false;
        }

        public bool CanCombat()
        {
            return this.canCombat;
        }

        public void ActivateHitbox()
        {
            WeaponInstance playerWeapon = equipmentGraphicsHandler.weaponInstance;

            if (playerWeapon != null)
            {
                playerWeapon.EnableHitbox();
            }
            else
            {
                DeactivateUnarmedHitboxes();

                if (attackComboIndex == 1)
                {
                    unarmedRightHand.EnableHitbox();
                }
                else if (attackComboIndex == 2)
                {
                    unarmedLeftHand.EnableHitbox();
                }
                else if (attackComboIndex == 3)
                {
                    unarmedFoot.EnableHitbox();
                }
            }
        }

        public void DeactivateHitbox()
        {
            WeaponInstance playerWeapon = equipmentGraphicsHandler.weaponInstance;

            if (playerWeapon != null)
            {
                playerWeapon.DisableHitbox();
            }
            else
            {
                DeactivateUnarmedHitboxes();
            }
        }

        void DeactivateUnarmedHitboxes()
        {
            unarmedLeftHand.DisableHitbox();
            unarmedRightHand.DisableHitbox();
            unarmedFoot.DisableHitbox();
        }

        public void FireProjectile()
        {
            throw new System.NotImplementedException();
        }

        public IWeaponInstance GetWeaponInstance()
        {
            return equipmentGraphicsHandler.weaponInstance;
        }

        public ShieldInstance GetShieldInstance()
        {
            return equipmentGraphicsHandler.shieldInstance;
        }

        public AudioSource GetCombatantAudioSource()
        {
            return this.combatAudioSource;
        }

        public float GetCurrentHealth()
        {
            return PlayerStatsManager.instance.GetCurrentHealth();
        }

        public float GetMaxHealth()
        {
            return PlayerStatsManager.instance.GetMaxHealthPoints();
        }

        public void SetCurrentHealth(float health)
        {
            PlayerStatsManager.instance.SetCurrentHealth(health);
        }
    }

}