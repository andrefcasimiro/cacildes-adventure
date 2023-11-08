using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JetBrains.Annotations;
using UnityEditor.Rendering;

namespace AF
{
    public class PlayerParryManager : MonoBehaviour
    {
        public int BASE_PARRY_DAMAGE = 30;

        public readonly int hashIsBlocking = Animator.StringToHash("IsBlocking");
        public readonly int hashIsTakingBlockHit = Animator.StringToHash("IsTakingBlockHit");

        Animator animator => GetComponent<Animator>();

        StarterAssetsInputs starterAssetsInputs => GetComponent<StarterAssetsInputs>();

        EquipmentGraphicsHandler equipmentGraphicsHandler => GetComponent<EquipmentGraphicsHandler>();

        PlayerCombatController playerCombatController => GetComponent<PlayerCombatController>();

        ClimbController climbController => GetComponent<ClimbController>();
        PlayerShootingManager playerShootingManager => GetComponent<PlayerShootingManager>();

        public float unarmedParryWindow = .4f;
        float parryTimer = 0f;

        [Tooltip("Once we stop blocking, what's the cooldown before we can block again")]
        public float maxBlockCooldown = .5f;
        float blockCooldown = 0f;

        public DestroyableParticle parryFx;

        public int unarmedDefenseAbsorption = 20;

        [Tooltip("The amount that multiplier the current attack power if we attack immediately after a parry")]
        public float counterAttackMultiplier = 1.5f;
        public float maxCounterAttackWindowAfterParry = 0.85f;
        float currentCounterAttackWindow = Mathf.Infinity;

        public bool IsWithinCounterAttackWindow()
        {
            return currentCounterAttackWindow < maxCounterAttackWindowAfterParry;
        }

        void Update()
        {
            HandleBlock();

            if (blockCooldown < maxBlockCooldown)
            {
                blockCooldown += Time.deltaTime;
            }

            if (IsParrying())
            {
                parryTimer += Time.deltaTime;
            }


            if (IsBlocking())
            {
                animator.SetBool("HasShield", Player.instance.equippedShield != null);
            }

            if (currentCounterAttackWindow < maxCounterAttackWindowAfterParry)
            {
                currentCounterAttackWindow += Time.deltaTime;
            }
        }

        void HandleBlock()
        {
            // If starting sprinting while running, cancel block
            if (IsBlocking() && starterAssetsInputs.sprint)
            {
                parryTimer = 0;
                blockCooldown = 0;
                animator.SetBool(hashIsBlocking, false);

                return;
            }

            if (!CanBlock())
            {
                return;
            }

            if (IsBlocking() && starterAssetsInputs.block == false)
            {
                parryTimer = 0;
                blockCooldown = 0;
            }

            animator.SetBool(hashIsBlocking, starterAssetsInputs.block);

        }

        bool CanBlock()
        {
            if (playerShootingManager.IsShooting())
            {
                return false;
            }

            if (starterAssetsInputs.sprint)
            {
                return false;
            }

            if (playerCombatController.isCombatting)
            {
                return false;
            }

            if (blockCooldown < maxBlockCooldown)
            {
                return false;
            }

            if (climbController.climbState != ClimbController.ClimbState.NONE)
            {
                return false;
            }

            return true;
        }

        public bool IsBlocking()
        {
            return animator.GetBool(hashIsBlocking);
        }

        public bool IsTakingBlockHit()
        {
            return animator.GetBool(hashIsTakingBlockHit);
        }

        public bool IsParrying()
        {
            if (playerCombatController.isCombatting)
            {
                return false;
            }

            if (!CanBlock())
            {
                return false;
            }

            if (IsBlocking() == false)
            {
                return false;
            }

            if (starterAssetsInputs.block == false)
            {
                return false;
            }

            return parryTimer < unarmedParryWindow;
        }

        public void InstantiateParryFx()
        {
            Transform tf = this.transform;

            if (equipmentGraphicsHandler.leftWeaponHitbox != null)
            {
                tf = equipmentGraphicsHandler.leftWeaponHitbox.transform;
            }
            else if (equipmentGraphicsHandler.leftUnarmedHitbox != null)
            {
                tf = equipmentGraphicsHandler.leftUnarmedHitbox.transform;
            }

            bool slowDown = GamePreferences.instance.ShouldSlowDownTimeWhenParrying();
            if (slowDown)
            {
                //Time.timeScale = 0.65f;
            }

            Instantiate(parryFx, tf.transform.position, Quaternion.identity);

            animator.Play("Parry Block Hit");

            currentCounterAttackWindow = 0f;

            if (slowDown)
            {
                //StartCoroutine(ResetTimeScale());
            }
        }

        IEnumerator ResetTimeScale()
        {
            yield return new WaitForSeconds(1f);
            Time.timeScale = 1f;
        }

    }

}
