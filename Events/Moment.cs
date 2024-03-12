using System.Collections;
using System.Collections.Generic;
using TigerForge;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Events
{
#if UNITY_EDITOR

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
#endif
    public class Moment : MonoBehaviour
    {
        [HideInInspector]
        private List<EventBase> events = new();

        [TextArea]
        public string comment = "Add children game objects with events. To execute, call Moment.Triger()";

        Coroutine TriggerEventsCoroutine;

        bool isRunning = false;

        [Header("Events")]
        public UnityEvent onMoment_Start;
        public UnityEvent onMoment_End;

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

            onMoment_Start?.Invoke();

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

            FinishEvent();
        }

        void FinishEvent()
        {
            onMoment_End?.Invoke();
            isRunning = false;
            EventManager.EmitEvent(EventMessages.ON_MOMENT_END);
        }

        private void OnDisable()
        {
            if (isRunning)
            {
                FinishEvent();
            }
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void CancelEvent()
        {
            if (TriggerEventsCoroutine != null)
            {
                StopCoroutine(TriggerEventsCoroutine);
                TriggerEventsCoroutine = null;
            }

            FinishEvent();
        }
    }
}
