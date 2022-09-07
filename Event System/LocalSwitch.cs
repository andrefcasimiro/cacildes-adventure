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