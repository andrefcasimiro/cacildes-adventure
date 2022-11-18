using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{

    public class Enemy : MonoBehaviour
    {
        public Animator animator;
        public NavMeshAgent agent;
        public Rigidbody rigidbody => GetComponent<Rigidbody>();

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

        EnemyCombatController enemyCombatController => GetComponent<EnemyCombatController>();
        EnemySightController enemySightController => GetComponent<EnemySightController>();

        EnemyHealthController enemyHealthController => GetComponent<EnemyHealthController>();

        [Header("Boss")]
        public bool isBoss = false;
        public string bossName = "";
        public GameObject fogWall;
        public AudioClip bossMusic;
        public string bossSwitchUuid;

        private void Awake()
        {
            playerCombatController = FindObjectOfType<PlayerCombatController>(true);
            player = playerCombatController.gameObject;

            if (fogWall != null)
            {
                fogWall.gameObject.SetActive(false);
            }
        }

        public void InitiateBossBattle()
        {
            enemyHealthController.ShowBossHud();
            fogWall.gameObject.SetActive(true);
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

        public void ForceCombat()
        {
            var lookRotation = player.transform.position - this.transform.position;
            var rotation = Quaternion.LookRotation(lookRotation);
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rotation, Time.deltaTime * rotationSpeed);

            animator.SetBool(hashChasing, true);
        }

        public bool InCombatWithPlayer()
        {
            return animator.GetBool(hashChasing) || enemyCombatController.IsCombatting();
        }
    }
}
