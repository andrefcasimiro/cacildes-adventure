using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class EnemySightController : MonoBehaviour
    {
        Enemy enemy => GetComponent<Enemy>();

        [Header("Sight")]
        public SightCone sightCone;

        private ClimbController player;

        private void Start()
        {
            player = FindObjectOfType<ClimbController>(true);
        }

        public bool IsPlayerInSight()
        {
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
                    if (hitInfo.collider.gameObject.tag == "Player") { return true; }
                }
            }

            return false;
        }

    }

}
