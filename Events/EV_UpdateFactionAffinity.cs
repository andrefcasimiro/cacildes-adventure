using System.Collections;

namespace AF
{

    public class EV_UpdateFactionAffinity : EventBase
    {
        public FactionName factionName;

        public bool increase = false;
        public bool decrease = false;
        public int amount = 0;


        public override IEnumerator Dispatch()
        {
            UpdateFactionAffinity();
            yield return null;
        }

        public void UpdateFactionAffinity()
        {

            var currentFactionAffinity = FactionManager.instance.factionEntriesDictionary[factionName.ToString()].currentPlayerAffinityWithFaction;

            var newValue = currentFactionAffinity;

            if (increase)
            {
                newValue += amount;
            }
            else if (decrease)
            {
                newValue -= amount;
            }

            FactionManager.instance.SetFactionAffinity(factionName, newValue);

        }

    }

}
