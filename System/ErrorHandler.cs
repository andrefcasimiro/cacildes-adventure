
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace AF
{
    [RequireComponent(typeof(UIDocument))]
    public class ErrorHandler : MonoBehaviour
    {
        UIDocument uIDocument => GetComponent<UIDocument>();
        public VisualTreeAsset errorNotificationPrefab;

        VisualElement root;
        VisualElement errorMessagesContainer;

        public SaveManager saveManager;
        public CursorManager cursorManager;

        public Dictionary<string, string> errors = new();

        private List<string> errorMessagesToIgnore = new()
        {
            "has no receiver! Are you missing a component?",
            "AnimationEvent has no function name specified!",
            "PhysX does not support concave Mesh Colliders with dynamic Rigidbody GameObjects.",
            "The variable fadeMaterial of",
            "can only be called on an active agent that has been placed on a NavMesh.",
            "The variable transformRef of FootstepReceiver has not been assigned",
            "Coroutine couldn't be started because the the game object",
            "cannot be converted to type 'AF.State'.",
            "[Steamworks.NET] SteamAPI_Init() failed.",
            "AF.Combat.TargetManager.ClearTarget ()"
        };

        void Start()
        {
            // Subscribe to the log message received event
            Application.logMessageReceived += HandleLog;

            this.root = uIDocument.rootVisualElement;
            errorMessagesContainer = root.Q<VisualElement>("ErrorMessagesContainer");
            errorMessagesContainer.Clear();
            CloseErrorPanel();
        }

        void OnDestroy()
        {
            // Unsubscribe from the log message received event
            Application.logMessageReceived -= HandleLog;
        }

        void DisplayErrorEntry(string errorMessage, string stackTraceMessage)
        {
            if (errorNotificationPrefab == null)
            {
                return;
            }

            VisualElement entry = errorNotificationPrefab.CloneTree();
            Button reloadLastSave, copyError, sendEmail, closeButton;

            entry.Q<Label>("ErrorName").text = errorMessage;
            entry.Q<Label>("StackTrace").text = stackTraceMessage;

            reloadLastSave = entry.Q<Button>("ReloadLastSave");
            copyError = entry.Q<Button>("CopyError");
            sendEmail = entry.Q<Button>("SendEmail");
            closeButton = entry.Q<Button>("CloseButton");

            reloadLastSave.RegisterCallback<ClickEvent>(ev =>
            {
                saveManager?.LoadLastSavedGame(false);
            });

            reloadLastSave.Focus();

            copyError.RegisterCallback<ClickEvent>(ev =>
            {
                CopyToClipboard(errorMessage, stackTraceMessage);
            });

            sendEmail.RegisterCallback<ClickEvent>(ev =>
            {
                SendEmail(errorMessage, stackTraceMessage);
            });

            closeButton.RegisterCallback<ClickEvent>(ev =>
            {
                errorMessagesContainer.Remove(entry);
                CloseErrorPanel();
                cursorManager?.HideCursor();
            });

            UIUtils.PlayPopAnimation(entry, new Vector3(0.8f, 0.8f, 0.8f));

            errorMessagesContainer.Add(entry);
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            // Handle the log message based on its type
            switch (type)
            {
                case LogType.Error:
                case LogType.Exception:
                    // Display error panel with error message
                    DisplayErrorPanel(logString, stackTrace);
                    break;
            }
        }

        public void DisplayErrorPanel(string errorMessage, string stackTrace)
        {
            if (ShouldIgnore(errorMessage) || errors.ContainsKey(errorMessage))
            {
                return;
            }

            errors.Add(errorMessage, stackTrace);

            DisplayErrorEntry(errorMessage, stackTrace);

            if (root != null)
            {
                root.visible = true;
            }

            cursorManager?.ShowCursor();
        }

        public void CloseErrorPanel()
        {
            if (errorMessagesContainer.childCount > 0)
            {
                return;
            }

            // Close the error panel
            root.visible = false;
        }

        public void CopyToClipboard(string errorMessage, string stackTrace)
        {
            try
            {
                GUIUtility.systemCopyBuffer = "Error Name: " + errorMessage + "\n Stack Trace: " + stackTrace;

            }
            catch (Exception e)
            {
                Debug.LogException(new Exception("Failed to copy error to clipboard!\n" + e));
            }
        }

        public void SendEmail(string errorName, string stackTrace)
        {
            string email = "cacildesadventure@gmail.com";
            string subject = MyEscapeURL("Error Found!");
            string body = MyEscapeURL("Found this error:\r\n" + errorName + "\r\n\r\nStack Trace: " + stackTrace);

            try
            {
                Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);

            }
            catch (Exception e)
            {
                Debug.LogException(new Exception("Failed to send email!\n" + e));
            }
        }

        string MyEscapeURL(string URL)
        {
            return UnityWebRequest.EscapeURL(URL).Replace("+", "%20");
        }

        bool ShouldIgnore(string error)
        {
            foreach (string errorMessage in errorMessagesToIgnore)
            {
                if (error.Contains(errorMessage))
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasErrors()
        {
            return errorMessagesContainer != null && errorMessagesContainer.childCount > 0;
        }
    }

}
