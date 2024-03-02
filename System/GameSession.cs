using AF;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSession", menuName = "System/New Game Session", order = 0)]
public class GameSession : ScriptableObject
{
    public bool hasShownTitleScreen = false;

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

    public bool shouldClearOnExitPlayMode = false;


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
            if (shouldClearOnExitPlayMode)
            {
                Clear();
            }
        }
    }
#endif

    void Clear()
    {
        hasShownTitleScreen = false;
    }

    public void SetTimeOfDay(int hour)
    {
        this.timeOfDay = hour;
    }

}
