using System.Collections.Generic;

namespace AF
{
    public enum LocalSwitchName
    {
        NONE,
        A,
        B,
        C,
        D,
        E,
        F,
        G,
    }

    [System.Serializable]
    public class LocalSwitch : MonoBehaviourID
    {
        public LocalSwitchName localSwitchName;

        List<Event> subscribedEvents = new List<Event>();

        private void Start()
        {
            Event eventInPage = GetComponent<Event>();
            Event[] childEvents = GetComponentsInChildren<Event>(true);

            if (eventInPage != null)
            {
                subscribedEvents.Add(eventInPage);
            }

            foreach (Event childEvent in childEvents)
            {
                subscribedEvents.Add(childEvent);
            }

            // Check if other events are subscribed to this local switch
            Event[] eventsInScene = FindObjectsOfType<Event>(true);
            foreach (var ev in eventsInScene)
            {
                if (ev.localSwitch != null && ev.localSwitch.ID == this.ID)
                {
                    subscribedEvents.Add(ev);
                }
            }

        }

        public void UpdateLocalSwitchValue(LocalSwitchName nextLocalSwitchName)
        {
            this.localSwitchName = nextLocalSwitchName;

            // Update local switch manager database
            LocalSwitchManager.instance.UpdateLocalSwitchDatabaseEntry(this);

            foreach (Event subscribedEvent in subscribedEvents)
            {
                if (subscribedEvent != null)
                {
                    subscribedEvent.RefreshEventPages();
                }
            }
        }
    }

}