using System.Collections.Generic;
using AF;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Bonfires Database", menuName = "System/New Bonfires Database", order = 0)]
public class BonfiresDatabase : ScriptableObject
{
    public List<string> unlockedBonfires = new();

    private void OnEnable()
    {
        // No need to populate the list; it's serialized directly
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            // Clear the list when exiting play mode
            Clear();
        }
    }

    public void Clear()
    {
        unlockedBonfires.Clear();
    }

}
