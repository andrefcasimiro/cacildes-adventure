using System.Collections;
using StarterAssets;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF {

    public class ViewClockMenu : MonoBehaviour
    {
        [HideInInspector]
        public VisualElement root;

        int currentHour = 0;

        DayNightManager dayNightManager;
        CursorManager cursorManager;
        ThirdPersonController thirdPersonController;
        SceneSettings sceneSettings;

        bool isPassingTime = false;

        Button passTimeButton, rest1HourButton, cancelButton;

        private void Awake()
        {
            dayNightManager = FindAnyObjectByType<DayNightManager>(FindObjectsInactive.Include);
            cursorManager = FindAnyObjectByType<CursorManager>(FindObjectsInactive.Include);
            sceneSettings = FindAnyObjectByType<SceneSettings>(FindObjectsInactive.Include);

            thirdPersonController = FindAnyObjectByType<ThirdPersonController>(FindObjectsInactive.Include);

            this.gameObject.SetActive(false);
        }

        protected virtual void OnEnable()
        {
            DrawUI();

            cursorManager.ShowCursor();

            thirdPersonController.LockCameraPosition = true;
        }

        private void OnDisable()
        {
            cursorManager.HideCursor();
        }

        void DrawUI()
        {
            root = GetComponent<UIDocument>().rootVisualElement;

            bool isEnglish = GamePreferences.instance.IsEnglish();

            root.Q<RadioButton>("0").RegisterValueChangedCallback(newValue =>
            {
                currentHour = 0;
            });
            root.Q<RadioButton>("3").RegisterValueChangedCallback(newValue =>
            {
                currentHour = 3;
            });
            root.Q<RadioButton>("6").RegisterValueChangedCallback(newValue =>
            {
                currentHour = 6;
            });
            root.Q<RadioButton>("9").RegisterValueChangedCallback(newValue =>
            {
                currentHour = 9;
            });
            root.Q<RadioButton>("12").RegisterValueChangedCallback(newValue =>
            {
                currentHour = 12;
            });
            root.Q<RadioButton>("15").RegisterValueChangedCallback(newValue =>
            {
                currentHour = 15;
            });
            root.Q<RadioButton>("18").RegisterValueChangedCallback(newValue =>
            {
                currentHour = 18;
            });
            root.Q<RadioButton>("21").RegisterValueChangedCallback(newValue =>
            {
                currentHour = 21;
            });

            passTimeButton = root.Q<Button>("PassTime");
            passTimeButton.text = isEnglish ? "Confirm" : "Confirmar";
            passTimeButton.SetEnabled(true);
            passTimeButton.clicked += HandleTimePassage;

            rest1HourButton = root.Q<Button>("Rest1Hour");
            rest1HourButton.text = isEnglish ? "Wait 1 Hour" : "Esperar 1 Hora";
            rest1HourButton.SetEnabled(true);
            rest1HourButton.clicked += Wait1Hour;

            cancelButton = root.Q<Button>("Cancel");
            cancelButton.text = isEnglish ? "Cancel" : "Cancelar";
            cancelButton.SetEnabled(true);
            cancelButton.clicked += () =>
            {
                if (isPassingTime)
                {
                    return;
                }

                this.gameObject.SetActive(false);
                thirdPersonController.LockCameraPosition = false;
            };


            cancelButton.style.visibility = Visibility.Visible;
            passTimeButton.style.visibility = Visibility.Visible;
            rest1HourButton.style.visibility = Visibility.Visible;

        }

        void HandleTimePassage()
        {
            if (isPassingTime)
            {
                return;
            }

            StartCoroutine(MoveTime(currentHour));
        }

        void Wait1Hour() {

            if (isPassingTime) {
                return;
            }

            StartCoroutine(MoveTime((int)Player.instance.timeOfDay + 1));
        }

        IEnumerator MoveTime(int desiredTime)
        {
            cancelButton.SetEnabled(false);
            passTimeButton.SetEnabled(false);
            rest1HourButton.SetEnabled(false);
            cancelButton.style.visibility = Visibility.Hidden;
            passTimeButton.style.visibility = Visibility.Hidden;
            rest1HourButton.style.visibility = Visibility.Hidden;


            var originalDaySpeed = Player.instance.daySpeed;

            var targetHour = desiredTime;
            bool isInteriorOriginal = sceneSettings.isInterior;

            if (!isPassingTime)
            {
                isPassingTime = true;

                sceneSettings.isInterior = false;

                dayNightManager.tick = true;

                if (targetHour > 23)
                {
                    Player.instance.timeOfDay = 0;
                    targetHour = 0;
                }

                yield return null;

                Player.instance.daySpeed = 3;

                yield return new WaitUntil(() => Mathf.FloorToInt(Player.instance.timeOfDay) == Mathf.FloorToInt(targetHour));

            }

            Player.instance.daySpeed = originalDaySpeed;

            dayNightManager.tick = dayNightManager.TimePassageAllowed();
            sceneSettings.isInterior = isInteriorOriginal;


            isPassingTime = false;

            this.gameObject.SetActive(false);
            thirdPersonController.LockCameraPosition = false;

            yield return null;
        }
    }

}