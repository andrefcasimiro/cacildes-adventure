using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public float currentHealth = 0;
    public float currentStamina = 0;
    public int reputation = 1;
    public int gold = 0;

    [Header("Lost Gold On Death")]
    public int lostGold = 0;
    public string sceneWhereGoldWasLost = "";
    public Vector3 positionWhereGoldWasLost = Vector3.zero;

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

        currentHealth = 0;
        currentStamina = 0;
        reputation = 1;
        gold = 0;
    }

    public int GetCurrentLevel()
    {
        return vitality + endurance + strength + dexterity + intelligence;
    }

    public int GetCurrentReputation()
    {
        return reputation;
    }

    public void SetLostGold(Vector3 deathPosition)
    {
        this.lostGold = this.gold;
        this.positionWhereGoldWasLost = deathPosition;
        this.sceneWhereGoldWasLost = SceneManager.GetActiveScene().name;
    }

    public bool HasLostGoldToRecover()
    {
        return this.lostGold > 0;
    }

    public void ClearLostGold()
    {
        this.lostGold = 0;
        this.positionWhereGoldWasLost = Vector3.zero;
        this.sceneWhereGoldWasLost = "";
    }
}
