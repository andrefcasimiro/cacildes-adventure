using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;
using UnityEngine.UIElements;
using MouseButton = UnityEngine.InputSystem.LowLevel.MouseButton;

namespace AF
{

    public class GamepadCursor : MonoBehaviour
    {

        private Mouse virtualMouse;

        [SerializeField]
        private PlayerInput playerInput;

        [SerializeField]
        private float cursorSpeed = 1000f;

        private bool previousMouseState;

        // UI

        UIDocument uIDocument => GetComponent<UIDocument>();

        VisualElement root;

        VisualElement cursorContainer;
        public IMGUIContainer mouseGUI;

        public PanelSettings panelSettings;

        public Vector2 previousPosition;

        public float mouseSize = 25;
        public float padding = 0.5f;


        private void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(null);

            if (Gamepad.current == null)
            {
                return;
            }

            this.root = uIDocument.rootVisualElement;

            this.root.pickingMode = PickingMode.Ignore;
            this.root.focusable = false;

            this.cursorContainer = root.Q<VisualElement>("CursorContainer");
            cursorContainer.pickingMode = PickingMode.Ignore;
            cursorContainer.focusable = false;

            mouseGUI = this.root.Q<IMGUIContainer>("CursorImage");
            mouseGUI.pickingMode = PickingMode.Ignore;
            mouseGUI.focusable = false;


            if (virtualMouse == null)
            {
                virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
            }
            else if (!virtualMouse.added)
            {
                InputSystem.AddDevice(virtualMouse);
            }


            Vector2 initialPos = new Vector2(Screen.width / 2, Screen.height / 2);

            previousPosition = initialPos;

            InputState.Change(virtualMouse.position, previousPosition);

            InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

            var scale = ResolveScale(GetDisplayRect(), Screen.dpi);
            AnchorCursor(virtualMouse.position.ReadValue() * scale);

            InputSystem.onAfterUpdate += UpdateMotion;

        }

        private void OnDisable()
        {
            if (virtualMouse != null)
            {
                InputSystem.RemoveDevice(virtualMouse);
                InputSystem.onAfterUpdate -= UpdateMotion;
            }
        }

        private void Update()
        {
            if (Gamepad.current != null)
            {
                uIDocument.sortingOrder = UnityEngine.Random.RandomRange(999, 9999);
                EventSystem.current.SetSelectedGameObject(null);
            }
            else
            {
                this.gameObject.SetActive(false);
            }
        }

        private void UpdateMotion()
        {
            if (virtualMouse == null || Gamepad.current == null)
            {
                return;
            }


            Vector2 deltaValue = Gamepad.current.rightStick.ReadValue();
            deltaValue *= cursorSpeed * Time.deltaTime;


            Vector2 currentPosition = virtualMouse.position.ReadValue();

            Vector2 newPosition = currentPosition + deltaValue;

            newPosition.x = Mathf.Clamp(newPosition.x, 0f, Screen.width - 25);
            newPosition.y = Mathf.Clamp(newPosition.y, 0f, Screen.height - 25);


            InputState.Change(virtualMouse.position, newPosition);
            InputState.Change(virtualMouse.delta, deltaValue);


            previousPosition = newPosition;


            bool buttonSouthIsPressed = Gamepad.current.buttonSouth.IsPressed();
            if (previousMouseState != buttonSouthIsPressed)
            {
                virtualMouse.CopyState<MouseState>(out var mouseState);

                mouseState.WithButton(MouseButton.Left, Gamepad.current.buttonSouth.IsPressed());

                previousMouseState = buttonSouthIsPressed;
            }

            newPosition *= ResolveScale(GetDisplayRect(), Screen.dpi);

            AnchorCursor(newPosition);
        }

        float GetOffset()
        {
            return 25 * ResolveScale(GetDisplayRect(), Screen.dpi);
        }

        public Vector2 GetScreen()
        {
            return new Vector2(this.root.resolvedStyle.width, this.root.resolvedStyle.height);
        }



        private void AnchorCursor(Vector2 position)
        {
            mouseGUI.style.translate = new StyleTranslate(new Translate(position.x, GetScreen().y - GetOffset() - position.y, 0));
        }

        public float GetMouseSize()
        {
            return mouseSize - padding;
        }

        internal Rect GetDisplayRect()
        {
            if (uIDocument.panelSettings.targetTexture != null)
            {
                return new Rect(0f, 0f, uIDocument.panelSettings.targetTexture.width, uIDocument.panelSettings.targetTexture.height);
            }

            if (uIDocument.panelSettings.targetDisplay > 0 && uIDocument.panelSettings.targetDisplay < Display.displays.Length)
            {
                return new Rect(0f, 0f, Display.displays[uIDocument.panelSettings.targetDisplay].renderingWidth, Display.displays[uIDocument.panelSettings.targetDisplay].renderingHeight);
            }

            return new Rect(0f, 0f, Screen.width, Screen.height);
        }

        float ResolveScale(Rect targetRect, float screenDpi)
        {
            float num = 1f;

            if (uIDocument.panelSettings.referenceResolution.x * uIDocument.panelSettings.referenceResolution.y != 0)
            {
                Vector2 vector = uIDocument.panelSettings.referenceResolution;
                Vector2 vector2 = new Vector2(targetRect.width / vector.x, targetRect.height / vector.y);
                float num2 = 0f;
                switch (uIDocument.panelSettings.screenMatchMode)
                {
                    case PanelScreenMatchMode.Expand:
                        num2 = Mathf.Min(vector2.x, vector2.y);
                        break;
                    case PanelScreenMatchMode.Shrink:
                        num2 = Mathf.Max(vector2.x, vector2.y);
                        break;
                    default:
                        {
                            float t = Mathf.Clamp01(uIDocument.panelSettings.match);
                            num2 = Mathf.Lerp(vector2.x, vector2.y, t);
                            break;
                        }
                }

                if (num2 != 0f)
                {
                    num = 1f / num2;
                }

                if (uIDocument.panelSettings.scale > 0f)
                {
                    return num / uIDocument.panelSettings.scale;
                }
            }

            return 0f;
        }
    }

}