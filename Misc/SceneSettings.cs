using UnityEngine;
using System.Collections;
using System.Linq;

namespace AF
{
    public class SceneSettings: MonoBehaviour, IClockListener
    {
        [Header("Music")]
        public AudioClip dayMusic;
        public AudioClip nightMusic;

        public AudioClip dayAmbience;
        public AudioClip nightAmbience;

        public AudioClip battleMusic;

        [Header("Map")]
        public bool isInterior;
        public bool isTitleScreen = false;

        [Header("Debug")]
        public bool enemiesIgnorePlayer = false;

        [Header("Tutorial")]
        public bool isSceneTutorial = false;
        public DestroyableParticle respawnFx;

        PlayerComponentManager playerComponentManager => FindObjectOfType<PlayerComponentManager>(true);
 
        [Header("Elemental Damage")]
        public GameObject elementalFireDamageFx;
        public Color elementalFireTextColor;
        public GameObject elementalFrostDamageFx;
        public Color elementalFrostTextColor;


        void Awake()
        {
            StartCoroutine(SpawnPlayer());
        }

        IEnumerator SpawnPlayer()
        {
            yield return null;

            TeleportManager.instance.SpawnPlayer(playerComponentManager.gameObject);
        }

        public void Start()
        {
            HandleSceneSound();

            if (enemiesIgnorePlayer)
            {
                var allEnemies = FindObjectsOfType<EnemyManager>(true);
                foreach (var enemy in allEnemies)
                {
                    if (enemy.enemySightController != null)
                    {
                        enemy.enemySightController.ignorePlayer = true;
                    }
                }
            }

            if (isTitleScreen && Player.instance.hasShownTitleScreen == false)
            {
                Utils.ShowCursor();
            }
            else
            {
                Utils.HideCursor();
            }
        }

        #region Scene Music & Ambience
        public void HandleSceneSound()
        {
            // Don't change map music if a combat is on going
            var allEnemies = FindObjectsOfType<EnemyManager>(true);
            if (allEnemies != null && allEnemies.Length > 0 && allEnemies.FirstOrDefault(x => x.enemyCombatController.IsInCombat()))
            {
                return;
            }

            EvaluateMusic();
            EvaluateAmbience();
        }

        void EvaluateMusic()
        {
            if (nightMusic == null && dayMusic == null)
            {
                BGMManager.instance.StopMusic();
                return;
            }

            // Play day only
            if (nightMusic == null)
            {
                if (IsPlayingSameMusic(dayMusic.name))
                {
                    return;
                }

                BGMManager.instance.PlayMusic(dayMusic);
                return;
            }

            if (Player.instance.timeOfDay >= 20 && Player.instance.timeOfDay <= 24 || Player.instance.timeOfDay >= 0 && Player.instance.timeOfDay < 6)
            {
                if (IsPlayingSameMusic(nightMusic.name))
                {
                    return;
                }

                BGMManager.instance.PlayMusic(nightMusic);
            }
            else
            {
                if (IsPlayingSameMusic(dayMusic.name))
                {
                    return;
                }

                BGMManager.instance.PlayMusic(dayMusic);
            }

        }

        bool IsPlayingSameMusic(string musicClipName)
        {
            return BGMManager.instance.bgmAudioSource.clip != null && BGMManager.instance.bgmAudioSource.clip.name == musicClipName;
        }

        void EvaluateAmbience()
        {
            if (nightAmbience == null && dayAmbience == null)
            {
                BGMManager.instance.StopAmbience();
                return;
            }

            // Play day only
            if (nightAmbience == null)
            {
                if (IsPlayingSameAmbience(dayAmbience.name))
                {
                    return;
                }

                BGMManager.instance.PlayAmbience(dayAmbience);
                return;
            }

            if (Player.instance.timeOfDay >= 20 && Player.instance.timeOfDay <= 24 || Player.instance.timeOfDay >= 0 && Player.instance.timeOfDay < 6)
            {
                if (IsPlayingSameAmbience(nightAmbience.name))
                {
                    return;
                }

                BGMManager.instance.PlayAmbience(nightAmbience);
            }
            else
            {
                if (IsPlayingSameAmbience(dayAmbience.name))
                {
                    return;
                }

                BGMManager.instance.PlayAmbience(dayAmbience);
            }

        }

        bool IsPlayingSameAmbience(string musicClipName)
        {
            return BGMManager.instance.ambienceAudioSource.clip != null && BGMManager.instance.ambienceAudioSource.clip.name == musicClipName;
        }

        public void OnHourChanged()
        {
            HandleSceneSound();
        }
        #endregion

        #region Tutorial
        public void RestartTutorialFromCheckpoint(Vector3 playerPosition)
        {
            // Cure player
            playerComponentManager.CurePlayer();

            // Find all active enemies in scene
            var allEnemiesInScene = FindObjectsOfType<EnemyManager>();
            foreach (var enemy in allEnemiesInScene)
            {
                if (enemy.enemyBossController != null)
                {
                    continue;
                }

                enemy.enabled = true;
                enemy.Revive();
            }

            // Attempt to teleport player
            var newPlayerPosition = Utils.GetNearestNavMeshPoint(playerPosition);

            playerComponentManager.UpdatePosition(newPlayerPosition, Quaternion.identity);
            Instantiate(respawnFx, newPlayerPosition, Quaternion.identity);
        }
        #endregion
    }

}
