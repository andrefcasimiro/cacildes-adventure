using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.Events;

namespace AF
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool toggleWalk;
		public bool dodge;
		public bool lightAttack;
		public UnityEvent onLightAttackInput;

		public bool block;
		public bool lockOn;
		public bool heavyAttack;
		public UnityEvent onHeavyAttackInput;

		public bool interact;
		public UnityEvent onInteract;
		public bool switchSpell;
		public UnityEvent onSwitchSpellInput;
		public bool switchWeapon;
		public UnityEvent onSwitchWeaponInput;

		public bool switchShield;
		public UnityEvent onSwitchShieldInput;

		public bool switchConsumable;
		public UnityEvent onSwitchConsumableInput;
		public bool consumeFavoriteItem;
		public bool quickSave;
		public bool quickLoad;
		public bool aim;

		[Header("UI")]
		public bool menu;
		public bool tab;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		CursorManager cursorManager;

		public UnityAction onMenuInput;
		public UnityAction onTabInput;

		public UnityAction onViewInGameControlsInput;


		private void Awake()
		{
			cursorManager = FindObjectOfType<CursorManager>(true);
		}


#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if (cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnToggleWalk(InputValue value)
		{
			ToggleWalkInput(value.isPressed);
		}

		public void OnDodge(InputValue value)
		{
			DodgeInput(value.isPressed);
		}

		public void OnMenu(InputValue value)
		{
			MenuInput(value.isPressed);

			if (value.isPressed)
			{
				onMenuInput.Invoke();
			}
		}

		public void OnTab(InputValue value)
		{
			TabInput(value.isPressed);

			if (value.isPressed)
			{
				// onViewInGameControlsInput.Invoke();
				onTabInput?.Invoke();
			}
		}

		public void OnLightAttack(InputValue value)
		{
			LightAttackInput(value.isPressed);

			onLightAttackInput?.Invoke();

		}

		public void OnBlock(InputValue value)
		{
			BlockInput(value.isPressed);
		}

		public void OnLockOn(InputValue value)
		{
			LockOnInput(value.isPressed);
		}

		public void OnHeavyAttack(InputValue value)
		{
			HeavyAttackInput(value.isPressed);
			onHeavyAttackInput?.Invoke();

		}

		public void OnInteract(InputValue value)
		{
			InteractInput(value.isPressed);

			if (value.isPressed)
			{
				onInteract?.Invoke();
			}
		}

		public void OnSwitchSpell(InputValue value)
		{
			if (value.isPressed)
			{
				onSwitchSpellInput?.Invoke();
			}
		}

		public void OnSwitchConsumable(InputValue value)
		{
			if (value.isPressed)
			{
				onSwitchConsumableInput?.Invoke();
			}
		}

		public void OnSwitchWeapon(InputValue value)
		{
			if (value.isPressed)
			{
				onSwitchWeaponInput?.Invoke();
			}
		}

		public void OnSwitchShield(InputValue value)
		{
			if (value.isPressed)
			{
				onSwitchShieldInput?.Invoke();
			}
		}

		public void OnConsumeFavoriteItem(InputValue value)
		{
			ConsumeFavoriteItemInput(value.isPressed);
		}

		public void OnQuickSave(InputValue value)
		{
			QuickSaveInput(value.isPressed);
		}
		public void OnQuickLoad(InputValue value)
		{
			QuickLoadInput(value.isPressed);
		}
#endif

		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void ToggleWalkInput(bool newToggleWalkState)
		{
			toggleWalk = !toggleWalk;
		}

		public void DodgeInput(bool newDodgeState)
		{
			dodge = newDodgeState;
		}

		public void MenuInput(bool newMenuState)
		{
			menu = newMenuState;
		}

		public void TabInput(bool state)
		{
			tab = state;
		}

		public void LightAttackInput(bool lightAttackState)
		{
			lightAttack = lightAttackState;
		}

		public void BlockInput(bool blockState)
		{
			block = blockState;
		}

		public void LockOnInput(bool lockOnState)
		{
			lockOn = lockOnState;
		}

		public void HeavyAttackInput(bool heavyAttackState)
		{
			heavyAttack = heavyAttackState;
		}

		public void InteractInput(bool interactState)
		{
			interact = interactState;
		}

		public void ConsumeFavoriteItemInput(bool state)
		{
			consumeFavoriteItem = state;
		}
		public void QuickSaveInput(bool state)
		{
			quickSave = state;
		}

		public void QuickLoadInput(bool state)
		{
			quickLoad = state;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			//SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

		private void Start()
		{

			InputSystem.onDeviceChange += (device, deviceChange) =>
			{
				// If cursor is not being shown or used for menus, dont do anything
				if (Cursor.lockState == CursorLockMode.Locked)
				{
					return;
				}

				var gamePadCursor = FindObjectOfType<GamepadCursor>(true);

				// If gamepad was disconnected
				if (Gamepad.current == null && gamePadCursor.isActiveAndEnabled)
				{
					gamePadCursor.gameObject.SetActive(false);
				}
				else if (!gamePadCursor.isActiveAndEnabled)
				{
					gamePadCursor.gameObject.SetActive(true);
				}

				cursorManager.ShowCursor();
			};
		}


	}

}