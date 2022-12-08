using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{
    public class EnemySleepController : MonoBehaviour, IClockListener
    {
        public float sleepFrom = 22f;
        public float sleepUntil = 05f;

        Enemy enemy => GetComponent<Enemy>();
        EnemyCombatController enemyCombatController => GetComponent<EnemyCombatController>();
        EnemySightController enemySightController => GetComponent<EnemySightController>();

        public bool isSleeping = false;

        public GameObject bed;

        private void Start()
        {
            if (bed != null) { bed.gameObject.SetActive(false); }

            OnHourChanged();
        }

        public void OnHourChanged()
        {
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
                if (enemyCombatController.IsCombatting() == false && enemySightController.IsPlayerInSight() == false)
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

        private void Update()
        {
            if (isSleeping)
            {
                if (Vector3.Distance(this.transform.position, enemy.playerCombatController.transform.position) <= 1f)
                {
                    isSleeping = false;
                    WakeUp();
                }
            }
        }

        public void Sleep()
        {
            isSleeping = true;
            enemy.animator.Play("Sleeping");
            enemy.agent.isStopped = true;

            if (bed != null) { bed.gameObject.SetActive(true); }
        }

        public void WakeUp()
        {
            isSleeping = false;
            enemy.animator.SetBool("IsSleeping", false);
            enemy.agent.isStopped = false;
            if (bed != null) { bed.gameObject.SetActive(false); }

        }

    }

}
