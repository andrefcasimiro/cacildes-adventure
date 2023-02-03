using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class EnemySleepController : MonoBehaviour, IClockListener
    {
        [Header("Settings")]
        public bool canSleep = false;
        public float sleepFrom = 22f;
        public float sleepUntil = 05f;
        public bool isSleeping = false;
        public GameObject bed;

        EnemyManager enemyManager => GetComponent<EnemyManager>();

        // Start is called before the first frame update
        void Start()
        {
            InitializeSleep();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateSleep();
        }

        void InitializeSleep()
        {
            if (bed != null) { bed.gameObject.SetActive(false); }
        }

        void UpdateSleep()
        {
            if (isSleeping)
            {
                if (Vector3.Distance(this.transform.position, enemyManager.player.transform.position) <= 1f)
                {
                    isSleeping = false;
                    WakeUp();
                }
            }
        }

        public void OnHourChanged()
        {
            if (canSleep == false)
            {
                return;
            }

            bool shouldSleep = false;

            // If appear until is after midnight, it may become smaller than appearFrom (i. e. appear from 17 until 4)
            if (sleepFrom > sleepUntil)
            {
                shouldSleep = Player.instance.timeOfDay >= sleepFrom && Player.instance.timeOfDay <= 24 || (Player.instance.timeOfDay >= 0 && Player.instance.timeOfDay <= sleepUntil);
            }
            else
            {
                shouldSleep = Player.instance.timeOfDay >= sleepFrom && Player.instance.timeOfDay <= sleepUntil;
            }

            if (shouldSleep)
            {
                if (enemyManager.enemyCombatController.IsInCombat() == false)
                {
                    // Sleep
                    Sleep();
                }
            }
            else
            {
                if (isSleeping)
                {
                    WakeUp();
                }
            }
        }


        public void Sleep()
        {
            isSleeping = true;
            enemyManager.animator.CrossFade(enemyManager.hashSleeping, 0.1f);
            enemyManager.agent.isStopped = true;

            if (bed != null)
            {
                bed.SetActive(true);
            }
        }

        public void WakeUp()
        {
            isSleeping = false;
            enemyManager.animator.SetBool(enemyManager.hashIsSleeping, false);
            enemyManager.agent.isStopped = false;
            
            if (bed != null)
            {
                bed.SetActive(false);
            }
        }

    }
}
