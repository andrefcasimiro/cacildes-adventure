using System.Linq;
using AF.Combat;
using AF.Events;
using AF.Flags;
using AF.Health;
using AF.Music;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{
    public class CharacterBossController : MonoBehaviour
    {
        public bool isBoss = false;

        [Header("Settings")]
        public string bossName;
        public AudioClip bossMusic;

        public UIDocument bossHud;
        public IMGUIContainer bossFillBar;

        public CharacterManager characterManager;

        [Header("Events")]
        public UnityEvent onBattleBegin;
        public UnityEvent onBossDefeated;

        // Flags
        [HideInInspector] public bool bossBattleHasBegun = false;

        [Header("Flags")]
        public MonoBehaviourID monoBehaviourID;
        public FlagsDatabase flagsDatabase;

        // Scene References
        private BGMManager bgmManager;
        private SceneSettings sceneSettings;

        public void Start()
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
                if (!isBoss)
                {
                    return;
                }

                if (characterManager.health.GetCurrentHealth() <= 0)
                {
                    HideBossHud();
                    return;
                }

                bossFillBar ??= bossHud.rootVisualElement.Q<IMGUIContainer>("hp-bar");
                bossFillBar.style.width = new Length(characterManager.health.GetCurrentHealth() * 100 / characterManager.health.GetMaxHealth(), LengthUnit.Percent);
            }
        }

        public void ShowBossHud()
        {
            if (bossHud == null)
            {
                return;
            }

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

        public bool IsBossHUDEnabled()
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

            if (bossMusic != null && GetBGMManager() != null)
            {
                if (GetBGMManager().IsPlayingMusicClip(bossMusic.name) == false)
                {
                    GetBGMManager().PlayMusic(bossMusic);
                    GetBGMManager().isPlayingBossMusic = true;
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

            EventManager.EmitEvent(EventMessages.ON_BOSS_BATTLE_BEGINS);
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
                if (GetBGMManager() != null)
                {
                    GetBGMManager().isPlayingBossMusic = false;
                }

                GetSceneSettings().HandleSceneSound(true);

                EventManager.EmitEvent(EventMessages.ON_BOSS_BATTLE_ENDS);
                onBossDefeated?.Invoke();
                UpdateBossFlag();
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
            return isBoss;
        }

        BGMManager GetBGMManager()
        {
            if (bgmManager == null)
            {
                bgmManager = FindAnyObjectByType<BGMManager>(FindObjectsInactive.Include);
            }

            return bgmManager;
        }

        SceneSettings GetSceneSettings()
        {
            if (sceneSettings == null)
            {
                sceneSettings = FindAnyObjectByType<SceneSettings>(FindObjectsInactive.Include);
            }
            return sceneSettings;
        }
    }
}
