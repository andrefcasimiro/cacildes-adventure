using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AF.Music;
using Unity.AI.Navigation;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace AF
{
    public class RoundManager : MonoBehaviour
    {
        [Header("Enemies")]
        public List<CharacterManager> easyEnemies;
        public List<CharacterManager> mediumEnemies;
        public List<CharacterManager> hardEnemies;
        public List<CharacterManager> easyBosses;
        public List<CharacterManager> mediumBosses;
        public List<CharacterManager> hardBosses;

        [System.Serializable]
        public class SpecialRound
        {
            public int round = 10;
            public List<CharacterManager> specialRoundEnemies;
            public int numberOfEnemiesToSpawn = 0;
        }

        public SpecialRound[] specialRounds;

        public Transform[] spawnRefs;

        public BGMManager bgmManager;
        public Soundbank soundbank;

        public int currentRound = 1;
        public int recordForBestRound = -1;
        public float recordForBestElapsedTime;

        UIDocument uiDocument => GetComponent<UIDocument>();
        VisualElement root;

        private bool isTiming = false;
        public float elapsedTime = 0f;

        Label round;
        Label counter;
        Label bestRound;

        public GenericTrigger nextRoundTrigger;
        public string sceneToReturnWhenExitingTournament;

        public UnityEvent onDeathEvent;

        [Header("On Round Win")]
        public AudioClip roundWinApplause;

        PlayerInventory playerInventory;
        PlayerComponentManager playerComponentManager;
        NotificationManager notificationManager;

        [System.Serializable]
        public class Stage
        {
            public GameObject stageGameObject;
            public AudioClip[] musics;
        }
        [Header("Stages")]
        public Stage[] stages;
        Stage currentStage;

        [Header("Powerups")]
        // public ArenaPowerup[] arenaPowerups;


        [Header("FX")]
        public DestroyableParticle puffFx;

        [Header("Achievements")]
        public Achievement achievement;

        [Header("Components")]
        public TeleportManager teleportManager;

        private void OnEnable()
        {
            root = uiDocument.rootVisualElement;
            root.style.display = DisplayStyle.None;
            round = root.Q<Label>("Round");
            counter = root.Q<Label>("Counter");
            bestRound = root.Q<Label>("BestRound");
            bestRound.style.display = DisplayStyle.None;

            if (playerInventory == null)
            {
                playerInventory = FindAnyObjectByType<PlayerInventory>(FindObjectsInactive.Include);
                playerComponentManager = playerInventory.GetComponent<PlayerComponentManager>();
            }

            if (notificationManager == null)
            {
                notificationManager = FindAnyObjectByType<NotificationManager>(FindObjectsInactive.Include);
            }
        }

        private void Update()
        {
            if (isTiming)
            {
                elapsedTime += Time.deltaTime;
                UpdateTimerUI();
            }
        }

        public void BeginRound()
        {
            currentRound++;

            round.text = $"Round {currentRound}";
            ResumeTimer();

            SpawnEnemiesForRound();

            root.style.display = DisplayStyle.Flex;
        }

        IEnumerator PlayApplause()
        {
            yield return new WaitForSeconds(0.15f);

            bgmManager.PlaySound(roundWinApplause, null);
        }

        public void EndRound()
        {
            if (currentRound == 15)
            {
                achievement.AwardAchievement();
            }

            bgmManager.StopMusicImmediately();

            StopTimer();

            ClearEnemiesFromRound();

            nextRoundTrigger.gameObject.SetActive(true);

            if (Random.Range(0, 1f) >= 0.5f)
            {
                StartCoroutine(PlayApplause());
            }

            if (currentRound % 5 == 0 && playerInventory != null)
            {

                playerInventory.ReplenishItems();

                soundbank.PlaySound(soundbank.uiItemReceived);
                notificationManager.ShowNotification("The crowd voted, your items have been replenished. Good luck!", null);
            }
            else if (currentRound % 3 == 0 && playerInventory != null && Random.Range(0, 100f) > 50f)
            {


                soundbank.PlaySound(soundbank.uiItemReceived);
                notificationManager.ShowNotification("The crowd voted, your health was restored. They wish to see you fight!"
                    , null);
            }
        }

        bool isEndingTournament = false;
        public void CallEndTournament()
        {
            if (isEndingTournament)
            {
                return;
            }

            isEndingTournament = true;

            onDeathEvent.Invoke();
        }

        public void ExitTournament()
        {
            //            playerComponentManager.isInBonfire = true;
            //            playerComponentManager.CurePlayer();

            teleportManager.Teleport(sceneToReturnWhenExitingTournament, "A");

        }

        void SpawnEnemiesForRound()
        {
            // Check stages
            if (currentStage == null)
            {
                currentStage = stages[0];
            }

            if (currentRound > 1)
            {
                // Change stage
                foreach (var stage in stages)
                {
                    stage.stageGameObject.SetActive(false);
                }


                List<Stage> newStages = stages.Where(x => x != currentStage).ToList();
                var idx = Random.Range(0, newStages.Count);

                currentStage = newStages[idx];
                newStages[idx].stageGameObject.SetActive(true);

            }


            if (currentStage != null)
            {
                bgmManager.StopAllCoroutines();

                if (currentStage.musics.Length > 0)
                {
                    bgmManager.PlayMusic(currentStage.musics[Random.Range(0, currentStage.musics.Length)]);
                }
            }



            SpecialRound specialRound = specialRounds.FirstOrDefault(x => x.round == currentRound);

            int maxEnemiesToSpawn = 1;
            int minEnemiesToSpawn = 1;

            List<CharacterManager> availableEnemies = new();

            if (specialRound != null)
            {
                // If it's a special round, use the specified parameters.
                maxEnemiesToSpawn = specialRound.numberOfEnemiesToSpawn;
                minEnemiesToSpawn = specialRound.numberOfEnemiesToSpawn;
                availableEnemies = specialRound.specialRoundEnemies;
            }
            else
            {
                if (currentRound <= 1)
                {
                    // For the initial rounds, gradually increase the difficulty.
                    maxEnemiesToSpawn = 1;

                    availableEnemies.AddRange((IEnumerable<CharacterManager>)(easyEnemies as IEnumerable));
                }
                else if (currentRound <= 5)
                {
                    // In early rounds, add medium enemies to the mix.
                    maxEnemiesToSpawn = 3;
                    minEnemiesToSpawn = 2;

                    availableEnemies.AddRange((IEnumerable<CharacterManager>)(easyEnemies as IEnumerable));

                    // Pick one medium enemy to add variety.
                    var randomMediumEnemyPicked = mediumEnemies[Random.Range(0, mediumEnemies.Count())];
                    availableEnemies.Add(randomMediumEnemyPicked);
                }
                else if (currentRound <= 10)
                {
                    // As rounds progress, introduce easy bosses or hard enemies.
                    maxEnemiesToSpawn = 4;
                    minEnemiesToSpawn = 2;

                    var mediumEnemy1 = mediumEnemies[Random.Range(0, mediumEnemies.Count())];
                    availableEnemies.Add(mediumEnemy1);
                    var mediumEnemy2 = mediumEnemies[Random.Range(0, mediumEnemies.Count())];
                    availableEnemies.Add(mediumEnemy2);

                    var easyBoss1 = easyBosses[Random.Range(0, easyBosses.Count())];
                    availableEnemies.Add(easyBoss1);

                    var hardEnemy1 = hardEnemies[Random.Range(0, hardEnemies.Count())];
                    availableEnemies.Add(hardEnemy1);
                }
                else if (currentRound <= 15)
                {
                    // As rounds progress, introduce easy bosses or hard enemies.
                    maxEnemiesToSpawn = 2;
                    minEnemiesToSpawn = 1;

                    var enemy1 = hardEnemies[Random.Range(0, hardEnemies.Count())];
                    availableEnemies.Add(enemy1);
                    var enemy2 = hardEnemies[Random.Range(0, hardEnemies.Count())];
                    availableEnemies.Add(enemy2);

                    var enemy3 = mediumBosses[Random.Range(0, mediumBosses.Count())];
                    availableEnemies.Add(enemy3);
                    var enemy4 = mediumBosses[Random.Range(0, mediumBosses.Count())];
                    availableEnemies.Add(enemy4);

                    var enemy5 = hardBosses[Random.Range(0, hardBosses.Count())];
                    availableEnemies.Add(enemy5);
                }
                else if (currentRound <= 25)
                {
                    // As rounds progress, introduce easy bosses or hard enemies.
                    maxEnemiesToSpawn = 3;
                    minEnemiesToSpawn = 1;

                    var enemy1 = hardEnemies[Random.Range(0, hardEnemies.Count())];
                    availableEnemies.Add(enemy1);

                    var enemy3 = mediumBosses[Random.Range(0, mediumBosses.Count())];
                    availableEnemies.Add(enemy3);
                    var enemy4 = mediumBosses[Random.Range(0, mediumBosses.Count())];
                    availableEnemies.Add(enemy4);
                    var enemy5 = mediumBosses[Random.Range(0, mediumBosses.Count())];
                    availableEnemies.Add(enemy5);

                    var enemy6 = hardBosses[Random.Range(0, hardBosses.Count())];
                    availableEnemies.Add(enemy6);
                    var enemy7 = hardBosses[Random.Range(0, hardBosses.Count())];
                    availableEnemies.Add(enemy7);
                }
                else if (currentRound > 25)
                {
                    // As rounds progress, introduce easy bosses or hard enemies.
                    maxEnemiesToSpawn = 3;
                    minEnemiesToSpawn = 1;

                    var enemy1 = hardEnemies[Random.Range(0, hardEnemies.Count())];
                    availableEnemies.Add(enemy1);

                    var enemy3 = mediumBosses[Random.Range(0, mediumBosses.Count())];
                    availableEnemies.Add(enemy3);
                    var enemy4 = mediumBosses[Random.Range(0, mediumBosses.Count())];
                    availableEnemies.Add(enemy4);

                    var enemy5 = hardBosses[Random.Range(0, hardBosses.Count())];
                    availableEnemies.Add(enemy5);
                    var enemy6 = hardBosses[Random.Range(0, hardBosses.Count())];
                    availableEnemies.Add(enemy6);
                    var enemy7 = hardBosses[Random.Range(0, hardBosses.Count())];
                    availableEnemies.Add(enemy7);
                    var enemy8 = hardBosses[Random.Range(0, hardBosses.Count())];
                    availableEnemies.Add(enemy8);
                }
            }

            int enemiesToSpawnThisRound = Random.Range(minEnemiesToSpawn, maxEnemiesToSpawn);
            for (int i = 0; i < enemiesToSpawnThisRound; i++)
            {
                int randomRangeForEnemyToSpawn = Random.Range(0, availableEnemies.Count());

                var choosenEnemyRound = availableEnemies[randomRangeForEnemyToSpawn];

                HandleInstatiatedEnemy(choosenEnemyRound);
            }


        }

        void HandleInstatiatedEnemy(CharacterManager choosenEnemyRound)
        {
            CharacterManager choosenEnemyRoundInstance = Instantiate(choosenEnemyRound,
                    spawnRefs[Random.Range(0, spawnRefs.Count())].transform.position,
                    Quaternion.identity);

            /*choosenEnemyRoundInstance.RepositionNavmeshAgent();

            var additionalLevels = (int)(currentRound / 10);
            choosenEnemyRoundInstance.currentLevel += additionalLevels;

            if (choosenEnemyRoundInstance.enemyBossController != null)
            {
                Destroy(choosenEnemyRoundInstance.enemyLoot);
                choosenEnemyRoundInstance.enemyBossController.bossMusic = null;
            }

            if (choosenEnemyRoundInstance.enemySleepController != null)
            {
                Destroy(choosenEnemyRoundInstance.enemySleepController.bed);
                Destroy(choosenEnemyRoundInstance.enemySleepController);
            }

            choosenEnemyRoundInstance.enemyHealthController.onEnemyDeathInColliseum.AddListener(() =>
            {
                CheckIfRoundShouldEnd();
            });

            choosenEnemyRoundInstance.alwaysTrackPlayer = true;
            choosenEnemyRoundInstance.enemyBehaviorController.ChasePlayer();
            choosenEnemyRoundInstance.enemyBehaviorController.TurnAgressive();*/
        }

        void ClearEnemiesFromRound()
        {
            var allEnemiesInRound = FindObjectsByType<CharacterManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            bool hasPlayedSound = false;
            foreach (var enemy in allEnemiesInRound)
            {
                var puffInstance = Instantiate(puffFx, enemy.transform.position, Quaternion.identity);
                if (hasPlayedSound == false)
                {
                    puffInstance.GetComponent<AudioSource>().Play();
                    hasPlayedSound = true;
                }
                Destroy(enemy.gameObject);
            }
        }

        public void CheckIfRoundShouldEnd()
        {
            var allEnemiesInRound = FindObjectsByType<CharacterManager>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            /*
                        if (allEnemiesInRound.All(x => x.enemyHealthController.currentHealth <= 0))
                        {
                            // End round
                            EndRound();
                        }*/
        }

        private void UpdateTimerUI()
        {
            counter.text = GetFormmatedTimed(elapsedTime);
        }


        int previousMinute = -1;
        public string GetFormmatedTimed(float elapsedTime)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(elapsedTime);

            int hours = timeSpan.Hours;
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;

            if (minutes != previousMinute && minutes >= 1)
            {
                previousMinute = minutes;

                // Code smell, it's a side effect to a otherwise pure function, but works for now
                // Time to spawn FX
                SpawnPowerup();
            }

            return $"{hours}h : {minutes}m : {seconds}s";
        }

        void SpawnPowerup()
        {
            NavMeshSurface navMeshSurface = FindAnyObjectByType<NavMeshSurface>(FindObjectsInactive.Exclude);

            // ArenaPowerup arenaPowerup = arenaPowerups[Random.Range(0, arenaPowerups.Length)];


            if (navMeshSurface != null)
            {
                Vector3 randomPointInSphere = Random.insideUnitSphere;

                // Scale and offset the point to fit within the NavMesh bounds
                float navMeshHeight = navMeshSurface.gameObject.transform.position.y;
                randomPointInSphere = new Vector3(randomPointInSphere.x, navMeshHeight, randomPointInSphere.z);

                // Sample the NavMesh for a valid position
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPointInSphere, out hit, 10f, NavMesh.AllAreas))
                {
                    // Use the hit.position as the valid random position on the NavMesh
                    Vector3 randomNavMeshPosition = hit.position;

                    // Instantiate(arenaPowerup, randomNavMeshPosition, Quaternion.identity);
                }
            }
            else
            {
                Transform targetTransform = FindAnyObjectByType<PlayerCombatController>().transform;

                // Teleport near player
                NavMesh.SamplePosition(targetTransform.position + targetTransform.forward * -2f, out NavMeshHit rightHit, 10f, NavMesh.AllAreas);

                if (rightHit.hit)
                {
                    // Instantiate(arenaPowerup, rightHit.position, Quaternion.identity);
                }
            }
        }

        public void ResumeTimer()
        {
            isTiming = true;
        }

        public void StopTimer()
        {
            isTiming = false;
        }

        public void ResetTimer()
        {
            isTiming = false;
            elapsedTime = 0f;
            UpdateTimerUI();
        }

        public bool HasBeatenNewRecord()
        {
            if (currentRound > recordForBestRound)
            {
                return true;
            }

            if (currentRound == recordForBestRound)
            {
                // Evaluate time
                return elapsedTime > recordForBestElapsedTime;
            }

            return false;
        }

    }

}
