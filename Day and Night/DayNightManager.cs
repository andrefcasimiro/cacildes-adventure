using AF.Events;
using TigerForge;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    [ExecuteInEditMode]
    public class DayNightManager : MonoBehaviour
    {
        public Light directionalLight;

        [Header("Scene Light")]
        public bool useOverride = false;
        public Gradient AmbientColor;
        public Gradient DirectionalColor;
        public bool useFog = true;
        public Gradient FogColor;

        public float fogDensity = 0.03f;

        [Header("Values")]
        [Range(0, 24)]
        [TextArea] public string comment = "Put timeOfDay at 0 after finishing playing around with it";
        public bool tick = true;

        [Header("Skyboxes")]
        public Material dawnSky;
        public Material daySky;
        public Material duskSky;
        public Material nightfallSky;
        public Material nightSky;

        [Header("UI")]
        public UIDocumentPlayerHUDV2 uIDocumentPlayerHUDV2;
        public Sprite dawnSprite;
        public Sprite daySprite;
        public Sprite eveningSprite;
        public Sprite nightSprite;
        [HideInInspector] public IMGUIContainer dayNightIcon;
        [HideInInspector] public Label dayNightText;

        public SceneSettings sceneSettings;
        public bool canUpdateLighting = true;

        [Header("Systems")]
        public GameSession gameSession;

        private void Start()
        {
            RenderSettings.fogDensity = fogDensity;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void AdvanceOneHour()
        {
            SetHour((int)(gameSession.timeOfDay + 1));
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void GoBackOneHour()
        {
            var targetHour = (int)gameSession.timeOfDay - 1;
            if (targetHour < 0)
            {
                targetHour = 23;
            }

            SetHour(targetHour);
        }

        public void SetHour(int hour)
        {
            SetTimeOfDay(hour, 0);
        }

        public void SetTimeOfDay(int hours, int minutes)
        {
            var min = (minutes / 60);
            _SetInternalTimeOfDay(hours + min);
        }

        void _SetInternalTimeOfDay(float newValue)
        {
            var oldHour = Mathf.Round(gameSession.timeOfDay);
            gameSession.timeOfDay = newValue;


            if (oldHour != Mathf.Round(newValue))
            {
                EventManager.EmitEvent(EventMessages.ON_HOUR_CHANGED);
            }
        }

        private void OnEnable()
        {
            if (dayNightIcon == null || dayNightText == null)
            {
                var root = uIDocumentPlayerHUDV2.root;
                if (root != null)
                {
                    dayNightIcon = root.Q<IMGUIContainer>("DayTimeIcon");
                    dayNightText = root.Q<VisualElement>("Clock").Q<Label>("Value");
                }
            }
        }

        private void Update()
        {
            if (Application.isPlaying)
            {
                var newTimeOfDayValue = gameSession.timeOfDay;

                if (TimePassageAllowed() && tick)
                {
                    newTimeOfDayValue += Time.deltaTime * gameSession.daySpeed;
                }

                var copy = newTimeOfDayValue;
                copy %= 25;

                newTimeOfDayValue %= 24; // Clamp between 0 - 24

                if (copy >= 24 && newTimeOfDayValue >= 0 && newTimeOfDayValue < 23)
                {
                    gameSession.daysPassed++;
                }


                _SetInternalTimeOfDay(newTimeOfDayValue);
            }


            if (canUpdateLighting)
            {
                UpdateLighting(gameSession.timeOfDay / 24f);
            }

            ShowClockText();
            ShowClockIcon();
        }

        void ShowClockText()
        {
            if (uIDocumentPlayerHUDV2 != null && uIDocumentPlayerHUDV2.root != null && uIDocumentPlayerHUDV2.isActiveAndEnabled)
            {
                dayNightText = uIDocumentPlayerHUDV2.root.Q<VisualElement>("Clock").Q<Label>("Value");
            }

            if (dayNightText == null)
            {
                return;
            }

            var hour = (int)gameSession.timeOfDay;
            var minutes = Mathf.Abs(hour - gameSession.timeOfDay) * 60;
            minutes = (int)System.Math.Round(minutes, 2);
            string hourString = hour.ToString();
            if (hourString.Length == 1)
            {
                hourString = "0" + hour.ToString();
            }

            string minutesString = minutes.ToString();
            if (minutesString.Length == 1)
            {
                minutesString = "0" + minutesString;
            }

            dayNightText.text = hourString + ":" + minutesString;
        }

        void ShowClockIcon()
        {
            if (uIDocumentPlayerHUDV2.isActiveAndEnabled && uIDocumentPlayerHUDV2 != null && uIDocumentPlayerHUDV2.root != null)
            {
                dayNightIcon = uIDocumentPlayerHUDV2.root.Q<IMGUIContainer>("DayTimeIcon");
            }

            if (dayNightIcon == null)
            {
                return;
            }

            if (gameSession.timeOfDay >= 5 && gameSession.timeOfDay < 8)
            {
                dayNightIcon.style.backgroundImage = new StyleBackground(dawnSprite);
            }
            else if (gameSession.timeOfDay > 8 && gameSession.timeOfDay < 17)
            {
                dayNightIcon.style.backgroundImage = new StyleBackground(daySprite);
            }
            else if (gameSession.timeOfDay > 17 && gameSession.timeOfDay < 21)
            {
                dayNightIcon.style.backgroundImage = new StyleBackground(eveningSprite);
            }
            else if (gameSession.timeOfDay >= 0 && gameSession.timeOfDay < 5 || gameSession.timeOfDay > 21 && gameSession.timeOfDay <= 24)
            {
                dayNightIcon.style.backgroundImage = new StyleBackground(nightSprite);
            }
        }

        void UpdateLighting(float timePercent)
        {
            if (gameSession.timeOfDay >= 7 && gameSession.timeOfDay < 18f)
            {
                RenderSettings.skybox = daySky;
            }
            else if (gameSession.timeOfDay >= 18f && gameSession.timeOfDay < 20)
            {
                RenderSettings.skybox = duskSky;
            }
            else if (gameSession.timeOfDay >= 20 && gameSession.timeOfDay <= 22)
            {
                RenderSettings.skybox = nightfallSky;
            }
            else if (gameSession.timeOfDay >= 22 && gameSession.timeOfDay <= 24 || gameSession.timeOfDay >= 0 && gameSession.timeOfDay < 5)
            {
                RenderSettings.skybox = nightSky;
            }
            else if (gameSession.timeOfDay >= 5 && gameSession.timeOfDay < 7)
            {
                RenderSettings.skybox = dawnSky;
            }

            RenderSettings.ambientLight = useOverride ? AmbientColor.Evaluate(timePercent) : gameSession.AmbientColor.Evaluate(timePercent);

            if (useFog)
            {
                RenderSettings.fogColor = useOverride ? FogColor.Evaluate(timePercent) : gameSession.FogColor.Evaluate(timePercent);
            }

            if (directionalLight != null)
            {
                directionalLight.color = useOverride ? DirectionalColor.Evaluate(timePercent) : gameSession.DirectionalColor.Evaluate(timePercent);
                directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, -170f, 0));
            }
        }

        public bool TimePassageAllowed()
        {
            return sceneSettings != null && sceneSettings.isInterior == false;
        }

        private void OnValidate()
        {
            if (directionalLight != null)
            {
                return;
            }

            if (RenderSettings.sun != null)
            {
                directionalLight = RenderSettings.sun;
            }
            else
            {
                Light[] lights = FindObjectsOfType<Light>();
                foreach (Light light in lights)
                {
                    if (light.type == LightType.Directional)
                    {
                        directionalLight = light;
                    }
                }
            }
        }
    }
}
