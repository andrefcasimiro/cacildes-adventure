using System.Collections;
using System.Collections.Generic;
using AF;
using AF.Events;
using TigerForge;
using UnityEditor;
using UnityEngine;

namespace Events
{
    [CustomEditor(typeof(Moment), editorForChildClasses: true)]
    public class EventEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            Moment moment = target as Moment;

            if (GUILayout.Button("Raise"))
            {
                moment.Trigger();
            }
        }
    }
    public class Moment : MonoBehaviour
    {
        [HideInInspector]
        private List<EventBase> events = new();

        [TextArea]
        public string comment = "Add children game objects with events. To execute, call Moment.Triger()";

        Coroutine TriggerEventsCoroutine;

        bool isRunning = false;

        private void Awake()
        {
            CollectEventsFromChildren();
        }

        private void CollectEventsFromChildren()
        {
            events.Clear();

            foreach (Transform childTransform in transform)
            {
                foreach (EventBase eventBase in childTransform.GetComponents<EventBase>())
                {
                    events.Add(eventBase);
                }
            }
        }

        public void Trigger()
        {
            if (isRunning)
            {
                return;
            }

            // For testing purposes, acquire events on runtime if we are switching / reordering the events in the editor
            CollectEventsFromChildren();

            if (TriggerEventsCoroutine != null)
            {
                StopCoroutine(TriggerEventsCoroutine);
            }

            TriggerEventsCoroutine = StartCoroutine(TriggerEvents_Coroutine());
        }

        private IEnumerator TriggerEvents_Coroutine()
        {
            isRunning = true;

            EventManager.EmitEvent(EventMessages.ON_MOMENT_START);

            foreach (EventBase ev in events)
            {
                if (ev != null)
                {
                    yield return StartCoroutine(ev.Dispatch());
                }
            }

            isRunning = false;
        }

        public void StopEvent()
        {
            StartCoroutine(StopEvent_Coroutine());
        }

        IEnumerator StopEvent_Coroutine()
        {
            yield return new WaitForEndOfFrame();

            if (TriggerEventsCoroutine != null)
            {
                StopCoroutine(TriggerEventsCoroutine);
                TriggerEventsCoroutine = null;
            }

            isRunning = false;
        }
    }
}
