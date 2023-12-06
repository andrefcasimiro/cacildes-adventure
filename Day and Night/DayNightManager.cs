using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AF.Events;
using TigerForge;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

namespace AF
{
    [ExecuteInEditMode]
    public class DayNightManager : MonoBehaviour
    {

        public Light directionalLight;

        [Header("Scene Light")]
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
        public WorldSettings worldSettings;

        private void Start()
        {
            RenderSettings.fogDensity = fogDensity;
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
            var oldHour = Mathf.Round(worldSettings.timeOfDay);
            worldSettings.timeOfDay = newValue;


            if (oldHour != Mathf.Round(newValue))
            {
                EventManager.EmitEvent(EventMessages.ON_HOUR_CHANGED);
            }
        }

        private void Update()
        {
            if (dayNightIcon == null || dayNightText == null)
            {
                var root = FindObjectOfType<UIDocumentPlayerHUDV2>(true).root;
                if (root != null)
                {
                    dayNightIcon = root.Q<IMGUIContainer>("DayTimeIcon");
                    dayNightText = root.Q<VisualElement>("Clock").Q<Label>("Value");
                }
            }

            if (Application.isPlaying)
            {

                var newTimeOfDayValue = worldSettings.timeOfDay;

                if (TimePassageAllowed() && tick)
                {
                    newTimeOfDayValue += ((Time.deltaTime * worldSettings.daySpeed));
                }

                var copy = newTimeOfDayValue;
                copy %= 25;

                newTimeOfDayValue %= 24; // Clamp between 0 - 24

                if (copy >= 24 && newTimeOfDayValue >= 0 && newTimeOfDayValue < 23)
                {
                    worldSettings.daysPassed++;
                }


                _SetInternalTimeOfDay(newTimeOfDayValue);

            }


            if (canUpdateLighting)
            {
                UpdateLighting(worldSettings.timeOfDay / 24f);
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

            var hour = (int)(worldSettings.timeOfDay);
            var minutes = Mathf.Abs(hour - worldSettings.timeOfDay) * 60;
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

            if (worldSettings.timeOfDay >= 5 && worldSettings.timeOfDay < 8)
            {
                dayNightIcon.style.backgroundImage = new StyleBackground(dawnSprite);
            }
            else if (worldSettings.timeOfDay > 8 && worldSettings.timeOfDay < 17)
            {
                dayNightIcon.style.backgroundImage = new StyleBackground(daySprite);
            }
            else if (worldSettings.timeOfDay > 17 && worldSettings.timeOfDay < 21)
            {
                dayNightIcon.style.backgroundImage = new StyleBackground(eveningSprite);
            }
            else if (worldSettings.timeOfDay >= 0 && worldSettings.timeOfDay < 5 || worldSettings.timeOfDay > 21 && worldSettings.timeOfDay <= 24)
            {
                dayNightIcon.style.backgroundImage = new StyleBackground(nightSprite);
            }
        }

        void UpdateLighting(float timePercent)
        {
            if (worldSettings.timeOfDay >= 7 && worldSettings.timeOfDay < 18f)
            {
                RenderSettings.skybox = daySky;
            }
            else if (worldSettings.timeOfDay >= 18f && worldSettings.timeOfDay < 20)
            {
                RenderSettings.skybox = duskSky;
            }
            else if (worldSettings.timeOfDay >= 20 && worldSettings.timeOfDay <= 22)
            {
                RenderSettings.skybox = nightfallSky;
            }
            else if (worldSettings.timeOfDay >= 22 && worldSettings.timeOfDay <= 24 || worldSettings.timeOfDay >= 0 && worldSettings.timeOfDay < 5)
            {
                RenderSettings.skybox = nightSky;
            }
            else if (worldSettings.timeOfDay >= 5 && worldSettings.timeOfDay < 7)
            {
                RenderSettings.skybox = dawnSky;
            }

            RenderSettings.ambientLight = AmbientColor.Evaluate(timePercent);

            if (useFog)
            {
                RenderSettings.fogColor = FogColor.Evaluate(timePercent);
            }

            if (directionalLight != null)
            {
                directionalLight.color = DirectionalColor.Evaluate(timePercent);
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
