using System.Collections;
using UnityEngine;
using StarterAssets;

namespace AF
{

    public class PlayerComponentManager : MonoBehaviour
    {
        ThirdPersonController thirdPersonController => GetComponent<ThirdPersonController>();
        PlayerCombatController playerCombatController => GetComponent<PlayerCombatController>();
        DodgeController dodgeController => GetComponent<DodgeController>();
        PlayerParryManager playerParryManager => GetComponent<PlayerParryManager>();

        void Awake() {
            StartCoroutine(SpawnPlayer());
        }

        IEnumerator SpawnPlayer() {
            yield return null;
            TeleportManager.instance.SpawnPlayer(this.gameObject);
        }

        public void EnableComponents()
        {
            thirdPersonController.enabled = true;
            playerCombatController.enabled = true;
            dodgeController.enabled = true;
            playerParryManager.enabled = true;
        } 

        public void DisableComponents()
        {
            thirdPersonController.StopMovement();
            thirdPersonController.enabled = false;
            playerCombatController.enabled = false;
            dodgeController.enabled = false;
            playerParryManager.enabled = false;
        }

        public void DisableCharacterController()
        {
            GetComponent<CharacterController>().enabled = false;
        }

        public void EnableCharacterController()
        {
            GetComponent<CharacterController>().enabled = true;
        }


    }

}