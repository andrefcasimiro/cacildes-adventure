using System.Linq;
using AF.Flags;
using AF.Health;
using AF.Music;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{
    public class CharacterBossController : MonoBehaviour
    {
        [Header("Settings")]
        public string bossName;
        public AudioClip bossMusic;

        [Header("Components")]
        public BGMManager bgmManager;
        public SceneSettings sceneSettings;

        public UIDocument bossHud;
        public IMGUIContainer bossFillBar;
        public CharacterHealth characterHealth;

        [Header("Partner")]
        public CharacterBossController[] partners;
        public int order = 0;

        [Header("Events")]
        public UnityEvent onBattleBegin;
        public UnityEvent onBossDefeated;

        // Flags
        bool bossBattleHasBegun = false;

        [Header("Flags")]
        public MonoBehaviourID monoBehaviourID;
        public FlagsDatabase flagsDatabase;

        private void Awake()
        {
            HideBossHud();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void UpdateUI()
        {
            if (IsBossHUDEnabled())
            {
                if (characterHealth.GetCurrentHealth() <= 0)
                {
                    HideBossHud();
                    return;
                }

                if (bossFillBar == null)
                {
                    bossFillBar = bossHud.rootVisualElement.Q<IMGUIContainer>("hp-bar");
                }

                bossFillBar.style.width = new Length(characterHealth.GetCurrentHealth() * 100 / characterHealth.GetMaxHealth(), LengthUnit.Percent);
            }
        }

        public void ShowBossHud()
        {
            bossHud.enabled = true;
            bossHud.rootVisualElement.Q<Label>("boss-name").text = bossName;
            bossHud.rootVisualElement.style.display = DisplayStyle.Flex;
            bossHud.rootVisualElement.Q<VisualElement>("container").style.marginBottom = order;
        }

        public void HideBossHud()
        {
            if (IsBossHUDEnabled())
            {
                bossHud.rootVisualElement.style.display = DisplayStyle.None;
            }
        }

        bool IsBossHUDEnabled()
        {
            return bossHud != null && bossHud.enabled;
        }

        public void BeginBossBattle()
        {
            if (bossBattleHasBegun)
            {
                return;
            }

            bossBattleHasBegun = true;

            ShowBossHud();

            if (bossMusic != null && bgmManager != null)
            {
                if (bgmManager.IsPlayingMusicClip(bossMusic.name) == false)
                {
                    bgmManager.PlayMusic(bossMusic);
                    bgmManager.isPlayingBossMusic = true;
                }
            }
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnAllBossesDead()
        {
            if (partners?.Length > 0)
            {
                if (characterHealth.GetCurrentHealth() <= 0 && partners.All(partner => partner.characterHealth.GetCurrentHealth() <= 0))
                {
                    onBossDefeated?.Invoke();
                    UpdateBossFlag();
                }
            }
            else if (characterHealth.GetCurrentHealth() <= 0)
            {
                onBossDefeated?.Invoke();
                UpdateBossFlag();
            }

            if (bgmManager != null)
            {
                bgmManager.isPlayingBossMusic = false;
            }

            if (sceneSettings != null)
            {
                sceneSettings.HandleSceneSound(true);
            }
        }

        void UpdateBossFlag()
        {
            flagsDatabase.AddFlag(monoBehaviourID.ID, "Boss killed: " + bossName);
        }

        public bool IsBoss()
        {
            return !string.IsNullOrEmpty(bossName);
        }
    }
}
