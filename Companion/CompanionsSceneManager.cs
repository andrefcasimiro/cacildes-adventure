using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{
    public class CompanionsSceneManager : MonoBehaviour, ISaveable
    {
        NotificationManager notificationManager;

        private void Awake()
        {
            notificationManager = FindObjectOfType<NotificationManager>(true);
        }

        private void Start()
        {
            EvaluateCompanionsInScene();
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
            notificationManager.ShowNotification(companion.character.name + " " + LocalizedTerms.HasJoinedTheParty(), companion.character.avatar);

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
            notificationManager.ShowNotification(companion.character.name + " " + LocalizedTerms.HasLeftTheParty(), companion.character.avatar);

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

        public void OnGameLoaded(GameData gameData)
        {
            EvaluateCompanionsInScene();
        }

        void EvaluateCompanionsInScene()
        {
            var allCompanions = FindObjectsOfType<CompanionManager>(true);

            foreach (var companion in allCompanions)
            {
                companion.gameObject.SetActive(false);

                var companionInstance = Player.instance.companions.FirstOrDefault(c => c.companionId == companion.companion.companionId);
                bool companionIsInParty = companionInstance != null;

                if (companionIsInParty)
                {
                    companion.inParty = true;

                    if (companionInstance.isWaitingForPlayer)
                    {
                        companion.waitingForPlayer = true;

                        if (companionInstance.waitingForPlayerSceneIndex == SceneManager.GetActiveScene().buildIndex)
                        {
                            companion.transform.position = companionInstance.waitingForPlayerPosition;
                            companion.gameObject.SetActive(true);
                        }
                        else
                        {
                            // Dont show companion, he is on another scene
                            companion.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        companion.gameObject.SetActive(true);
                        companion.SpawnNearPlayer();
                    }
                }
                else
                {
                    companion.inParty = false;

                    // Companion is not on the party, only show him in map if scene is his home
                    companion.gameObject.SetActive(companion.sceneIsHome);
                }


            }
        }
    }

}
