using System.Collections;
using UnityEngine;

namespace AF.Events
{
    public class EV_MoveTowards : EventBase
    {
        public CharacterManager characterManager;
        public bool shouldRun = false;
        public Transform targetDestination;

        public override IEnumerator Dispatch()
        {
            if (characterManager.agent.enabled)
            {
                characterManager.agent.speed = shouldRun ? characterManager.chaseSpeed : characterManager.patrolSpeed;
                characterManager.agent.destination = targetDestination.position;

                yield return new WaitUntil(() => characterManager.agent.remainingDistance <= characterManager.agent.stoppingDistance);

                characterManager.agent.speed = 0f;
            }
        }
    }
}
