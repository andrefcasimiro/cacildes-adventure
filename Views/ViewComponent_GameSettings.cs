using System.Collections;
using AF.Music;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace AF
{
    public class ViewComponent_GameSettings : MonoBehaviour
    {
        [Header("Components")]
        public BGMManager bgmManager;

        public UnityAction onLanguageChanged;

        UIDocumentPlayerHUDV2 uiDocumentPlayerHUDV2;
        public readonly string graphicsQualityLabel = "GraphicsQuality";
        public readonly string cameraSensitivityLabel = "CameraSensitivity";
        public readonly string musicVolumeLabel = "MusicVolume";

        [Header("Key Rebinding")]
        VisualElement pressAnyKeyModal;
        VisualElement KeyBindingsList;
        ScrollView keyBindingScroll;
        public StarterAssetsInputs inputs;

        public VisualTreeAsset overrideButtonInputPrefab;


        [Header("Databases")]
        public GameSettings gameSettings;

        public void SetupRefs(VisualElement root)
        {
            if (uiDocumentPlayerHUDV2 == null)
            {
                uiDocumentPlayerHUDV2 = FindAnyObjectByType<UIDocumentPlayerHUDV2>(FindObjectsInactive.Include);
            }

            root.Q<Button>("ResetSettings").RegisterCallback<ClickEvent>(ev =>
            {
                gameSettings.ResetSettings();
                UpdateUI(root);
            });

            keyBindingScroll = root.Q<ScrollView>("KeyBindingScrolls");

            UpdateUI(root);
        }

        void UpdateUI(VisualElement root)
        {

            RadioButtonGroup graphicsOptions = root.Q<RadioButtonGroup>(graphicsQualityLabel);
            Slider cameraSensitivity = root.Q<Slider>(cameraSensitivityLabel);
            Slider musicVolumeSlider = root.Q<Slider>(musicVolumeLabel);

            graphicsOptions.value = gameSettings.GetGraphicsQuality();
            graphicsOptions.Focus();

            graphicsOptions.RegisterValueChangedCallback(ev =>
            {
                gameSettings.SetGameQuality(ev.newValue);
            });

            cameraSensitivity.RegisterValueChangedCallback(ev =>
            {
                gameSettings.SetCameraSensitivity(ev.newValue);
                cameraSensitivity.label = $"Camera Sensitivity ({ev.newValue})";
            });
            cameraSensitivity.lowValue = gameSettings.minimumMouseSensitivity;
            cameraSensitivity.highValue = gameSettings.maximumMouseSensitivity;
            cameraSensitivity.value = gameSettings.GetMouseSensitivity();
            cameraSensitivity.label = $"Camera Sensitivity ({gameSettings.GetMouseSensitivity()})";

            musicVolumeSlider.RegisterValueChangedCallback(ev =>
            {
                gameSettings.SetMusicVolume(ev.newValue);
                musicVolumeSlider.label = $"Music Volume ({ev.newValue})";
            });
            musicVolumeSlider.lowValue = 0f;
            musicVolumeSlider.highValue = 1f;
            musicVolumeSlider.value = gameSettings.GetMusicVolume();
            musicVolumeSlider.label = $"Music Volume ({gameSettings.GetMusicVolume()})";

            SetupKeyRebindingRefs(root);
        }

        void SetupKeyRebindingRefs(VisualElement root)
        {
            RadioButtonGroup useDefaultOrCustomizeRadioButtonGroup = root.Q<RadioButtonGroup>("UseDefaultOrCustomize");

            useDefaultOrCustomizeRadioButtonGroup.value = gameSettings.UseCustomInputs() ? 1 : 0;
            useDefaultOrCustomizeRadioButtonGroup.SetEnabled(Gamepad.current == null);

            useDefaultOrCustomizeRadioButtonGroup.RegisterValueChangedCallback(ev =>
            {
                gameSettings.SetUseCustomInputs(ev.newValue == 1 ? "true" : "false");

                if (ev.newValue == 0)
                {
                    DisableKeyBindings();
                    inputs.RestoreDefaultKeyBindings();
                }
                else
                {
                    EnableKeyBindings();
                }

                UpdateKeyBindingsUI(root);
            });

            pressAnyKeyModal = root.Q<VisualElement>("PressAnyKeyModal");
            pressAnyKeyModal.style.display = DisplayStyle.None;

            KeyBindingsList = root.Q<VisualElement>("KeyBindingsList");

            if (gameSettings.UseCustomInputs() && Gamepad.current == null)
            {
                EnableKeyBindings();
            }
            else
            {
                DisableKeyBindings();
            }

            UpdateKeyBindingsUI(root);
        }

        void EnableKeyBindings()
        {
            KeyBindingsList.style.opacity = 1f;
        }
        void DisableKeyBindings()
        {
            KeyBindingsList.style.opacity = 0.25f;
        }

        void UpdateKeyBindingsUI(VisualElement root)
        {
            keyBindingScroll.Clear();

            // Define an array of action names
            string[] actionNames = { "Jump", "Dodge", "HeavyAttack", "Tab", "Sprint" };

            // Loop through each action and create input overrides
            foreach (string actionName in actionNames)
            {
                VisualElement actionInputOverride = overrideButtonInputPrefab.CloneTree();

                string label = actionName;
                if (label == "Tab")
                {
                    label = "Toggle Weapon Stance";
                }
                else if (label == "HeavyAttack")
                {
                    label = "Heavy Attack";
                }

                actionInputOverride.Q<Label>("Label").text = $"{label} Action";

                DrawKeyBinding(actionInputOverride, inputs.GetCurrentKeyBindingForAction(actionName), () =>
                {
                    StartCoroutine(SelectKeyBinding(actionName, (bindingPayload) =>
                    {
                        switch (actionName)
                        {
                            case "Jump":
                                gameSettings.SetJumpOverrideBindingPayload(bindingPayload);
                                break;
                            case "Dodge":
                                gameSettings.SetDodgeOverrideBindingPayload(bindingPayload);
                                break;
                            case "HeavyAttack":
                                gameSettings.SetHeavyAttackOverrideBindingPayload(bindingPayload);
                                break;
                            case "Tab":
                                gameSettings.SetTwoHandModeOverrideBindingPayload(bindingPayload);
                                break;
                            case "Sprint":
                                gameSettings.SetSprintOverrideBindingPayload(bindingPayload);
                                break;
                                // Add cases for other actions as needed
                        }
                    }, () =>
                    {
                        UpdateKeyBindingsUI(root);
                    }));
                });

                keyBindingScroll.Add(actionInputOverride);
            }
        }


        void DrawKeyBinding(VisualElement root, string key, UnityAction onClickAction)
        {
            root.Q<Label>("Value").text = key;

            Button customizeBtn = root.Q<Button>("CustomizeButton");

            customizeBtn.RegisterCallback<ClickEvent>((ev) =>
            {
                onClickAction.Invoke();
            });

            customizeBtn.SetEnabled(gameSettings.UseCustomInputs());
        }

        IEnumerator SelectKeyBinding(string actionName, UnityAction<string> onRebindSuccessPayload, UnityAction onFinish)
        {
            pressAnyKeyModal.style.display = DisplayStyle.Flex;
            yield return inputs.Rebind(actionName, (action) =>
            {
                onRebindSuccessPayload.Invoke(action);
            });
            pressAnyKeyModal.style.display = DisplayStyle.None;
            onFinish?.Invoke();
        }
    }
}
