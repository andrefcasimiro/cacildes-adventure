using UnityEngine;
using StarterAssets;

namespace AF
{
    public class ClimbLadderTrigger : MonoBehaviour, IEventNavigatorCapturable
    {
        [TextArea]
        public string comment = "LADDERS DONT WORK WITH NEGATIVE Y";

        public enum ClimbDirection
        {
            FROM_BOTTOM,
            FROM_TOP,
        }

        [Header("Start Climb Transforms")]
        public Transform startFromBottomRef;
        public Transform startFromTopRef;

        [Header("Finish Climb Transforms")]
        public Transform finishFromBottomRef;
        public Transform finishFromTopRef;

        ClimbController climbController;

        StarterAssetsInputs inputs;
        UIDocumentKeyPrompt uIDocumentKeyPrompt;
        PlayerShootingManager playerShootingManager;

        private void Start()
        {
            inputs = FindObjectOfType<StarterAssetsInputs>(true);
            uIDocumentKeyPrompt = FindObjectOfType<UIDocumentKeyPrompt>(true);
            playerShootingManager = FindObjectOfType<PlayerShootingManager>(true);
        }

        private void OnTriggerEnter(Collider other)
        {
            HandleCapture(other.gameObject);
        }

        public void OnCaptured()
        {
            HandleCapture(FindObjectOfType<PlayerCombatController>().gameObject);
        }

        void HandleCapture(GameObject other)
        {
            var dodgeController = other.gameObject.GetComponent<DodgeController>();
            if (dodgeController != null && dodgeController.IsDodging())
            {
                return;
            }

            var playerCombatController = other.gameObject.GetComponent<PlayerCombatController>();
            if (playerCombatController != null && playerCombatController.isCombatting)
            {
                return;
            }

            climbController = other.gameObject.GetComponent<ClimbController>();
            if (climbController.climbState != ClimbController.ClimbState.NONE)
            {
                return;
            }

            if (playerShootingManager.IsShooting())
            {
                return;
            }

            uIDocumentKeyPrompt.key = "E";
            uIDocumentKeyPrompt.action = LocalizedTerms.UseLadder();
            uIDocumentKeyPrompt.gameObject.SetActive(true);
        }

        public void OnInvoked()
        {
            var other = FindObjectOfType<PlayerCombatController>().gameObject;
            HandleClimbing(other);
        }

        private void OnTriggerStay(Collider other)
        {
            HandleClimbing(other.gameObject);
        }

        void HandleClimbing(GameObject other)
        {

            climbController = other.gameObject.GetComponent<ClimbController>();
            float ladderBottomY;
            float ladderTopY;

            if (climbController.climbState == ClimbController.ClimbState.NONE)
            {
                if (inputs.interact)
                {
                    uIDocumentKeyPrompt.gameObject.SetActive(false);

                    float playerFeetY = climbController.playerFeetRef.transform.position.y;
                    ladderBottomY = startFromBottomRef.position.y;
                    ladderTopY = startFromTopRef.position.y;

                    if (playerFeetY >= ladderBottomY && playerFeetY <= ladderTopY)
                    {
                        climbController.StartFromBottom(startFromBottomRef);
                    }
                    else if (playerFeetY >= ladderTopY)
                    {
                        climbController.StartFromTop(startFromTopRef);
                    }
                }
            }
            else if (climbController.climbState == ClimbController.ClimbState.CLIMBING)
            {
                float playerHeadY = climbController.playerHeadRef.transform.position.y;
                float playerFeetY = climbController.playerFeetRef.transform.position.y;

                if (playerHeadY >= finishFromTopRef.transform.position.y)
                {
                    climbController.ExitToTop();
                }
                else if (playerFeetY <= finishFromBottomRef.transform.position.y)
                {
                    climbController.ExitToBottom();
                }
            }
        }
    }
}
