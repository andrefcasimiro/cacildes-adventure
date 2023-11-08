using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using static AF.SaveSystem;

namespace AF
{
    public interface IClockListener
    {

        public void OnHourChanged();

    }

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
        public float timeOfDay; // This is used only in editor. In game, we use the Player.instance.timeOfDay;
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

        IEnumerable<IClockListener> iClockListenersInScene;

        public bool canUpdateLighting = true;

        private void Awake()
        {
            iClockListenersInScene = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).OfType<IClockListener>();
        }

        private void Start()
        {
            if (Player.instance != null)
            {
                timeOfDay = Player.instance.timeOfDay;
            }

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
            var oldHour = Mathf.Round(timeOfDay);
            timeOfDay = newValue;

            if (Player.instance != null)
            {
                Player.instance.timeOfDay = newValue;
            }

            if (oldHour != Mathf.Round(newValue))
            {
                foreach (IClockListener iClockListener in iClockListenersInScene)
                {
                    iClockListener.OnHourChanged();
                }
            }
        }

        public void NotifyClockListeners()
        {
            foreach (IClockListener iClockListener in iClockListenersInScene)
            {
                iClockListener.OnHourChanged();
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

                var newTimeOfDayValue = timeOfDay;

                if (TimePassageAllowed() && tick)
                {
                    newTimeOfDayValue += ((Time.deltaTime * Player.instance.daySpeed));
                }

                var copy = newTimeOfDayValue;
                copy %= 25;

                newTimeOfDayValue %= 24; // Clamp between 0 - 24

                if (copy >= 24 && newTimeOfDayValue >= 0 && newTimeOfDayValue < 23)
                {
                    Player.instance.daysPassed++;
                }


                _SetInternalTimeOfDay(newTimeOfDayValue);

            }


            if (canUpdateLighting)
            {
                UpdateLighting(timeOfDay / 24f);
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

            var hour = (int)(timeOfDay);
            var minutes = Mathf.Abs(hour - timeOfDay) * 60;
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

            if (timeOfDay >= 5 && timeOfDay < 8)
            {
                dayNightIcon.style.backgroundImage = new StyleBackground(dawnSprite);
            }
            else if (timeOfDay > 8 && timeOfDay < 17)
            {
                dayNightIcon.style.backgroundImage = new StyleBackground(daySprite);
            }
            else if (timeOfDay > 17 && timeOfDay < 21)
            {
                dayNightIcon.style.backgroundImage = new StyleBackground(eveningSprite);
            }
            else if (timeOfDay >= 0 && timeOfDay < 5 || timeOfDay > 21 && timeOfDay <= 24)
            {
                dayNightIcon.style.backgroundImage = new StyleBackground(nightSprite);
            }
        }

        void UpdateLighting(float timePercent)
        {
            if (timeOfDay >= 7 && timeOfDay < 18f)
            {
                RenderSettings.skybox = daySky;
            }
            else if (timeOfDay >= 18f && timeOfDay < 20)
            {
                RenderSettings.skybox = duskSky;
            }
            else if (timeOfDay >= 20 && timeOfDay <= 22)
            {
                RenderSettings.skybox = nightfallSky;
            }
            else if (timeOfDay >= 22 && timeOfDay <= 24 || timeOfDay >= 0 && timeOfDay < 5)
            {
                RenderSettings.skybox = nightSky;
            }
            else if (timeOfDay >= 5 && timeOfDay < 7)
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
