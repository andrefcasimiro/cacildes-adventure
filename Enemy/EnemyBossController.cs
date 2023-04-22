using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{
    public class EnemyBossController : MonoBehaviour
    {
        [Header("Boss")]
        public LocalizedText bossName;
        public GameObject fogWall;
        public AudioClip bossMusic;

        [Header("Switch")]
        public SwitchEntry bossSwitchEntry;
        [Tooltip("Update boss switch immediately after boss is killed")]
        public bool refreshEventsUponSwitchActivation = false;
        public SwitchListener ignoredSwitchListener;

        // UI Components
        [HideInInspector] public UIDocument bossHud => GetComponent<UIDocument>();
        [HideInInspector] public IMGUIContainer bossFillBar;

        public UnityEvent onBossBattleBeginEvent;
        bool hasRunBossBattleBeginEvent = false;

        private void Start()
        {
            InitializeBossHUD();
        }

        void InitializeBossHUD()
        {
            bossHud.enabled = true;
            bossFillBar = bossHud.rootVisualElement.Q<IMGUIContainer>("hp-bar");
            bossHud.rootVisualElement.Q<Label>("boss-name").text = bossName.GetText();

            HideBossHud();
        }

        public void UpdateBossHUDHealthPercentage(float currentHealthPercentage)
        {
            bossFillBar.style.width = new Length(currentHealthPercentage, LengthUnit.Percent);
        }

        public void ShowBossHud()
        {
            GetComponent<UIDocument>().enabled = true;

            bossFillBar = bossHud.rootVisualElement.Q<IMGUIContainer>("hp-bar");
            bossHud.rootVisualElement.Q<Label>("boss-name").text = bossName.GetText();

            GetComponent<EnemyHealthController>().HideHUD();

            bossHud.rootVisualElement.style.display = DisplayStyle.Flex;
        }

        public void HideBossHud()
        {
            bossHud.rootVisualElement.style.display = DisplayStyle.None;
            GetComponent<UIDocument>().enabled = false;
        }

        public void BeginBossBattle()
        {
            if (hasRunBossBattleBeginEvent == false && onBossBattleBeginEvent != null)
            {
                onBossBattleBeginEvent.Invoke();
            }

            ShowBossHud();

            if (fogWall != null)
            {
                fogWall.SetActive(true);
            }

            if (bossMusic != null)
            {
                if (BGMManager.instance.IsPlayingMusicClip(bossMusic.name) == false)
                {
                    BGMManager.instance.PlayMusic(bossMusic);
                }
            }
        }
    }
}
