using System.Collections;
using System.Collections.Generic;
using AF;
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
        private List<EventBase> events = new List<EventBase>();

        [TextArea]
        public string comment = "Add children game objects with events. To execute, call Moment.Triger()";

        private void Awake()
        {
            CollectEventsFromChildren();
        }

        private void CollectEventsFromChildren()
        {
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
            StartCoroutine(TriggerEvents());
        }

        private IEnumerator TriggerEvents()
        {
            foreach (EventBase ev in events)
            {
                if (ev != null)
                {
                    yield return StartCoroutine(ev.Dispatch());
                }
            }
        }
    }
}
