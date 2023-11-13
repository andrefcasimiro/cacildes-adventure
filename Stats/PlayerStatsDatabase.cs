using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Stats Database", menuName = "System/New Player Stats Database", order = 0)]
public class PlayerStatsDatabase : ScriptableObject
{


    [Header("Stats")]
    public int vitality = 1;
    public int endurance = 1;
    public int strength = 1;
    public int dexterity = 1;
    public int intelligence = 1;

    [Header("Attributes")]
    public float currentStamina = 0;
    public int reputation = 1;
    public int gold = 0;

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
        vitality = 1;
        endurance = 1;
        strength = 1;
        dexterity = 1;
        intelligence = 1;
    }

    public int GetCurrentLevel()
    {
        return vitality + endurance + strength + dexterity + intelligence;
    }

    public int GetCurrentReputation()
    {
        return reputation;
    }
}
