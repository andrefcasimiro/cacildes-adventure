using AF.Events;
using TigerForge;
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

    [Header("Max Attributes")]
    public int defaultMaxHealth = 300;
    public int maxHealth = 300;
    public float levelMultiplierForHealth = 2.25f;
    public int defaultMaxStamina = 150;
    public int maxStamina = 150;
    public float levelMultiplierForStamina = 3.25f;
    public int defaultMaxMana = 100;
    public int maxMana = 100;
    public float levelMultiplierForMana = 3.25f;


    [Header("Current Attributes")]

    public float currentHealth = -1;
    public float currentStamina = -1;
    public float currentMana = -1;

    public int reputation = 1;
    public int gold = 0;

    [Header("Lost Gold On Death")]
    public int lostGold = -1;
    public string sceneWhereGoldWasLost = "";
    public Vector3 positionWhereGoldWasLost = Vector3.zero;

#if UNITY_EDITOR

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
            Clear(false);
        }
    }
#endif

    public void Clear(bool isFromGameOver)
    {
        vitality = 1;
        endurance = 1;
        strength = 1;
        dexterity = 1;
        intelligence = 1;

        maxHealth = defaultMaxHealth;
        maxStamina = defaultMaxStamina;
        maxMana = defaultMaxMana;

        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentMana = maxMana;

        reputation = 1;
        gold = 0;

        if (!isFromGameOver)
        {
            lostGold = -1;
            sceneWhereGoldWasLost = "";
            positionWhereGoldWasLost = Vector3.zero;
        }
    }

    public void ClearForNewGamePlus()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentMana = maxMana;
        gold = 0;
        lostGold = -1;
        sceneWhereGoldWasLost = "";
        positionWhereGoldWasLost = Vector3.zero;
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
        return this.lostGold >= 0;
    }

    public void ClearLostGold()
    {
        this.lostGold = -1;
        this.positionWhereGoldWasLost = Vector3.zero;
        this.sceneWhereGoldWasLost = "";
    }

    /// <summary>
    /// Unity Event
    /// </summary>
    public void ResetReputation()
    {
        this.reputation = 0;
        EventManager.EmitEvent(EventMessages.ON_REPUTATION_CHANGED);
    }

    public void IncreaseReputation(int value)
    {
        this.reputation += value;
        EventManager.EmitEvent(EventMessages.ON_REPUTATION_CHANGED);
    }

    public void DecreaseReputation(int value)
    {
        this.reputation -= value;
        EventManager.EmitEvent(EventMessages.ON_REPUTATION_CHANGED);
    }

    public void SetStrength(int value)
    {
        this.strength = value;
    }
    public void SetDexterity(int value)
    {
        this.dexterity = value;
    }
    public void SetIntelligence(int value)
    {
        this.intelligence = value;
    }
    public void IncreaseStrength(int value)
    {
        this.strength += value;
    }
    public void IncreaseDexterity(int value)
    {
        this.dexterity += value;
    }
    public void IncreaseIntelligence(int value)
    {
        this.intelligence += value;
    }
}
