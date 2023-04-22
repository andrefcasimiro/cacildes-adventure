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

        EnemyManager enemyManager => GetComponent<EnemyManager>();

        private void Start()
        {
            EvaluateIfAgressive();
        }

        public void EvaluateIfAgressive()
        {
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

            if (FactionManager.instance.IsFriendlyTowardsPlayer(faction))
            {
                TurnFriendly();
            }
            else
            {
                TurnAgressive();
            }

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
