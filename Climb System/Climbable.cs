using UnityEngine;

namespace AF
{

    [RequireComponent(typeof(BoxCollider))]
    public class Climbable : MonoBehaviour
    {
        public bool isAscending;

        public Transform rootMotionGoalTransform;

        Player player;

        private void Awake()
        {
            player = GameObject.FindWithTag("Player").GetComponent<Player>();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject != player.gameObject)
            {
                return;
            }

            //if (player.climber.IsClimbing())
            //{
            //    player.climber.FinishClimbing(isAscending);
            //    return;
            //}


            other.gameObject.transform.position = Vector3.Lerp(other.transform.position, transform.position, Time.deltaTime * 20f);

            var lookPos = transform.position - other.transform.position;
            lookPos.y = 0;
            var targetRot = Quaternion.LookRotation(lookPos);
            other.transform.rotation = Quaternion.Slerp(other.transform.rotation, targetRot, Time.deltaTime * 20f);

            //player.climber.BeginClimbing(isAscending);
        }
    }

}