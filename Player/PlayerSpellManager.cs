using System.Collections;
using System.Collections.Generic;
using AF.Stats;
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
        //PlayerStatusManager playerStatusManager => GetComponent<PlayerStatusManager>();
        DefenseStatManager defenseStatManager => GetComponent<DefenseStatManager>();

        Spell currentSpell;

        [Header("Components")]
        public StatsBonusController statsBonusController;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;

        private void Awake()
        {
            notificationManager = FindObjectOfType<NotificationManager>(true);

            lockOnManager = FindObjectOfType<LockOnManager>(true);
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

            if (currentSpell.spellCastParticle != null)
            {
                var originTransformPosition = playerChestRef.transform.position;

                var spellParticle = Instantiate(currentSpell.spellCastParticle, originTransformPosition, Quaternion.identity);

                spellParticle.GetComponent<ParticleSystem>().Play();

                //                spellParticle.statsBonusController = statsBonusController;
                //                spellParticle.spell = currentSpell;
            }

            EvaluateSpellCastEffects(currentSpell);

            // Clear current spell
            currentSpell = null;
        }

        void EvaluateSpellCastEffects(Spell currentSpell)
        {
            if (currentSpell.healingAmount != -1)
            {
            }

            if (currentSpell.selfDamageAmount != -1)
            {
            }

            if (currentSpell.selfStatusEffect != null)
            {
                /* playerStatusManager.InflictStatusEffect(
                     currentSpell.selfStatusEffect,
                     currentSpell.selfStatusEffectAmount,
                     currentSpell.selfStatusEffectAmount >= defenseStatManager.GetMaximumStatusResistanceBeforeSufferingStatusEffect(
                         currentSpell.selfStatusEffect));*/
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

        public bool IsSpellCasting()
        {
            return animator.GetBool(hashIsSpellCasting);
        }

        public int GetCurrentInteligence()
        {
            return playerStatsDatabase.intelligence + statsBonusController.intelligenceBonus;
        }
    }

}
