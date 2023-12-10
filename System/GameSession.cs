using AF;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSession", menuName = "System/New Game Session", order = 0)]
public class GameSession : ScriptableObject
{
    public bool hasShownTitleScreen = false;


    [Header("Teleport")]
    public string nextMap_SpawnGameObjectName;

    [Header("Debugging")]
    public bool rememberPlayerPosition = false;
    public Vector3 lastPlayerPosition;

    public bool shouldClearOnExitPlayMode = false;

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            if (rememberPlayerPosition)
            {
                lastPlayerPosition = GameObject.FindWithTag("Player").transform.position;
            }

            if (shouldClearOnExitPlayMode)
            {
                Clear();

            }
        }
    }

    void Clear()
    {
        hasShownTitleScreen = false;
    }

}
