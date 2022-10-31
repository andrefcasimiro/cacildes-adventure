using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.Events;

namespace AF
{

    public class Enemy : MonoBehaviour
    {
        public Animator animator;
        public NavMeshAgent agent;

        public readonly int hashIdle = Animator.StringToHash("Idle");
        public readonly int hashChasing = Animator.StringToHash("Chasing");

        [Header("General Settings")]
        public float rotationSpeed = 5f;

        [Header("Chasing")]
        public float maximumChaseDistance = 10f;

        public int goldReceived = 0;

        // Player Refs
        [HideInInspector] public PlayerCombatController playerCombatController;
        private GameObject player;

        // Flags
        [HideInInspector] public bool facePlayer = false;

        private void Awake()
        {
            playerCombatController = FindObjectOfType<PlayerCombatController>(true);
            player = playerCombatController.gameObject;
        }

        private void Update()
        {
            if (facePlayer)
            {
                var lookRotation = player.transform.position - this.transform.position;
                var rotation = Quaternion.LookRotation(lookRotation);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
}
