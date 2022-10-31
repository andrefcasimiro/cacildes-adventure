using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

namespace AF
{
    public class ClimbController : MonoBehaviour
    {   
        public enum ClimbState
        {
            NONE,
            ENTERING,
            CLIMBING,
            EXITING,
        }

        // Climb start animations that will then transition to the climb idle state
        public readonly int hashStartFromTop = Animator.StringToHash("StartFromTop");
        public readonly int hashStartFromBottom = Animator.StringToHash("StartFromBottom");

        // Climb finish animations that will then transition to the idle state, which in turn resets our player to the idle state
        public readonly int hashExitToTop = Animator.StringToHash("ExitToTop");
        public readonly int hashExitToBottom = Animator.StringToHash("ExitToBottom");

        // Moving ladder animations
        public readonly string hashClimbAmount = "climbAmount";

        public ClimbState climbState = ClimbState.NONE;

        public Transform playerHeadRef;
        public Transform playerFeetRef;


        Animator animator => GetComponent<Animator>();
        EquipmentGraphicsHandler equipmentGraphicsHandler => GetComponent<EquipmentGraphicsHandler>();
        StarterAssetsInputs _input;

        private void Start()
        {
            _input = GetComponent<StarterAssetsInputs>();
        }

        public void StartFromBottom(Transform transformRef)
        {
            equipmentGraphicsHandler.HideWeapons();
            climbState = ClimbState.ENTERING;
            
            GetComponent<CharacterController>().enabled = false;
            this.transform.position = transformRef.position;
            this.transform.rotation = transformRef.rotation;

            animator.Play(hashStartFromBottom);
        }


        public void StartFromTop(Transform transformRef)
        {
            equipmentGraphicsHandler.HideWeapons();
            climbState = ClimbState.ENTERING;

            // Hacky. We are getting the rotation of the ladder itself because it works best for the entering ladder from top use-case
            this.transform.position = transformRef.position;
            this.transform.rotation = transformRef.GetComponentInParent<ClimbLadderTrigger>().transform.rotation;

            GetComponent<CharacterController>().enabled = false;
            animator.Play(hashStartFromTop);
            animator.applyRootMotion = true;
        }

        private void Update()
        {
            if (climbState != ClimbState.CLIMBING)
            {
                return;
            }

            Climb(_input.move.y);
        }

        public void Climb(float direction)
        {
            if (direction < 0)
            {
                direction = -1;
            }
            else if (direction > 0)
            {
                direction = 1;
            }

            animator.SetFloat(hashClimbAmount, direction);
        }


        public void ExitToTop()
        {

            climbState = ClimbState.EXITING;
            animator.CrossFade(hashExitToTop, 0.25f);
        }

        public void ExitToBottom()
        {
            climbState = ClimbState.EXITING;
            animator.CrossFade(hashExitToBottom, 0.25f);
        }

        /// <summary>
        ///  Animation Event
        /// </summary>
        public void BeginClimbing()
        {
            Physics.gravity = new Vector3(0, 0, 0);
            animator.applyRootMotion = true;
            GetComponent<CharacterController>().enabled = true;
            climbState = ClimbState.CLIMBING;
        }

        /// <summary>
        ///  Animation Event
        /// </summary>
        public void FinishClimbing()
        {
            Physics.gravity = new Vector3(0, -9.81f, 0);
            climbState = ClimbState.NONE;
            GetComponent<CharacterController>().enabled = true;
            animator.applyRootMotion = false;
            equipmentGraphicsHandler.ShowWeapons();
        }

    }

}
