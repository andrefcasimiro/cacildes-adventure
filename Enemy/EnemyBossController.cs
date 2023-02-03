using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class EnemyBossController : MonoBehaviour
    {
        [Header("Boss")]
        public string bossName = "";
        public GameObject fogWall;
        public AudioClip bossMusic;

        [Header("Switch")]
        public string bossSwitchUuid;
        [Tooltip("Update boss switch immediately after boss is killed")]
        public bool updateSwitchOnKill = false;

        // UI Components
        [HideInInspector] public UnityEngine.UIElements.UIDocument bossHud => GetComponent<UIDocument>();
        [HideInInspector] public UnityEngine.UIElements.IMGUIContainer bossFillBar;

        private void Start()
        {
            InitializeBossHUD();
        }

        void InitializeBossHUD()
        {
            bossHud.enabled = true;
            bossFillBar = bossHud.rootVisualElement.Q<IMGUIContainer>("hp-bar");
            bossHud.rootVisualElement.Q<Label>("boss-name").text = bossName;

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
            bossHud.rootVisualElement.Q<Label>("boss-name").text = bossName;

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
            ShowBossHud();

            if (fogWall != null)
            {
                fogWall.gameObject.SetActive(true);
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
