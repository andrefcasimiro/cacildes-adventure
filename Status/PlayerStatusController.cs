using UnityEngine;

namespace AF.StatusEffects
{

    public class PlayerStatusController : StatusController
    {

        public TeleportManager teleportManager;
        public StatusDatabase statusDatabase;

        private void Start()
        {
            foreach (var appliedStatus in statusDatabase.appliedStatus)
            {
                InflictStatusEffect(appliedStatus.statusEffect, appliedStatus.currentAmount, appliedStatus.hasReachedTotalAmount);
            }

            statusDatabase.appliedStatus.Clear();
        }

        private void OnEnable()
        {
            teleportManager.onChangingScene += SaveAppliedStatuses;
        }

        private void OnDisable()
        {
            teleportManager.onChangingScene -= SaveAppliedStatuses;
        }

        private void SaveAppliedStatuses()
        {
            statusDatabase.appliedStatus.Clear();
            foreach (var appliedStatus in this.appliedStatusEffects)
            {
                statusDatabase.appliedStatus.Add(appliedStatus);
            }
        }
    }
}
