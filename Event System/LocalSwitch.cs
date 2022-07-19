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

        public void UpdateLocalSwitchValue(LocalSwitchName nextLocalSwitchName)
        {
            this.localSwitchName = nextLocalSwitchName;

            // Update local switch manager database
            LocalSwitchManager.instance.UpdateLocalSwitchDatabaseEntry(this);

            Event ev = gameObject.GetComponent<Event>();
            if (ev != null)
            {
                ev.RefreshEventPages();
            }
        }
    }

}