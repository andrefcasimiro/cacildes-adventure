using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class StalactiteCeilingManager : MonoBehaviour
    {
        public Cinemachine.CinemachineImpulseSource cinemachineImpulseSource;

        public AudioSource audioSource => GetComponent<AudioSource>();

        public float timeBeforeDroppingStalacites = .5f;

        PlayerCombatController playerCombatController;

        private void Awake()
        {
            playerCombatController = FindObjectOfType<PlayerCombatController>(true);
        }

        public void ShakeCamera()
        {

            // Rotate randomly
            var targetRot = Random.rotation;
            targetRot.x = 0;
            targetRot.z = 0;

            cinemachineImpulseSource.GenerateImpulse();

        }

        public void Launch()
        {
            cinemachineImpulseSource.GenerateImpulse();

            audioSource.Play();

            StartCoroutine(DropStalactites());
        }

        IEnumerator DropStalactites()
        {
            yield return new WaitForSeconds(timeBeforeDroppingStalacites);

            transform.position = new Vector3(playerCombatController.transform.position.x, this.transform.position.y, playerCombatController.transform.position.z);


            Stalactite[] stalactites = transform.GetComponentsInChildren<Stalactite>(true);

            foreach (Stalactite t in stalactites)
            {
                t.gameObject.SetActive(true);
                t.Drop();
            }
        }

    }
}
