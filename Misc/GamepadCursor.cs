    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;
    using UnityEngine.InputSystem.LowLevel;
    using UnityEngine.InputSystem.Users;
    using UnityEngine.UIElements;
    using MouseButton = UnityEngine.InputSystem.LowLevel.MouseButton;

    namespace AF
    {
        public class GamepadCursor : MonoBehaviour
        {
            public Mouse virtualMouse;

            public PlayerInput playerInput;

            [SerializeField]
            private float cursorSpeed = 1000f;

            [HideInInspector] public bool previousMouseState;

            // UI
            UIDocument uIDocument;

            [HideInInspector] public VisualElement root;
            [HideInInspector] public VisualElement cursorContainer;
            public IMGUIContainer mouseGUI;

            public PanelSettings panelSettings;

            public Vector2 previousPosition;

            public float cursorSize = 25;
            public float padding = 0.5f;

            StarterAssetsInputs inputs;

            private void Awake()
            {
                uIDocument = GetComponent<UIDocument>();
                inputs = FindObjectOfType<StarterAssetsInputs>(true);
            }

            private void OnEnable()
            {
                InitializeVirtualMouse();

                EventSystem.current.SetSelectedGameObject(null);

                if (Gamepad.current == null)
                {
                    return;
                }

                SetupUIElements();

                InputSystem.onAfterUpdate += UpdateCursorMotion;
            }

            public void OnDisable()
            {
                CleanUpVirtualMouse();
            }

            private void OnDestroy()
            {
                CleanUpVirtualMouse();
            }

            public void InitializeVirtualMouse()
            {
                CleanUpVirtualMouse();

                if (Gamepad.current == null)
                {
                    return;
                }

                this.root = uIDocument.rootVisualElement;
                SetupUIElements();

                virtualMouse = (Mouse)InputSystem.AddDevice("VirtualMouse");
                Vector2 initialPos = new Vector2(Screen.width / 2, Screen.height / 2);
                previousPosition = initialPos;
                InputState.Change(virtualMouse.position, previousPosition);
                InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);
            }

            public void CleanUpVirtualMouse()
            {
                if (virtualMouse != null)
                {
                    InputSystem.RemoveDevice(virtualMouse);
                    InputSystem.onAfterUpdate -= UpdateCursorMotion;
                    virtualMouse = null;
                }
            }

            public void SetupUIElements()
            {
                if (this.root == null)
                {
                    return;
                }

                this.root.pickingMode = PickingMode.Ignore;
                this.root.focusable = false;

                this.cursorContainer = root.Q<VisualElement>("CursorContainer");
                cursorContainer.pickingMode = PickingMode.Ignore;
                cursorContainer.focusable = false;

                mouseGUI = this.root.Q<IMGUIContainer>("CursorImage");
                mouseGUI.pickingMode = PickingMode.Ignore;
                mouseGUI.focusable = false;
            }

            private void Update()
            {
                if (Gamepad.current != null)
                {
                    uIDocument.sortingOrder = Random.Range(999, 9999);
                    EventSystem.current.SetSelectedGameObject(null);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }

            public void UpdateCursorMotion()
            {
                if (virtualMouse == null || Gamepad.current == null)
                {
                    return;
                }

                Vector2 deltaValue = Gamepad.current.rightStick.ReadValue();
                deltaValue *= cursorSpeed * Time.deltaTime;

                Vector2 currentPosition = virtualMouse.position.ReadValue();
                Vector2 newPosition = currentPosition + deltaValue;

                ClampCursorPosition(currentPosition, ref newPosition);

                InputState.Change(virtualMouse.position, newPosition);
                InputState.Change(virtualMouse.delta, deltaValue);

                previousPosition = newPosition;

                HandleButtonSouthInput();

                newPosition *= ResolveScale(GetDisplayRect(), Screen.dpi);
                AnchorCursor(newPosition);
            }

            public void ClampCursorPosition(Vector2 currentPosition, ref Vector2 newPosition)
            {
                newPosition.x = Mathf.Clamp(newPosition.x, 0f, Screen.width - cursorSize);
                newPosition.y = Mathf.Clamp(newPosition.y, 0f, Screen.height - cursorSize);
            }

            public void HandleButtonSouthInput()
            {
                bool buttonSouthIsPressed = Gamepad.current.buttonSouth.IsPressed();

                if (previousMouseState != buttonSouthIsPressed)
                {
                    virtualMouse.CopyState<MouseState>(out var mouseState);

                    mouseState.WithButton(MouseButton.Left, buttonSouthIsPressed);
                    InputState.Change(virtualMouse, mouseState);
                    Debug.Log("Button South Pressed: " + buttonSouthIsPressed);
                    previousMouseState = buttonSouthIsPressed;
                }
            }

            public void AnchorCursor(Vector2 position)
            {
                mouseGUI.style.translate = new StyleTranslate(new Translate(position.x, GetScreen().y - GetOffset() - position.y, 0));
            }

            public float GetOffset()
            {
                return cursorSize * ResolveScale(GetDisplayRect(), Screen.dpi);
            }

            public Vector2 GetScreen()
            {
                return new Vector2(this.root.resolvedStyle.width, this.root.resolvedStyle.height);
            }

            private Rect GetDisplayRect()
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

            private float ResolveScale(Rect targetRect, float screenDpi)
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
