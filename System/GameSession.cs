using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSession", menuName = "System/New Game Session", order = 0)]
public class GameSession : ScriptableObject
{
    public enum GameState
    {
        NOT_INITIALIZED,
        INITIALIZED,
        INITIALIZED_AND_SHOWN_TITLE_SCREEN,
        BEGINNING_NEW_GAME_PLUS
    }

    public GameState gameState = GameState.NOT_INITIALIZED;

    [Header("Teleport")]
    public string nextMap_SpawnGameObjectName;
    [Header("Arena Settings")]
    public bool isParticipatingInArenaEvent = false;

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

    [Header("New Game Plus")]
    public int currentGameIteration = 0;
    public float newGamePlusScalingFactor = 1.25f;


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
        isParticipatingInArenaEvent = false;
        loadSavedPlayerPositionAndRotation = false;
        nextMap_SpawnGameObjectName = "";
        currentGameIteration = 0;
    }

    /// <summary>
    /// Unity Event
    /// </summary>
    /// <param name="hour"></param>
    public void SetTimeOfDay(int hour)
    {
        this.timeOfDay = hour;
    }

    /// <summary>
    /// Unity Event
    /// </summary>
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

    public void SetIsParticipatingInArenaEvent(bool value)
    {
        isParticipatingInArenaEvent = value;
    }

    public bool IsNightTime()
    {
        return timeOfDay >= 20 && timeOfDay <= 24 || timeOfDay >= 0 && timeOfDay < 6;
    }
}
