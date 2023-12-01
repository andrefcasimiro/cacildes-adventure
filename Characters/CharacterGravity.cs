using UnityEngine;

namespace AF
{
    public class CharacterGravity : MonoBehaviour
    {
        public CharacterManager characterManager;

        private void Update()
        {
            if (characterManager.characterController.isGrounded && characterManager.agent.enabled)
            {
                return;
            }

            if (CheckEnemyGrounded())
            {
                characterManager.agent.Warp(characterManager.transform.position);
                characterManager.agent.enabled = true;
                return;
            }

            characterManager.characterController.Move(new Vector3(0.0f, -9f, 0.0f) * Time.deltaTime);
        }

        bool CheckEnemyGrounded()
        {
            // Cast a ray downward from the player's position
            Ray groundRay = new Ray(characterManager.transform.position, Vector3.down);

            // Set the maximum distance the ray can travel
            float maxDistance = 1f;

            // Perform the raycast and check if it hits something
            if (Physics.Raycast(groundRay, out RaycastHit hit, maxDistance))
            {
                return true; // The enemy is grounded
            }

            return false; // The enemy is not grounded
        }
    }

}