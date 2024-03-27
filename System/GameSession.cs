using AF.Events;
using TigerForge;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSession", menuName = "System/New Game Session", order = 0)]
public class GameSession : ScriptableObject
{
    public enum GameState
    {
        NOT_INITIALIZED,
        INITIALIZED,
        INITIALIZED_AND_SHOWN_TITLE_SCREEN
    }

    public GameState gameState = GameState.NOT_INITIALIZED;

    [Header("Teleport")]
    public string nextMap_SpawnGameObjectName;

    [Header("Save Settings")]
    public bool loadSavedPlayerPositionAndRotation = false;
    public Vector3 savedPlayerPosition;
    public Quaternion savedPlayerRotation;

    [Header("World Settings")]
    public Gradient AmbientColor;
    public Gradient DirectionalColor;
    public bool useFog = true;
    public Gradient FogColor;

    [Header("Time Settings")]
    [Range(0, 24)] public float timeOfDay;
    public float initialTimeOfDay = 11;

    public int daysPassed = 0;
    public float daySpeed = 0.005f;

    [Header("Game Settings")]
    public float mouseSensitivity = 1f;
    public float minimumMouseSensitivity = 0f;
    public float maximumMouseSensitivity = 10f;

    public enum GraphicsQuality { LOW, MEDIUM, GOOD, ULTRA };
    public GraphicsQuality graphicsQuality = GraphicsQuality.GOOD;


#if UNITY_EDITOR
    private void OnEnable()
    {
        timeOfDay = initialTimeOfDay;

        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            Clear();
        }
    }
#endif

    void Clear()
    {
        gameState = GameState.NOT_INITIALIZED;
        initialTimeOfDay = 11;
    }

    public void SetTimeOfDay(int hour)
    {
        this.timeOfDay = hour;
    }

    public void IncreaseTimeOfDay()
    {
        if (timeOfDay > 23)
        {
            timeOfDay = 0;
        }
        else
        {
            timeOfDay++;
        }
    }

    public void SetGameQuality(int newValue)
    {

        if (newValue == 0)
        {
            graphicsQuality = GraphicsQuality.LOW;
            QualitySettings.SetQualityLevel(0);
        }
        else if (newValue == 1)
        {
            graphicsQuality = GraphicsQuality.MEDIUM;
            QualitySettings.SetQualityLevel(2);
        }
        else if (newValue == 2)
        {
            graphicsQuality = GraphicsQuality.GOOD;
            QualitySettings.SetQualityLevel(4);
        }
        else if (newValue == 3)
        {
            graphicsQuality = GraphicsQuality.ULTRA;
            QualitySettings.SetQualityLevel(5);
        }

        EventManager.EmitEvent(EventMessages.ON_GRAPHICS_QUALITY_CHANGED);
    }

}
