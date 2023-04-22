using StarterAssets;
using System.Collections;
using UnityEngine;

namespace AF
{
    [RequireComponent(typeof(BoxCollider))]
    public class ClimbLadderTriggerV2 : MonoBehaviour, IEventNavigatorCapturable
    {
        public float ladderBottom;
        public float ladderTop;

        ClimbController player => FindObjectOfType<ClimbController>(true);

        StarterAssetsInputs inputs;
        UIDocumentKeyPrompt uIDocumentKeyPrompt;
        PlayerShootingManager playerShootingManager;

        [HideInInspector] public Transform forwardRef => GetComponentInChildren<Transform>();

        public float playerOffsetFromStairs = 2.65f;

        private void Awake()
        {
        }

        private void Start()
        {
            inputs = FindObjectOfType<StarterAssetsInputs>(true);
            uIDocumentKeyPrompt = FindObjectOfType<UIDocumentKeyPrompt>(true);
            playerShootingManager = FindObjectOfType<PlayerShootingManager>(true);
            
            ladderBottom = transform.position.y;
            ladderTop = transform.position.y + GetComponent<BoxCollider>().bounds.extents.y * 2;
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
            var dodgeController = other.GetComponent<DodgeController>();
            if (dodgeController != null && dodgeController.IsDodging())
            {
                return;
            }

            var playerCombatController = other.gameObject.GetComponent<PlayerCombatController>();
            if (playerCombatController != null && playerCombatController.isCombatting)
            {
                return;
            }

            if ( player.climbState != ClimbController.ClimbState.NONE)
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
            if (player.climbState == ClimbController.ClimbState.NONE && inputs.interact && player.currentLadder == null)
            {
                BeginClimbing();
            }
        }

        void BeginClimbing()
        {
            inputs.interact = false;

            uIDocumentKeyPrompt.gameObject.SetActive(false);

            float playerHeadY = player.playerHeadRef.transform.position.y;

            if (playerHeadY >= ladderBottom && playerHeadY <= ladderTop)
            {
                var topTransformPosition = transform.position + (transform.forward / playerOffsetFromStairs);
                topTransformPosition.y = ladderBottom;
                player.currentLadder = this;
                player.StartFromBottomV2(topTransformPosition);

            }
            else if (playerHeadY >= ladderTop)
            {
                player.currentLadder = this;
                var topTransformPosition = transform.position - (transform.forward / playerOffsetFromStairs);
                topTransformPosition.y = ladderTop;
                player.StartFromTopV2(topTransformPosition);
            }
        }

        private void Update()
        {
            if (player == null || player.climbState != ClimbController.ClimbState.CLIMBING)
            {
                return;
            }

            if (player.currentLadder != this)
            {
                return;
            }

            UpdateClimbing();
        }

        void UpdateClimbing()
        {
            float playerHeadY = player.playerHeadRef.transform.position.y;
            float playerFeetY = player.playerFeetRef.transform.position.y;

            if (playerHeadY > ladderTop)
            {
                player.ExitToTop();
            }
            else if (playerFeetY - ladderBottom < 0)
            {
                player.ExitToBottom();
            }
        }
    }
}
