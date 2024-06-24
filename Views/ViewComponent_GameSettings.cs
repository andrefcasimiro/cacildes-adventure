using System;
using System.Collections;
using AF.Music;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace AF
{
    public class ViewComponent_GameSettings : MonoBehaviour
    {
        [Header("Components")]
        public BGMManager bgmManager;

        public UnityAction onLanguageChanged;

        UIDocumentPlayerHUDV2 uiDocumentPlayerHUDV2;
        public readonly string gameLanguageLabel = "GameLanguage";
        public readonly string graphicsQualityLabel = "GraphicsQuality";
        public readonly string cameraSensitivityLabel = "CameraSensitivity";
        public readonly string musicVolumeLabel = "MusicVolume";

        [Header("Key Rebinding")]
        VisualElement pressAnyKeyModal;
        VisualElement KeyBindingsList;
        ScrollView keyBindingScroll;
        public StarterAssetsInputs inputs;

        public VisualTreeAsset overrideButtonInputPrefab;

        public Soundbank soundbank;

        [Header("Databases")]
        public GameSettings gameSettings;

        [Header("Localization")]
        public LocalizedString CameraSensitivityLabel; // Camera Sensitivity ({0})
        public LocalizedString MusicVolumeLabel; // Music Volume ({0})

        public LocalizedString JumpLabel; // Jump
        public LocalizedString ToggleCombatStanceLabel; // Toggle Combat Stance
        public LocalizedString HeavyAttackLabel; // Heavy Attack
        public LocalizedString DodgeLabel; // Dodge Attack
        public LocalizedString SprintLabel; // Sprint
        public LocalizedString Action; // Action
        public LocalizedString Customize; // Customize


        public void SetupRefs(VisualElement root)
        {
            if (uiDocumentPlayerHUDV2 == null)
            {
                uiDocumentPlayerHUDV2 = FindAnyObjectByType<UIDocumentPlayerHUDV2>(FindObjectsInactive.Include);
            }

            UIUtils.SetupButton(root.Q<Button>("ResetSettings"), () =>
            {
                gameSettings.ResetSettings();
                UpdateUI(root);
            }, soundbank);

            keyBindingScroll = root.Q<ScrollView>("KeyBindingScrolls");

            UpdateUI(root);
        }

        void UpdateUI(VisualElement root)
        {
            RadioButtonGroup gameLanguageOptions = root.Q<RadioButtonGroup>(gameLanguageLabel);
            gameLanguageOptions.value = LocalizationSettings.SelectedLocale.Identifier == "en" ? 0 : 1;
            gameLanguageOptions.Focus();

            gameLanguageOptions.RegisterValueChangedCallback(ev =>
            {
                if (ev.newValue == 0)
                {
                    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("en");
                }
                else
                {
                    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("pt");
                }
            });



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
                cameraSensitivity.label = String.Format(CameraSensitivityLabel.GetLocalizedString(), ev.newValue);
            });
            cameraSensitivity.lowValue = gameSettings.minimumMouseSensitivity;
            cameraSensitivity.highValue = gameSettings.maximumMouseSensitivity;
            cameraSensitivity.value = gameSettings.GetMouseSensitivity();
            cameraSensitivity.label = String.Format(CameraSensitivityLabel.GetLocalizedString(), gameSettings.GetMouseSensitivity());

            musicVolumeSlider.RegisterValueChangedCallback(ev =>
            {
                gameSettings.SetMusicVolume(ev.newValue);
                musicVolumeSlider.label = String.Format(MusicVolumeLabel.GetLocalizedString(), ev.newValue);
            });
            musicVolumeSlider.lowValue = 0f;
            musicVolumeSlider.highValue = 1f;
            musicVolumeSlider.value = gameSettings.GetMusicVolume();
            musicVolumeSlider.label = String.Format(MusicVolumeLabel.GetLocalizedString(), gameSettings.GetMusicVolume());

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

                if (label == "Jump")
                {
                    label = JumpLabel.GetLocalizedString();
                }
                else if (label == "Dodge")
                {
                    label = DodgeLabel.GetLocalizedString();
                }
                else if (label == "HeavyAttack")
                {
                    label = HeavyAttackLabel.GetLocalizedString();
                }
                else if (label == "Tab")
                {
                    label = ToggleCombatStanceLabel.GetLocalizedString();
                }
                else if (label == "Sprint")
                {
                    label = SprintLabel.GetLocalizedString();
                }

                actionInputOverride.Q<Label>("Label").text = $"{Action.GetLocalizedString()} {label}";

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
            customizeBtn.text = Customize.GetLocalizedString();

            UIUtils.SetupButton(customizeBtn, () =>
            {
                onClickAction?.Invoke();
            }, soundbank);

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
