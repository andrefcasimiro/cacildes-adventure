using UnityEngine;


namespace AF
{

    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Player))]
    public class Climber : MonoBehaviour
    {
        Player player => GetComponent<Player>();
        Animator animator => GetComponent<Animator>();

        // Ladder
        public readonly int hashClimbing = Animator.StringToHash("Climbing");


        public readonly int hashBeginAscending = Animator.StringToHash("Begin Ascending");
        public readonly int hashBeginDescending = Animator.StringToHash("Begin Descending");
        public readonly int hashFinishAscending = Animator.StringToHash("Finish Ascending");
        public readonly int hashFinishDescending = Animator.StringToHash("Finish Descending");

        public readonly int hashLadderUp = Animator.StringToHash("Ladder Up");
        public readonly int hashLadderDown = Animator.StringToHash("Ladder Down");


        // Ladder Movement Actions
        public readonly int hashClimbingUp = Animator.StringToHash("climbingUp");
        public readonly int hashClimbingDown = Animator.StringToHash("climbingDown");

        private void Update()
        {
            if (!IsClimbing())
            {
                return;
            }

            HandleClimbing();
        }

        public void BeginClimbing(bool isAscending)
        {
            if (isAscending)
            {
                animator.Play(hashBeginAscending);
            }
            else
            {
                animator.Play(hashBeginDescending);
            }

        }


        public void FinishClimbing(bool isAscending)
        {

            if (isAscending)
            {
                animator.Play(hashFinishDescending);
            }
            else
            {
                animator.Play(hashFinishAscending);
            }
        }

        void HandleClimbing()
        {
            Vector3 targetVector = player.GetMoveDirection();


            animator.SetBool(hashClimbingDown, targetVector.z > 0);
            animator.SetBool(hashClimbingUp, targetVector.z < 0);
        }


        public bool IsClimbing()
        {
            return animator.GetBool(hashClimbing);
        }

    }

}