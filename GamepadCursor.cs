using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
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

        float screenHeight;
        float screenWidth;

        public PanelSettings panelSettings;

        public Vector2 previousPosition;

        public float mouseSize = 25;
        public float padding = 0.5f;

        private void OnEnable()
        {
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

            InputState.Change(virtualMouse.position, previousPosition);

            InputUser.PerformPairingWithDevice(virtualMouse, playerInput.user);

            AnchorCursor(virtualMouse.position.ReadValue());

            InputSystem.onAfterUpdate += UpdateMotion;
        }

        private void OnDisable()
        {
            InputSystem.RemoveDevice(virtualMouse);
            InputSystem.onAfterUpdate -= UpdateMotion;
        }

        private void Update()
        {
            UnityEngine.Cursor.visible = false;
            uIDocument.sortingOrder = Random.RandomRange(999, 9999);

            if (Gamepad.current == null)
            {
                UnityEngine.Cursor.visible = true;
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
            newPosition.x = Mathf.Clamp(newPosition.x, 0f, GetScreen().x - 25);
            newPosition.y = Mathf.Clamp(newPosition.y, 0f, GetScreen().y - 25);

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

            AnchorCursor(newPosition);  
        }

        public Vector2 GetScreen()
        {
            return new Vector2(uIDocument.rootVisualElement.resolvedStyle.width, uIDocument.rootVisualElement.resolvedStyle.height);
        }

        private void AnchorCursor(Vector2 position)
        {
            mouseGUI.style.translate = new StyleTranslate(new Translate(position.x, GetScreen().y - 25 - position.y, 0));
        }

        public float GetMouseSize()
        {
            return mouseSize - padding;
        }

    }

}