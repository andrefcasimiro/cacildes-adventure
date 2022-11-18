using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;
using JetBrains.Annotations;

namespace AF
{
    public class PlayerParryManager : MonoBehaviour
    {
        public readonly int hashIsBlocking = Animator.StringToHash("IsBlocking");

        Animator animator => GetComponent<Animator>();

        StarterAssetsInputs starterAssetsInputs => GetComponent<StarterAssetsInputs>();

        EquipmentGraphicsHandler equipmentGraphicsHandler => GetComponent<EquipmentGraphicsHandler>();

        PlayerCombatController playerCombatController => GetComponent<PlayerCombatController>();

        ClimbController climbController => GetComponent<ClimbController>();

        public float unarmedParryWindow = .4f;
        float parryTimer = 0f;

        [Tooltip("Once we stop blocking, what's the cooldown before we can block again")]
        public float maxBlockCooldown = .5f;
        float blockCooldown = 0f;

        public DestroyableParticle parryFx;

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

        public bool IsBlocking() {
            return animator.GetBool(hashIsBlocking);
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

            Instantiate(parryFx, tf.transform.position, Quaternion.identity);
        }

    }

}
