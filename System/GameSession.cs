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

    public bool shouldClearOnExitPlayMode = false;

#if UNITY_EDITOR 
    private void OnEnable()
    {
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

}
