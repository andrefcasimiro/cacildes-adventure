using UnityEngine;
using UnityEngine.AI;

namespace AF
{

    [RequireComponent(typeof(NavMeshAgent))]
    public class Enemy : Character
    {

        public readonly int hashIdle = Animator.StringToHash("Idle");
        public readonly int hashPatrol = Animator.StringToHash("Patrolling");
        public readonly int hashChasing = Animator.StringToHash("Chasing");

        // Combat
        public readonly int hashCombatting = Animator.StringToHash("Combatting");
        public readonly int hashAttacking1 = Animator.StringToHash("Attacking1");
        public readonly int hashAttacking2 = Animator.StringToHash("Attacking2");
        public readonly int hashAttacking3 = Animator.StringToHash("Attacking3");
        public readonly int hashBlocking = Animator.StringToHash("Blocking");
        public readonly int hashWaiting = Animator.StringToHash("Waiting");

        [Header("Patrolling")]
        public Transform[] waypoints;
        public float restingTimeOnWaypoint = 2f;
        int destinationPoint = 0;

        [Header("Chasing")]
        public float maximumChaseDistance = 10f;

        [Header("Sight")]
        public LayerMask obstructionMask;

        [Header("Block Settings")]
        public float minBlockChance = 0f;
        public float maxBlockChance = 1f;
        public float blockChance = 0.5f;

        [Header("Dodge Settings")]
        public float minDodgeChange = 0f;
        public float maxDodgeChance = 1f;
        public float dodgeChance = 0.5f;

        // Components
        Animator animator => GetComponent<Animator>();
        [HideInInspector] public NavMeshAgent agent => GetComponent<NavMeshAgent>();

        // References
        [HideInInspector] public Player player;

        private void Start()
        {
            player = GameObject.FindWithTag("Player").GetComponent<Player>();
        }

        public void GotoNextPoint()
        {
            agent.destination = waypoints[destinationPoint].position;
            destinationPoint = (destinationPoint + 1) % waypoints.Length;
        }

    }

}
