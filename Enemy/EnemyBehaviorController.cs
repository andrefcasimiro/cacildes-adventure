using UnityEngine;

namespace AF
{
    public class EnemyBehaviorController : MonoBehaviour
    {
        [Header("Behaviour")]
        public bool isAgressive = true;
        public bool becomeAgressiveOnAttacked = true;

        [Header("Agressive By Switch")]
        public SwitchEntry switchToBecomeAgressive;
        public bool switchUuidValueToBecomeAgressive = true;

        [Header("Agressive By Faction")]
        public FactionName faction = FactionName.NONE;
        public bool ignoreFactionAndJustCheckForEquippedArmor = false;

        EnemyManager enemyManager => GetComponent<EnemyManager>();

        [Header("Player Apparel")]
        public Helmet disguisedHelmet;
        public Armor disguisedArmor;
        public Gauntlet disguisedGauntlets;
        public Legwear disguisedLegwear;
        public bool requireOnlyArmor = false;

        private void Start()
        {
            EvaluateIfAgressive();
        }

        public void EvaluateIfAgressive()
        {
            if (ignoreFactionAndJustCheckForEquippedArmor)
            {
                bool isFriendly = IsPlayerDisguisedAsFaction();
                if (isFriendly)
                {
                    TurnFriendly();
                }
                else
                {
                    TurnAgressive();
                }
                return;
            }

            // If doesn't belong to faction, just use the default value of isAgressive
            if (faction == FactionName.NONE)
            {
                return;
            }

            if (switchToBecomeAgressive != null)
            {
                var shouldBecomeAgressive = SwitchManager.instance.GetSwitchCurrentValue(switchToBecomeAgressive) == switchUuidValueToBecomeAgressive;
                if (shouldBecomeAgressive)
                {
                    TurnAgressive();
                }
            }

            if (FactionManager.instance.IsFriendlyTowardsPlayer(faction) || IsPlayerDisguisedAsFaction())
            {
                TurnFriendly();
            }
            else
            {
                TurnAgressive();
            }
        }

        bool IsPlayerDisguisedAsFaction()
        {
            return false;
            /*// If `requireOnlyArmor` is true, check only the equipped armor.
            if (requireOnlyArmor)
            {
                return Player.instance.equippedArmor != null && Player.instance.equippedArmor.name.GetEnglishText() == disguisedArmor.name.GetEnglishText();
            }

            // Check if any disguise items are missing.
            if (disguisedHelmet == null || disguisedArmor == null || disguisedGauntlets == null || disguisedLegwear == null)
            {
                return false;
            }

            // Check if any equipment slot is empty.
            if (Player.instance.equippedHelmet == null || Player.instance.equippedArmor == null || Player.instance.equippedGauntlets == null || Player.instance.equippedLegwear == null)
            {
                return false;
            }

            // Check if all equipment pieces match the disguise.
            return Player.instance.equippedHelmet.name.GetEnglishText() == disguisedHelmet.name.GetEnglishText()
                && Player.instance.equippedGauntlets.name.GetEnglishText() == disguisedGauntlets.name.GetEnglishText()
                && Player.instance.equippedArmor.name.GetEnglishText() == disguisedArmor.name.GetEnglishText()
                && Player.instance.equippedLegwear.name.GetEnglishText() == disguisedLegwear.name.GetEnglishText();*/
        }
        void TurnFriendly()
        {
            isAgressive = false;
        }

        public void TurnAgressive()
        {
            isAgressive = true;

            if (faction != FactionName.NONE && FactionManager.instance.IsFriendlyTowardsPlayer(faction))
            {
                FactionManager.instance.SetFactionAffinity(faction, -1);
            }

            enemyManager.enemyHealthController.ShowHUD();
        }

        public void ChasePlayer()
        {
            enemyManager.animator.SetBool(enemyManager.hashChasing, true);
        }
    }
}
