using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{
        public class EnemyPhysicsController : MonoBehaviour
    {
        public Rigidbody rigidbody => GetComponent<Rigidbody>();
        public NavMeshAgent agent => GetComponent<NavMeshAgent>();

        public bool tick = true;

        // Start is called before the first frame update
        void Start()
            {

            }

            // Update is called once per frame
            void Update()
            {

            }

            public void AllowPhysics()
            {
                if (rigidbody != null)
                {
                    rigidbody.isKinematic = false;
                    rigidbody.useGravity = true;
                }

                agent.enabled = false;
            }

            public void DisablePhysics()
            {
                if (rigidbody != null)
                {
                    rigidbody.isKinematic = true;
                    rigidbody.useGravity = false;
                }

                agent.enabled = true;
            }

        }
    }