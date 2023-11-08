using UnityEngine;

namespace AF
{
    public class FollowPlayerUtil : MonoBehaviour
    {
        public float speed = 3f;

        PlayerCombatController playerCombatController;

        private void Start()
        {
            playerCombatController =FindAnyObjectByType<PlayerCombatController>(FindObjectsInactive.Include);  
        }

        private void Update()
        {
            if (playerCombatController != null)
            {
                // Calculate the direction from the particle system to the player
                Vector3 direction = playerCombatController.transform.position - transform.position;
                direction.Normalize(); // Normalize the direction vector to make it a unit vector

                // Move the particle system in the direction of the player
                transform.position += direction * speed * Time.deltaTime;
            }
        }
    }

}
