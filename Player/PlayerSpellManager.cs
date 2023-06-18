using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class PlayerSpellManager : MonoBehaviour
    {
        [HideInInspector] public int hashIsSpellCasting = Animator.StringToHash("IsSpellCasting");
        Animator animator => GetComponent<Animator>();

        PlayerInventory playerInventory => GetComponent<PlayerInventory>();

        NotificationManager notificationManager;

        public LocalizedText notEnoughSpellUsageText;

        public Transform playerChestRef;
        public Transform playerFeetRef;
        public Transform playerLeftHandRef;
        public Transform playerRightHandRef;

        LockOnManager lockOnManager;

        EquipmentGraphicsHandler equipmentGraphicsHandler => GetComponent<EquipmentGraphicsHandler>();
        PlayerComponentManager playerComponentManager => GetComponent<PlayerComponentManager>();
        PlayerHealthbox playerHealthbox => GetComponentInChildren<PlayerHealthbox>();
        PlayerStatusManager playerStatusManager => GetComponent<PlayerStatusManager>();
        DefenseStatManager defenseStatManager => GetComponent<DefenseStatManager>();

        Spell currentSpell;

        private void Awake()
        {
            notificationManager = FindObjectOfType<NotificationManager>(true);

            lockOnManager = FindObjectOfType<LockOnManager>(true);
        }

        public bool ShouldInterruptSpell(int poiseDamage)
        {
            return currentSpell == null || poiseDamage > currentSpell.spellPoise;
        }

        public void PrepareSpell(Spell currentSpell)
        {
            if (IsSpellCasting())
            {
                return;
            }

            this.currentSpell = currentSpell;

            if (playerInventory.GetItemQuantity(currentSpell) <= 0)
            {
                notificationManager.ShowNotification(notEnoughSpellUsageText.GetText(), notificationManager.notEnoughSpells);
                return;
            }

            playerInventory.RemoveItem(currentSpell, 1);

            animator.Play(currentSpell.startAnimation);

            if (currentSpell.prepareSpellParticle != null)
            {
                var originTransformPosition = transform.position;

                if (currentSpell.prepareSpellParticleSpawnAtChest)
                {
                    originTransformPosition = playerChestRef.transform.position;
                }
                else if (currentSpell.prepareSpellParticleSpawnAtLeftHand)
                {
                    originTransformPosition = playerLeftHandRef.transform.position;
                }
                else if (currentSpell.prepareSpellParticleSpawnAtRightHand)
                {
                    originTransformPosition = playerRightHandRef.transform.position;
                }
                else if (currentSpell.prepareSpellParticleSpawnAtFeet)
                {
                    originTransformPosition = playerFeetRef.transform.position;
                }

                var instance = Instantiate(currentSpell.prepareSpellParticle, originTransformPosition, Quaternion.identity);

                if (currentSpell.prepareSpellParticleSpawnAtLeftHand)
                {
                    instance.transform.SetParent(playerLeftHandRef);
                }
                else if (currentSpell.prepareSpellParticleSpawnAtRightHand)
                {
                    instance.transform.SetParent(playerRightHandRef);
                }
            }

            equipmentGraphicsHandler.HideWeapons();

            playerComponentManager.DisableCharacterController();
            playerComponentManager.DisableComponents();

            playerComponentManager.GetComponent<ThirdPersonController>().enabled = true;
            playerComponentManager.GetComponent<ThirdPersonController>().canMove = false;
            playerComponentManager.GetComponent<ThirdPersonController>().canRotateCharacter = false;
        }

        /// <summary>
        /// Animation Event or called from state machine
        /// </summary>
        public void CastSpell()
        {
            if (currentSpell == null)
            {
                return;
            }

            if (currentSpell.IsLoopable())
            {
                StartCoroutine(EndSpell(currentSpell));
            }

            if (currentSpell.spellCastParticle != null)
            {
                var originTransformPosition = playerChestRef.transform.position;

                if (currentSpell.spellCastSpawnAtChest)
                {
                    originTransformPosition = playerChestRef.transform.position;
                }
                else if (currentSpell.spellCastSpawnAtLeftHand)
                {
                    originTransformPosition = playerLeftHandRef.transform.position;
                }
                else if (currentSpell.spellCastSpawnAtRightHand)
                {
                    originTransformPosition = playerRightHandRef.transform.position;
                }
                else if (currentSpell.spellCastSpawnAtFeet)
                {
                    originTransformPosition = playerFeetRef.transform.position;
                }

                Transform nearestEnemy = lockOnManager.nearestLockOnTarget != null ? lockOnManager.nearestLockOnTarget.transform : null;

                
                if (currentSpell.spawnAtNearestOrLockedOnEnemy && nearestEnemy != null)
                {
                    originTransformPosition = nearestEnemy.transform.position;
                }
                var spellRotation = Quaternion.identity;

                var spellParticle = Instantiate(currentSpell.spellCastParticle, originTransformPosition, Quaternion.identity);

                if (currentSpell.spawnTowardsNearestOrLockedOnEnemy)
                {
                    Vector3 spellTargetPosition = nearestEnemy != null ? nearestEnemy.transform.position : playerChestRef.transform.position + playerChestRef.transform.forward;

                    var spellToEnemyDistance = spellTargetPosition - spellParticle.transform.position;
                    spellToEnemyDistance.y = 0;
                    spellRotation = Quaternion.LookRotation(spellToEnemyDistance);

                    spellParticle.transform.rotation = spellRotation;

                }


                spellParticle.GetComponent<ParticleSystem>().Play();

                spellParticle.spell = currentSpell;
            }

            EvaluateSpellCastEffects(currentSpell);

            // Clear current spell
            currentSpell = null;
        }

        void EvaluateSpellCastEffects(Spell currentSpell)
        {
            if (currentSpell.selfDamageAmount != -1)
            {
                playerHealthbox.TakeEnvironmentalDamage(currentSpell.selfDamageAmount, 0, true, currentSpell.selfDamageType);
            }

            if (currentSpell.selfStatusEffect != null)
            {
                playerStatusManager.InflictStatusEffect(
                    currentSpell.selfStatusEffect,
                    currentSpell.selfStatusEffectAmount,
                    currentSpell.selfStatusEffectAmount >= defenseStatManager.GetMaximumStatusResistanceBeforeSufferingStatusEffect(
                        currentSpell.selfStatusEffect));
            }
        }

        public Transform GetNearestEnemy(float maxSpellDistance)
        {
            if (lockOnManager.nearestLockOnTarget != null)
            {
                return lockOnManager.nearestLockOnTarget.transform;
            }

            return null;
        }

        public void CancelAnySpells()
        {
            if (IsSpellCasting())
            {
                StopAllCoroutines();
            }

            currentSpell = null;
        }

        IEnumerator EndSpell(Spell spell)
        {
            yield return new WaitForSeconds(spell.spellDuration);
            animator.SetBool(hashIsSpellCasting, false);
            currentSpell = null;
        }

        public bool IsSpellCasting()
        {
            return animator.GetBool(hashIsSpellCasting);
        }
    }

}
