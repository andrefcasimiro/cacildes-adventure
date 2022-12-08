using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{

    public class EnemySightController : MonoBehaviour
    {
        Enemy enemy => GetComponent<Enemy>();

        [Header("Sight")]
        public SightCone sightCone;

        private ClimbController player;
        private PlayerComponentManager playerComponentManager;

        EnemyHealthController enemyHealthController => GetComponent<EnemyHealthController>();

        public UnityEvent onPlayerSight;

        EnemySleepController enemySleepController => GetComponent<EnemySleepController>();

        public bool ignorePlayer = false;

        private void Awake()
        {
            player = FindObjectOfType<ClimbController>(true);

            playerComponentManager = player.GetComponent<PlayerComponentManager>();
        }

        public bool IsPlayerInSight()
        {
            if (playerComponentManager.IsBusy())
            {
                return false;
            }

            if (ignorePlayer)
            {
                return false;
            }

            if (enemyHealthController.currentHealth <= 0)
            {
                return false;
            }

            if (enemySleepController != null)
            {
                if (enemySleepController.isSleeping)
                {
                    return false;
                }
            }

            if (Vector3.Distance(player.transform.position, this.transform.position) <= enemy.agent.stoppingDistance)
            {
                return true;
            }

            if (Vector3.Distance(this.transform.position, player.transform.position) > sightCone.sightDistance)
            {
                return false;
            }

            Vector3 enemyEyes = sightCone.transform.position;
            Vector3 playerEyes = player.playerHeadRef.transform.position;
            
            if (sightCone.playerWithinRange)
            {
                Debug.DrawLine(enemyEyes, playerEyes, Color.blue);

                RaycastHit hitInfo;
                
                if (Physics.Linecast(enemyEyes, playerEyes, out hitInfo))
                {
                    if (hitInfo.collider.gameObject.tag == "Player") {
                        if (onPlayerSight != null)
                        {
                            onPlayerSight.Invoke();
                        }

                        return true;
                    }
                }
            }

            return false;
        }

    }

}
