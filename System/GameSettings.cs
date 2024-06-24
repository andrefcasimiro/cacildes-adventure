using AF;
using AF.Events;
using TigerForge;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Extensions;
using UnityEngine.Localization.SmartFormat.GlobalVariables;

[CreateAssetMenu(fileName = "GameSettings", menuName = "System/New Game Settings", order = 0)]
public class GameSettings : ScriptableObject
{
    public bool developerModeActive = false;

    public bool hasInitializedSettings = false;

    public float minimumMouseSensitivity = 0f;
    public float maximumMouseSensitivity = 10f;

    public enum GraphicsQuality { LOW, MEDIUM, GOOD, ULTRA };

    public readonly string GRAPHICS_QUALITY_KEY = "graphicsQuality";
    public readonly string MUSIC_VOLUME_KEY = "musicVolume";
    public readonly string MOUSE_SENSITIVITY_KEY = "mouseSensitivity";
    public readonly string USE_CUSTOM_INPUTS_KEY = "useCustomInputs";

    public readonly string JUMP_OVERRIDE_BINDING_PAYLOAD_KEY = "JUMP_OVERRIDE_BINDING_PAYLOAD_KEY";
    public readonly string DODGE_OVERRIDE_BINDING_PAYLOAD_KEY = "DODGE_OVERRIDE_BINDING_PAYLOAD_KEY";
    public readonly string HEAVY_ATTACK_OVERRIDE_BINDING_PAYLOAD_KEY = "HEAVY_ATTACK_OVERRIDE_BINDING_PAYLOAD_KEY";
    public readonly string TWO_HAND_MODE_OVERRIDE_BINDING_PAYLOAD_KEY = "TWO_HAND_MODE_OVERRIDE_BINDING_PAYLOAD_KEY";
    public readonly string SPRINT_OVERRIDE_BINDING_PAYLOAD_KEY = "SPRINT_OVERRIDE_BINDING_PAYLOAD_KEY";

    public string characterName;
    public readonly string PLAYER_NAME_KEY = "PLAYER_NAME";
    public readonly string defaultPlayerName = "Cacildes";


#if UNITY_EDITOR
    private void OnEnable()
    {
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
        hasInitializedSettings = false;
    }

    public void UpdatePlayerNameOnLocalizedAssets()
    {
        // Get our GlobalVariablesSource
        var source = LocalizationSettings
            .StringDatabase
            .SmartFormatter
            .GetSourceExtension<PersistentVariablesSource>();
        // Get the specific global variable
        var characterName =
            source["global"]["playerName"] as UnityEngine.Localization.SmartFormat.PersistentVariables.StringVariable;
        // Update the global variable
        characterName.Value = GetPlayerName();
    }


    public string GetPlayerName()
    {
        if (!PlayerPrefs.HasKey(PLAYER_NAME_KEY))
        {
            return defaultPlayerName;
        }

        return PlayerPrefs.GetString(PLAYER_NAME_KEY);
    }

    public void SetPlayerName(string playerName)
    {

        PlayerPrefs.SetString(PLAYER_NAME_KEY, playerName);
        PlayerPrefs.Save();
        UpdatePlayerNameOnLocalizedAssets();
    }

    public void ResetSettings()
    {
        SetGameQuality(2);
        SetCameraSensitivity(1f);
        SetMusicVolume(1f);
        SetUseCustomInputs("false");
        SetJumpOverrideBindingPayload("");
        SetDodgeOverrideBindingPayload("");
        SetHeavyAttackOverrideBindingPayload("");
        SetTwoHandModeOverrideBindingPayload("");
        SetSprintOverrideBindingPayload("");
    }

    public void LoadSettings(StarterAssetsInputs starterAssetsInputs)
    {
        if (hasInitializedSettings)
        {
            return;
        }

        hasInitializedSettings = true;
        SetGameQuality(GetGraphicsQuality());
        SetInputOverrides(starterAssetsInputs);
    }

    public void SetGameQuality(int newValue)
    {
        PlayerPrefs.SetInt(GRAPHICS_QUALITY_KEY, Mathf.Clamp(newValue, 0, 4));

        if (newValue == 0)
        {
            QualitySettings.SetQualityLevel(0);
        }
        else if (newValue == 1)
        {
            QualitySettings.SetQualityLevel(2);
        }
        else if (newValue == 2)
        {
            QualitySettings.SetQualityLevel(4);
        }
        else if (newValue == 3)
        {
            QualitySettings.SetQualityLevel(5);
        }

        EventManager.EmitEvent(EventMessages.ON_GRAPHICS_QUALITY_CHANGED);
    }

    public void SetCameraSensitivity(float newValue)
    {
        PlayerPrefs.SetFloat(MOUSE_SENSITIVITY_KEY, newValue);
    }
    public void SetMusicVolume(float newValue)
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, newValue);
        EventManager.EmitEvent(EventMessages.ON_MUSIC_VOLUME_CHANGED);
    }

    public void SetUseCustomInputs(string newValue)
    {
        PlayerPrefs.SetString(USE_CUSTOM_INPUTS_KEY, newValue);

        EventManager.EmitEvent(EventMessages.ON_USE_CUSTOM_INPUT_CHANGED);
    }

    public bool UseCustomInputs()
    {
        return PlayerPrefs.HasKey(USE_CUSTOM_INPUTS_KEY) ? PlayerPrefs.GetString(USE_CUSTOM_INPUTS_KEY) == "true" : false;
    }

    public int GetGraphicsQuality()
    {
        return PlayerPrefs.HasKey(GRAPHICS_QUALITY_KEY) ? PlayerPrefs.GetInt(GRAPHICS_QUALITY_KEY) : 3;
    }
    public float GetMouseSensitivity()
    {
        return PlayerPrefs.HasKey(MOUSE_SENSITIVITY_KEY) ? PlayerPrefs.GetFloat(MOUSE_SENSITIVITY_KEY) : 1f;
    }

    public float GetMusicVolume()
    {
        return PlayerPrefs.HasKey(MUSIC_VOLUME_KEY) ? PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY) : 1f;
    }

    void SetInputOverrides(StarterAssetsInputs starterAssetsInputs)
    {
        if (string.IsNullOrEmpty(GetJumpOverrideBindingPayload()) == false)
        {
            starterAssetsInputs.ApplyBindingOverride("Jump", GetJumpOverrideBindingPayload());
        }

        if (string.IsNullOrEmpty(GetDodgeOverrideBindingPayload()) == false)
        {
            starterAssetsInputs.ApplyBindingOverride("Dodge", GetDodgeOverrideBindingPayload());
        }

        if (string.IsNullOrEmpty(GetHeavyAttackOverrideBindingPayload()) == false)
        {
            starterAssetsInputs.ApplyBindingOverride("HeavyAttack", GetHeavyAttackOverrideBindingPayload());
        }

        if (string.IsNullOrEmpty(GetTwoHandModeOverrideBindingPayload()) == false)
        {
            starterAssetsInputs.ApplyBindingOverride("Tab", GetTwoHandModeOverrideBindingPayload());
        }

        if (string.IsNullOrEmpty(GetSprintOverrideBindingPayload()) == false)
        {
            starterAssetsInputs.ApplyBindingOverride("Sprint", GetSprintOverrideBindingPayload());
        }
    }

    public string GetJumpOverrideBindingPayload()
    {
        return PlayerPrefs.HasKey(JUMP_OVERRIDE_BINDING_PAYLOAD_KEY) ? PlayerPrefs.GetString(JUMP_OVERRIDE_BINDING_PAYLOAD_KEY) : "";
    }

    public void SetJumpOverrideBindingPayload(string newValue)
    {
        PlayerPrefs.SetString(JUMP_OVERRIDE_BINDING_PAYLOAD_KEY, newValue);
    }

    public string GetDodgeOverrideBindingPayload()
    {
        return PlayerPrefs.HasKey(DODGE_OVERRIDE_BINDING_PAYLOAD_KEY) ? PlayerPrefs.GetString(DODGE_OVERRIDE_BINDING_PAYLOAD_KEY) : "";
    }

    public void SetDodgeOverrideBindingPayload(string newValue)
    {
        PlayerPrefs.SetString(DODGE_OVERRIDE_BINDING_PAYLOAD_KEY, newValue);
    }

    public string GetHeavyAttackOverrideBindingPayload()
    {
        return PlayerPrefs.HasKey(HEAVY_ATTACK_OVERRIDE_BINDING_PAYLOAD_KEY) ? PlayerPrefs.GetString(HEAVY_ATTACK_OVERRIDE_BINDING_PAYLOAD_KEY) : "";
    }

    public void SetHeavyAttackOverrideBindingPayload(string newValue)
    {
        PlayerPrefs.SetString(HEAVY_ATTACK_OVERRIDE_BINDING_PAYLOAD_KEY, newValue);
    }

    public string GetTwoHandModeOverrideBindingPayload()
    {
        return PlayerPrefs.HasKey(TWO_HAND_MODE_OVERRIDE_BINDING_PAYLOAD_KEY) ? PlayerPrefs.GetString(TWO_HAND_MODE_OVERRIDE_BINDING_PAYLOAD_KEY) : "";
    }

    public void SetTwoHandModeOverrideBindingPayload(string newValue)
    {
        PlayerPrefs.SetString(TWO_HAND_MODE_OVERRIDE_BINDING_PAYLOAD_KEY, newValue);
    }

    public string GetSprintOverrideBindingPayload()
    {
        return PlayerPrefs.HasKey(SPRINT_OVERRIDE_BINDING_PAYLOAD_KEY) ? PlayerPrefs.GetString(SPRINT_OVERRIDE_BINDING_PAYLOAD_KEY) : "";
    }

    public void SetSprintOverrideBindingPayload(string newValue)
    {
        PlayerPrefs.SetString(SPRINT_OVERRIDE_BINDING_PAYLOAD_KEY, newValue);
    }
}
