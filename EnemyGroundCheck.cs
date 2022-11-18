using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class EnemyGroundCheck : MonoBehaviour
    {
        Enemy enemy => GetComponentInParent<Enemy>();
        EnemyHealthController enemyHealthController => GetComponentInParent<EnemyHealthController>();
        public LayerMask groundLayers;

        public float maxAgentCheckCooldown = 2f;
        public float agentCheckCooldown = Mathf.Infinity;
        public float multiplier = 5f;

        private void Update()
        {

            if (agentCheckCooldown < maxAgentCheckCooldown)
            {
                agentCheckCooldown += Time.deltaTime;
            }

            if (enemy.agent.enabled == false)
            {
                if (enemy.animator.GetBool("IsFalling") == false)
                {
                    if (!Physics.Raycast(transform.position, transform.up * -1, 2f))
                    {
                        enemy.animator.Play("Falling");
                    }
                }
            }
        }

        public void ApplyForce(Vector3 moveForce)
        {

            if (enemy.rigidbody != null)
            {
                enemy.rigidbody.constraints = RigidbodyConstraints.None;
                enemy.rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                enemy.agent.enabled = false;
                enemy.rigidbody.AddForce(moveForce * multiplier, ForceMode.Acceleration);
                agentCheckCooldown = 0f;

            }

        }

        private void OnTriggerStay(Collider other)
        {
            // Is grounded
            if ((groundLayers.value & (1 << other.transform.gameObject.layer)) > 0)
            {
                if (enemy.agent.enabled == false)
                {
                    if (agentCheckCooldown >= maxAgentCheckCooldown && enemyHealthController.currentHealth > 0)
                    {
                        enemy.agent.enabled = true;
                        enemy.animator.Play("Chasing");
                        enemy.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
                    }
                }
            }
        }
    }

}
