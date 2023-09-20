using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class EnemySightController : MonoBehaviour
    {
        [Header("Sight")]
        public SightCone sightCone;
        public bool ignorePlayer = false;
        public UnityEvent onPlayerSight;

        EnemyManager enemyManager => GetComponent<EnemyManager>();

        PlayerComponentManager playerComponentManager;
        ClimbController playerClimbController;
        SceneSettings sceneSettings;

        private void Start()
        {
            playerComponentManager = enemyManager.player.GetComponent<PlayerComponentManager>();
            playerClimbController = enemyManager.player.GetComponent<ClimbController>();
            sceneSettings = FindAnyObjectByType<SceneSettings>(FindObjectsInactive.Include);
        }

        public bool IsPlayerInSight()
        {
            if (enemyManager.enemyBehaviorController.isAgressive == false)
            {
                return false;
            }

            if (enemyManager.enemyTargetController.ignoreCompanions == false && enemyManager.enemyTargetController.currentCompanion != null)
            {
                return false;
            }

            if (playerComponentManager.IsBusy())
            {
                return false;
            }

            if (ignorePlayer)
            {
                return false;
            }

            if (enemyManager.enemyHealthController.currentHealth <= 0)
            {
                return false;
            }

            if (enemyManager.enemySleepController != null && enemyManager.enemySleepController.isSleeping)
            {
                return false;
            }

            if (Vector3.Distance(enemyManager.player.transform.position, this.transform.position) <= enemyManager.agent.stoppingDistance)
            {
                return true;
            }

            if (sceneSettings.isColliseum)
            {
                return true;
            }

            if (Vector3.Distance(this.transform.position, enemyManager.player.transform.position) > enemyManager.maximumChaseDistance)
            {
                return false;
            }

            Vector3 enemyEyes = sightCone.transform.position;
            Vector3 playerEyes = playerClimbController.playerHeadRef.transform.position;

            if (sightCone.playerWithinRange)
            {
                RaycastHit hitInfo;

                if (Physics.Linecast(enemyEyes, playerEyes, out hitInfo))
                {
                    if (hitInfo.collider.gameObject.tag == "Player" || hitInfo.collider.gameObject.tag == "PlayerHealthbox")
                    {
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
