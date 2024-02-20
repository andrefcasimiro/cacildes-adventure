using System.Linq;
using AF.Combat;
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

        public CharacterManager characterManager;

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
                if (characterManager.health.GetCurrentHealth() <= 0)
                {
                    HideBossHud();
                    return;
                }

                if (bossFillBar == null)
                {
                    bossFillBar = bossHud.rootVisualElement.Q<IMGUIContainer>("hp-bar");
                }

                bossFillBar.style.width = new Length(characterManager.health.GetCurrentHealth() * 100 / characterManager.health.GetMaxHealth(), LengthUnit.Percent);
            }
        }

        public void ShowBossHud()
        {
            bossHud.enabled = true;
            bossHud.rootVisualElement.Q<Label>("boss-name").text = bossName;
            bossHud.rootVisualElement.style.display = DisplayStyle.Flex;
            bossHud.rootVisualElement.Q<VisualElement>("container").style.marginBottom = characterManager.partnerOrder == 0 ? 0 : 60 * characterManager.partnerOrder;
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

            if (characterManager.partnerOrder == 0)
            {
                // Notify other boss companions that battle has begun
                foreach (CharacterManager partner in characterManager.partners)
                {
                    partner.characterBossController.BeginBossBattle();
                }
            }

            onBattleBegin?.Invoke();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnAllBossesDead()
        {
            bool isDead = characterManager.health.GetCurrentHealth() <= 0;

            bool allPartnersAreDead = isDead && characterManager.partners?.Length > 0
                && characterManager.partners.All(partner => partner.health.GetCurrentHealth() <= 0);

            if (characterManager.partners?.Length > 0 ? allPartnersAreDead : isDead)
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
            if (monoBehaviourID == null || flagsDatabase == null)
            {
                return;
            }

            flagsDatabase.AddFlag(monoBehaviourID.ID, "Boss killed: " + bossName);
        }

        public bool IsBoss()
        {
            return !string.IsNullOrEmpty(bossName);
        }
    }
}
