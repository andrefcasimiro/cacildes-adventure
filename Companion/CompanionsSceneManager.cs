using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{
    public class CompanionsSceneManager : MonoBehaviour
    {
        NotificationManager notificationManager;

        private void Awake()
        {
            notificationManager = FindObjectOfType<NotificationManager>(true);
        }

        private void Start()
        {
            var companionsInParty = Player.instance.companions;

            var allCompanions = FindObjectsOfType<CompanionManager>(true);

            foreach (var companion in allCompanions)
            {
                var companionInstance = companionsInParty.FirstOrDefault(c => c.companionId == companion.companion.companionId);

                if (companionInstance != null)
                {
                    if (companionInstance.isWaitingForPlayer)
                    {
                        if (companionInstance.waitingForPlayerSceneIndex == SceneManager.GetActiveScene().buildIndex)
                        {
                            companion.waitingForPlayer = true;
                            companion.transform.position = companionInstance.waitingForPlayerPosition;
                            companion.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        companion.gameObject.SetActive(true);
                    }
                }


            }
        }

        public void AddCompanionToParty(Companion companion)
        {
            if (Player.instance.companions.FirstOrDefault(x => x.companionId == companion.companionId) != null)
            {
                return;
            }

            SerializedCompanion serializedCompanion = new SerializedCompanion();
            serializedCompanion.companionId = companion.companionId;
            serializedCompanion.isWaitingForPlayer = false;
            Player.instance.companions.Add(serializedCompanion);

            var allCompanionsInScene = FindObjectsOfType<CompanionManager>(true);
            allCompanionsInScene.FirstOrDefault(x => x.companion == companion).inParty = true;

            Soundbank.instance.PlayCompanionJoinParty();
            notificationManager.ShowNotification(companion.character.name + " has joined the party!", companion.character.avatar);

            // Revaluate any objects in scene that depend on party elements
            var partyDependantObjects = FindObjectsOfType<PartyDependant>(true);
            foreach (var partyDependant in partyDependantObjects)
            {
                partyDependant.Reevaluate();
            }
        }

        public void DismissCompanion(Companion companion)
        {
            var companionInstance = Player.instance.companions.FirstOrDefault(x => x.companionId == companion.companionId);
            if (companionInstance == null)
            {
                return;
            }

            var allCompanionsInMap = FindObjectsOfType<CompanionManager>(true);
            allCompanionsInMap.FirstOrDefault(x => x.companion == companion).inParty = false;

            Player.instance.companions.Remove(companionInstance);

            Soundbank.instance.PlayCompanionLeaveParty();
            notificationManager.ShowNotification(companion.character.name + " has left the party!", companion.character.avatar);

            // Revaluate any objects in scene that depend on party elements
            var partyDependantObjects= FindObjectsOfType<PartyDependant>(true);
            foreach ( var partyDependant in partyDependantObjects)
            {
                partyDependant.Reevaluate();
            }
        }

        public void MarkCompanionAsWaiting(Companion companion)
        {
            var companionInstance = Player.instance.companions.FindIndex(x => x.companionId == companion.companionId);
            if (companionInstance == -1)
            {
                return;
            }

            var allCompanionsInMap = FindObjectsOfType<CompanionManager>(true);
            var companionInMap = allCompanionsInMap.FirstOrDefault(x => x.companion == companion);
            companionInMap.waitingForPlayer = true;

            Player.instance.companions[companionInstance].waitingForPlayerPosition = companionInMap.transform.position;
            Player.instance.companions[companionInstance].waitingForPlayerSceneIndex = SceneManager.GetActiveScene().buildIndex;
            Player.instance.companions[companionInstance].isWaitingForPlayer = true;
        }

        public void UnmarkCompanionAsWaiting(Companion companion)
        {
            var companionInstance = Player.instance.companions.FindIndex(x => x.companionId == companion.companionId);
            if (companionInstance == -1)
            {
                return;
            }

            var allCompanionsInMap = FindObjectsOfType<CompanionManager>(true);
            var companionInMap = allCompanionsInMap.FirstOrDefault(x => x.companion == companion);
            companionInMap.waitingForPlayer = false;

            Player.instance.companions[companionInstance].waitingForPlayerPosition = Vector3.zero;
            Player.instance.companions[companionInstance].waitingForPlayerSceneIndex = -1;
            Player.instance.companions[companionInstance].isWaitingForPlayer = false;
        }

    }

}
