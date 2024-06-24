using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AF.Events;
using AF.Music;
using AYellowpaper.SerializedCollections;
using TigerForge;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Localization;
using Random = UnityEngine.Random;

namespace AF.Arena
{
    [System.Serializable]
    public class EnemyRound
    {
        public List<CharacterManager> enemies = new();
        public AudioClip music;
        public int numberOfEnemiesToSpawnOverride = -1;
    }

    public class ArenaManager : MonoBehaviour
    {
        [SerializedDictionary("Round Number", "Enemies")]
        public SerializedDictionary<int, EnemyRound> enemyRounds = new();
        public Transform[] enemySpawnRefs;

        public int currentRoundIndex = 1;
        public EnemyRound currentEnemyRound = null;
        List<CharacterManager> enemiesInRound = new();

        public ArenaManagerUI arenaManagerUI;

        private bool isTiming = false;
        public float elapsedTime = 0f;

        [Header("Settings")]
        public int intervalBetweenRounds;

        [Header("Powerups")]
        public GameObject arenaPowerupPrefab;

        [Header("Components")]
        public BGMManager bgmManager;
        public Soundbank soundbank;
        public PlayerManager playerManager;
        public NotificationManager notificationManager;
        public UIDocumentPlayerGold uIDocumentPlayerGold;

        [Header("Databases")]
        public GameSession gameSession;
        public PlayerStatsDatabase playerStatsDatabase;
        public Achievement arenaAchievement;

        [Header("FX")]
        public DestroyableParticle puffFx;
        public AudioClip applauseSfx;

        [Header("Events")]
        public UnityEvent onRoundWon;
        public UnityEvent onArenaStarts;
        public UnityEvent onArenaEnds;

        [Header("Moment")]
        public Moment arenaWonMoment;

        int initialGoldWhenStartingArena;

        // You lost this time. Congratulations on gettings this far!"
        public LocalizedString youLostThisTime;

        // "You exited the arena. Ring the bell to try again"
        public LocalizedString youExitedTheArena;

        // The crowd voted, your items have been replenished. Good luck!
        public LocalizedString yourItemsHaveBeenReplenished;
        // "The crowd voted, your health was restored. They wish to see you fight!"
        public LocalizedString yourHealthWasRestored;

        private void Awake()
        {
            arenaManagerUI.onMinuteChanged += SpawnPowerup;
            arenaManagerUI.maxRounds = enemyRounds.Keys.Last();

            EventManager.StartListening(EventMessages.ON_ARENA_LOST, () =>
            {
                notificationManager.ShowNotification(youLostThisTime.GetLocalizedString(), null);
                EndArena(false);
            });
        }


        private void Update()
        {
            if (isTiming)
            {
                elapsedTime += Time.deltaTime;
                arenaManagerUI.UpdateTimerUI(elapsedTime);
            }
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnArenaStarts()
        {
            currentRoundIndex = 0;
            gameSession.SetIsParticipatingInArenaEvent(true);

            AdvanceToNextRound();
            arenaManagerUI.gameObject.SetActive(true);

            onArenaStarts?.Invoke();

            initialGoldWhenStartingArena = playerStatsDatabase.gold;
        }

        public void EndArenaDueToExitingArea()
        {
            notificationManager.ShowNotification(youExitedTheArena.GetLocalizedString(), notificationManager.systemError);
            EndArena(false);
        }

        public void EndArena(bool hasWon)
        {
            gameSession.SetIsParticipatingInArenaEvent(false);
            arenaManagerUI.gameObject.SetActive(false);

            onArenaEnds?.Invoke();

            ClearEnemiesFromRound();

            bgmManager.PlayMapMusicAfterKillingEnemy();

            if (hasWon)
            {
                arenaAchievement?.AwardAchievement();
                arenaWonMoment?.Trigger();
            }
            else
            {
                uIDocumentPlayerGold.LoseGold(playerStatsDatabase.gold - initialGoldWhenStartingArena);
            }
        }

        void AdvanceToNextRound()
        {
            if (!gameSession.isParticipatingInArenaEvent)
            {
                return;
            }

            isTiming = true;

            currentRoundIndex++;
            arenaManagerUI.UpdateCurrentRound(currentRoundIndex);
            currentEnemyRound = GetEnemyRound();

            bgmManager.PlayMusic(currentEnemyRound.music);

            enemiesInRound.Clear();

            int enemiesToSpawnThisRound = currentEnemyRound.numberOfEnemiesToSpawnOverride > 0
                ? currentEnemyRound.numberOfEnemiesToSpawnOverride : GetMaxEnemiesToSpawnBasedOnCurrentRound();

            for (int i = 0; i < enemiesToSpawnThisRound; i++)
            {
                CharacterManager choosenEnemyRound = currentEnemyRound.enemies[Random.Range(0, currentEnemyRound.enemies.Count)];

                if (choosenEnemyRound != null)
                {
                    HandleInstatiatedEnemy(choosenEnemyRound);
                }
            }
        }

        public void EndRound()
        {
            bgmManager.StopMusicImmediately();

            isTiming = false;

            Invoke(nameof(ClearEnemiesFromRound), .75f);

            if (Random.Range(0, 1f) >= 0.5f)
            {
                StartCoroutine(PlayApplause());
            }

            if (currentRoundIndex % 5 == 0)
            {
                playerManager.playerInventory.ReplenishItems();
                soundbank.PlaySound(soundbank.uiItemReceived);
                notificationManager.ShowNotification(yourItemsHaveBeenReplenished.GetLocalizedString(), null);
            }
            else if (currentRoundIndex % 3 == 0 && Random.Range(0, 100f) > 50f)
            {
                playerManager.health.RestoreFullHealth();
                soundbank.PlaySound(soundbank.uiItemReceived);
                notificationManager.ShowNotification(yourHealthWasRestored.GetLocalizedString()
                    , null);
            }

            if (currentRoundIndex + 1 > enemyRounds.Keys.Last())
            {
                EndArena(true);
            }
            else
            {
                StartCoroutine(WaitAndCallNextRound());
            }
        }
        public int GetMaxEnemiesToSpawnBasedOnCurrentRound()
        {
            if (currentRoundIndex % 5 == 0)
            {
                return 3;
            }

            if (currentRoundIndex % 5 == 1)
            {
                return 1;
            }

            return 2;
        }

        void HandleInstatiatedEnemy(CharacterManager choosenEnemyRound)
        {
            CharacterManager choosenEnemyRoundInstance = Instantiate(
                    choosenEnemyRound,
                    enemySpawnRefs[Random.Range(0, enemySpawnRefs.Count())].transform.position,
                    Quaternion.identity);

            choosenEnemyRoundInstance?.health?.onDeath?.AddListener(CheckIfRoundShouldEnd);
            choosenEnemyRoundInstance?.onForceAgressionTowardsPlayer?.Invoke();

            enemiesInRound.Add(choosenEnemyRoundInstance);
        }

        void ClearEnemiesFromRound()
        {
            if (enemiesInRound.Count <= 0)
            {
                return;
            }

            bool hasPlayedSound = false;
            foreach (var enemy in enemiesInRound)
            {
                var puffInstance = Instantiate(puffFx, enemy.transform.position, Quaternion.identity);

                if (hasPlayedSound == false)
                {
                    puffInstance.GetComponent<AudioSource>().Play();
                    hasPlayedSound = true;
                }

                Destroy(enemy.gameObject);
            }

            enemiesInRound.Clear();
        }

        public void CheckIfRoundShouldEnd()
        {
            if (enemiesInRound != null && enemiesInRound.Count > 0 && enemiesInRound.All(enemy => enemy?.health?.GetCurrentHealth() <= 0))
            {
                EndRound();
            }
        }

        void SpawnPowerup()
        {
            // Teleport near player
            NavMesh.SamplePosition(
                playerManager.transform.position + playerManager.transform.forward * -2f, out NavMeshHit rightHit, 10f, NavMesh.AllAreas);

            if (rightHit.hit)
            {
                Instantiate(arenaPowerupPrefab, rightHit.position, Quaternion.identity);
            }
        }

        IEnumerator WaitAndCallNextRound()
        {
            int waitTimeInSeconds = intervalBetweenRounds;  // Dynamic wait time in seconds

            while (waitTimeInSeconds > 0)
            {
                arenaManagerUI.UpdateWaitingForNextRound(waitTimeInSeconds);
                yield return new WaitForSeconds(1);
                waitTimeInSeconds--;
            }

            AdvanceToNextRound();
        }

        IEnumerator PlayApplause()
        {
            yield return new WaitForSeconds(0.15f);
            bgmManager.PlaySound(applauseSfx, null);
        }

        EnemyRound GetEnemyRound()
        {
            EnemyRound enemyRound = null;

            // Find the highest available round less than or equal to currentRound
            foreach (var kvp in enemyRounds)
            {
                if (currentRoundIndex <= kvp.Key)
                {
                    enemyRound = kvp.Value;
                    break;
                }
            }

            if (enemyRound == null)
            {
                Debug.LogWarning($"No enemy round defined for round {currentRoundIndex}");
            }

            return enemyRound;
        }

    }
}
