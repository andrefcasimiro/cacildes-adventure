using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentBonfireMenu : MonoBehaviour
    {
        public Bonfire bonfire;

        public UIDocumentLevelUp uiDocumentLevelUp;

        bool isPassingTime = false;

        float originalDaySpeed = 0f;

        private void Start()
        {
            originalDaySpeed = Player.instance.daySpeed;
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            isPassingTime = false;

            var root = GetComponent<UIDocument>().rootVisualElement;

            root.Q<Label>("BonfireName").text = bonfire.bonfireName;

            root.Q<Button>("LeaveButton").RegisterCallback<ClickEvent>(ev =>
            {
                Player.instance.daySpeed = originalDaySpeed;

                bonfire.ExitBonfire();
                FindObjectOfType<GamepadCursor>(true).gameObject.SetActive(false);
            });
            root.Q<Button>("LevelUpButton").RegisterCallback<ClickEvent>(ev =>
            {
                uiDocumentLevelUp.gameObject.SetActive(true);
                this.gameObject.SetActive(false);
            });

            root.Q<Button>("PassTimeButton").RegisterCallback<ClickEvent>(ev =>
            {
                if (isPassingTime)
                {
                    return;
                }

                StartCoroutine(MoveTime());
            });
            
            Utils.ShowCursor();


            FindObjectOfType<GamepadCursor>(true).gameObject.SetActive(true);
        }

        IEnumerator MoveTime()
        {
            if (!isPassingTime)
            {
                isPassingTime = true;

                bool isInteriorOriginal = FindObjectOfType<SceneSettings>(true).isInterior;

                FindObjectOfType<SceneSettings>(true).isInterior = false;
                FindObjectOfType<DayNightManager>(true).tick = true;
                var originalDaySpeed = Player.instance.daySpeed;

                var targetHour = Mathf.Floor(Player.instance.timeOfDay) + 1;

                if (targetHour > 23)
                {
                    Player.instance.timeOfDay = 0;
                    targetHour = 0;
                }

                yield return null;

                Player.instance.daySpeed = 2;

                yield return new WaitUntil(() => Mathf.Floor(Player.instance.timeOfDay) == targetHour);

                Player.instance.daySpeed = originalDaySpeed;

                FindObjectOfType<DayNightManager>(true).tick = FindObjectOfType<DayNightManager>(true).TimePassageAllowed();
                FindObjectOfType<SceneSettings>(true).isInterior = isInteriorOriginal;

                isPassingTime = false;
            }

            yield return null;
        }


        private void Update()
        {
            // UnityEngine.Cursor.lockState = CursorLockMode.None;
        }
    }

}
