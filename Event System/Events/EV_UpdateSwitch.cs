using System.Collections;

namespace AF
{

    public class EV_UpdateSwitch : EventBase
    {
        public Switches targetSwitch;
        public bool newValue;

        public bool useFade;

        public override IEnumerator Dispatch()
        {
            yield return StartCoroutine(UpdateSwitch());
        }

        private IEnumerator UpdateSwitch()
        {
            if (useFade)
            {
                FindObjectOfType<FadeCanvas>(true).FadeIn(1f);
            }

            SwitchManager.instance.UpdateSwitch(targetSwitch, newValue);

            // On switch changes, reevaluate all events in the scene
            Event[] events = FindObjectsOfType<Event>(true);
            foreach (Event e in events)
            {
                e.RefreshEventPages();
            }

            yield return null;
        }

        private void OnDisable()
        {
            if (useFade)
            {
                FadeCanvas fadeCanvas = FindObjectOfType<FadeCanvas>(true);

                if (fadeCanvas != null)
                {
                    fadeCanvas.FadeIn(1f);
                }
            }
        }

    }

}