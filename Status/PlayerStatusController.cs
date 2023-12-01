using UnityEngine;

namespace AF.StatusEffects
{
    // Change this to have a getter depending on AI or Player

    public class PlayerStatusController : StatusController
    {

        public StatusDatabase statusDatabase;

        // On loading scene, retrieve which statuses were applied
        private void Awake()
        {
            statusDatabase.LoadAppliedStatus(this);
        }

        // Before unloading the scene, remember which statuses were applied
        private void OnDestroy()
        {
            statusDatabase.appliedStatus.Clear();
            foreach (var appliedStatus in this.appliedStatusEffects)
            {
                statusDatabase.appliedStatus.Add(appliedStatus);
            }
        }
    }
}
