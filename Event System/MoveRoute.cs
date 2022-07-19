using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

namespace AF
{
    public class MoveRoute : MonoBehaviour
    {
        public Animator animator;
        public NavMeshAgent navMeshAgent;

        [HideInInspector]
        public List<EventBase> events = new List<EventBase>();

        public bool isRunning = false;

        private EventBase currentEvent;

        private void Awake()
        {
            EventBase[] gatheredEvents = GetComponents<EventBase>();

            this.events.Clear();
            foreach (EventBase ev in gatheredEvents)
            {
                events.Add(ev);
            }
        }

        public IEnumerator DispatchEvents()
        {
            isRunning = true;

            foreach (EventBase ev in events)
            {
                if (ev != null)
                {
                    currentEvent = ev;
                    yield return StartCoroutine(ev.Dispatch());
                }
            }

            isRunning = false;
        }

        public void StartCycle()
        {
            navMeshAgent.isStopped = false;

            StartCoroutine(DispatchEvents());
        }

        public void Interrupt()
        {
            StopAllCoroutines();

            animator.Play("Idle");
            navMeshAgent.isStopped = true;
        }

        public void ResumeCycle()
        {
            navMeshAgent.isStopped = false;

            StartCoroutine(ResumeFromLastEvent());
        }

        public IEnumerator ResumeFromLastEvent()
        {
            var indexOfCurrentEvent = events.IndexOf(currentEvent);

            if (indexOfCurrentEvent != -1)
            {
                var nextIndex = indexOfCurrentEvent;

                if (nextIndex <= events.Count)
                {
                    EventBase eventToResume = events[indexOfCurrentEvent];

                    if (eventToResume != null)
                    {
                        // Create a copy
                        List<EventBase> remainingEvents = new List<EventBase>();
                        foreach (EventBase ev in events)
                        {
                            remainingEvents.Add(ev);
                        }

                        remainingEvents.RemoveRange(0, indexOfCurrentEvent);

                        foreach (EventBase ev in remainingEvents)
                        {
                            if (ev != null)
                            {
                                currentEvent = ev;
                                yield return StartCoroutine(ev.Dispatch());
                            }
                        }
                    }
                }
            }

            isRunning = false;
        }

    }
}
