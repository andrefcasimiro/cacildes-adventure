using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace AF
{

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

        public StarterAssetsInputs starterAssetsInputs;

        [Header("Achievements")]
        public Achievement achievementForPassingTime;
        [Header("Systems")]
        public WorldSettings worldSettings;


        private void Awake()
        {
            dayNightManager = FindAnyObjectByType<DayNightManager>(FindObjectsInactive.Include);
            cursorManager = FindAnyObjectByType<CursorManager>(FindObjectsInactive.Include);
            sceneSettings = FindAnyObjectByType<SceneSettings>(FindObjectsInactive.Include);

            thirdPersonController = FindAnyObjectByType<ThirdPersonController>(FindObjectsInactive.Include);

            this.gameObject.SetActive(false);
        }

        private void Start()
        {

        }

        void Close()
        {

            if (isPassingTime)
            {
                return;
            }

            this.gameObject.SetActive(false);
        }

        protected virtual void OnEnable()
        {
            thirdPersonController.LockCameraPosition = true;

            DrawUI();

            cursorManager.ShowCursor();
        }

        private void Update()
        {
            if (isPassingTime == false)
            {
                thirdPersonController.LockCameraPosition = true;
            }
            else
            {
                thirdPersonController.LockCameraPosition = false;
            }

            if (Cursor.visible == false)
            {
                cursorManager.ShowCursor();
            }
        }

        private void OnDisable()
        {
            cursorManager.HideCursor();
            thirdPersonController.LockCameraPosition = false;
        }

        void DrawUI()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            this.root.style.display = DisplayStyle.Flex;



            thirdPersonController.GetComponent<PlayerCombatController>().enabled = false;

            root.Q<RadioButton>("0").RegisterCallback<ClickEvent>(newValue =>
            {
                UpdateCurrentHour(0);
            });
            root.Q<RadioButton>("3").RegisterCallback<ClickEvent>(newValue =>
            {
                UpdateCurrentHour(3);
            });
            root.Q<RadioButton>("6").RegisterCallback<ClickEvent>(newValue =>
            {
                UpdateCurrentHour(6);
            });
            root.Q<RadioButton>("9").RegisterCallback<ClickEvent>(newValue =>
            {
                UpdateCurrentHour(9);
            });
            root.Q<RadioButton>("12").RegisterCallback<ClickEvent>(newValue =>
            {
                UpdateCurrentHour(12);
            });
            root.Q<RadioButton>("15").RegisterCallback<ClickEvent>(newValue =>
            {
                UpdateCurrentHour(15);
            });
            root.Q<RadioButton>("18").RegisterCallback<ClickEvent>(newValue =>
            {
                UpdateCurrentHour(18);
            });
            root.Q<RadioButton>("21").RegisterCallback<ClickEvent>(newValue =>
            {
                UpdateCurrentHour(21);
            });

            passTimeButton = root.Q<Button>("PassTime");
            passTimeButton.text = "Confirm";
            passTimeButton.SetEnabled(true);
            passTimeButton.clicked += () =>
            {
                HandleTimePassage();
            };

            rest1HourButton = root.Q<Button>("Rest1Hour");
            rest1HourButton.text = "Wait 1 Hour";
            rest1HourButton.SetEnabled(true);
            rest1HourButton.clicked += Wait1Hour;

            cancelButton = root.Q<Button>("Cancel");
            cancelButton.text = "Cancel";
            cancelButton.SetEnabled(true);
            cancelButton.clicked += () =>
            {
                if (isPassingTime)
                {
                    return;
                }

                thirdPersonController.GetComponent<PlayerCombatController>().enabled = true;

                this.gameObject.SetActive(false);
                thirdPersonController.LockCameraPosition = false;
            };


            cancelButton.style.visibility = Visibility.Visible;
            passTimeButton.style.visibility = Visibility.Visible;
            rest1HourButton.style.visibility = Visibility.Visible;

        }

        void UpdateCurrentHour(int currentHour)
        {
            this.currentHour = currentHour;
        }

        void HandleTimePassage()
        {
            if (isPassingTime)
            {
                return;
            }

            achievementForPassingTime.AwardAchievement();

            StartCoroutine(MoveTime(currentHour));
        }

        void Wait1Hour()
        {

            if (isPassingTime)
            {
                return;
            }

            achievementForPassingTime.AwardAchievement();

            StartCoroutine(MoveTime((int)worldSettings.timeOfDay + 1));
        }

        IEnumerator MoveTime(int desiredTime)
        {
            thirdPersonController.GetComponent<PlayerCombatController>().enabled = true;

            cancelButton.SetEnabled(false);
            passTimeButton.SetEnabled(false);
            rest1HourButton.SetEnabled(false);
            cancelButton.style.visibility = Visibility.Hidden;
            passTimeButton.style.visibility = Visibility.Hidden;
            rest1HourButton.style.visibility = Visibility.Hidden;


            this.root.style.display = DisplayStyle.None;

            var originalDaySpeed = worldSettings.daySpeed;

            var targetHour = desiredTime;
            bool isInteriorOriginal = sceneSettings.isInterior;

            if (!isPassingTime)
            {
                isPassingTime = true;

                sceneSettings.isInterior = false;

                dayNightManager.tick = true;

                if (targetHour > 23)
                {
                    worldSettings.timeOfDay = 0;
                    targetHour = 0;
                }

                yield return null;

                worldSettings.daySpeed = 3;

                yield return new WaitUntil(() =>
                {
                    return Mathf.FloorToInt(worldSettings.timeOfDay) == Mathf.FloorToInt(targetHour);
                });

            }

            worldSettings.daySpeed = originalDaySpeed;

            dayNightManager.tick = dayNightManager.TimePassageAllowed();
            sceneSettings.isInterior = isInteriorOriginal;

            isPassingTime = false;

            this.gameObject.SetActive(false);
            thirdPersonController.LockCameraPosition = false;

            yield return null;
        }
    }

}