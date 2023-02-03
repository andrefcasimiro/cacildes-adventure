using UnityEngine;

namespace AF
{
    public class EnemyBehaviorController : MonoBehaviour
    {

        [Header("Behaviour")]
        public bool isAgressive = true;
        public bool becomeAgressiveOnAttacked = true;

        [Header("Agressive By Switch")]
        public string switchUuidToBecomeAgressive = "";
        public bool switchUuidValueToBecomeAgressive = true;

        [Header("Agressive By Faction")]
        public Faction enemyFaction;

        EnemyManager enemyManager => GetComponent<EnemyManager>();

        private void Start()
        {
            EvaluateIfAgressive();
        }

        void EvaluateIfAgressive()
        {
            if (isAgressive)
            {
                return;
            }

            if (System.String.IsNullOrEmpty(switchUuidToBecomeAgressive) == false)
            {
                var shouldBecomeAgressive = SwitchManager.instance.GetSwitchValue(switchUuidToBecomeAgressive) == switchUuidValueToBecomeAgressive;
                if (shouldBecomeAgressive)
                {
                    TurnAgressive();
                }
            }
        }

        public void TurnAgressive()
        {
            isAgressive = true;

            enemyManager.enemyHealthController.healthBarSlider.gameObject.SetActive(true);

            /*var eventKeyPrompt = FindObjectOfType<UIDocumentEventKeyPrompt>(true);
            if (eventKeyPrompt.eventUuid == this.gameObject.name)
            {
                eventKeyPrompt.gameObject.SetActive(false);
            }*/
        }

    }

}
