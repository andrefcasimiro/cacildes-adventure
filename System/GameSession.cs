using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSession", menuName = "System/New Game Session", order = 0)]
public class GameSession : ScriptableObject
{
    public bool hasShownTitleScreen = false;

    public bool shouldClearOnExitPlayMode = false;

    private void OnEnable()
    {
        // No need to populate the list; it's serialized directly
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode && shouldClearOnExitPlayMode)
        {
            // Clear the list when exiting play mode
            Clear();
        }
    }

    void Clear()
    {
        hasShownTitleScreen = false;
    }

}
