using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using static UnityEngine.InputSystem.InputActionRebindingExtensions;
using System.Collections;

namespace AF
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public UnityEvent onMoveInput;

		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool toggleWalk;

		public bool consumeFavoriteItem;
		public UnityEvent onConsumeFavoriteItem;

		public UnityEvent onDodgeInput;
		public UnityEvent onLightAttackInput;

		public bool block;
		public UnityEvent onBlock_Start;
		public UnityEvent onBlock_End;

		public UnityEvent onLockOnInput;

		public UnityEvent onHeavyAttackInput;

		public bool interact;
		public UnityEvent onInteract;
		public UnityEvent onSwitchSpellInput;
		public UnityEvent onSwitchWeaponInput;

		public UnityEvent onSwitchShieldInput;

		public UnityEvent onSwitchConsumableInput;
		public UnityEvent onToggleTwoHandsInput;

		[Header("UI")]
		public UnityEvent onMenuEvent;
		public UnityEvent onCustomizeCharacter;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorInputForLook = true;

		[Header("Main Menu")]
		public UnityEvent onNextMenu;
		public UnityEvent onPreviousMenu;

		[Header("Day Night")]
		public UnityEvent onAdvanceOneHour;
		public UnityEvent onGoBackOneHour;

		[Header("System")]
		public GameSettings gameSettings;

		Vector2 scaleVector = new(1, 1);

		[Header("Rebindings")]
		public PlayerInput playerInput;

		[Header("Stats")]
		public IntStat useHotkeyToCustomizeCharacterStat;

		private void Awake()
		{
			scaleVector = new(gameSettings.GetMouseSensitivity(), gameSettings.GetMouseSensitivity());
		}

		public void OnMove(InputValue value)
		{
			move = value.Get<Vector2>();

			onMoveInput?.Invoke();
		}

		public void OnLook(InputValue value)
		{
			if (cursorInputForLook)
			{
				look = value.Get<Vector2>();
			}

			if (scaleVector.x != gameSettings.GetMouseSensitivity())
			{
				scaleVector = new(gameSettings.GetMouseSensitivity(), gameSettings.GetMouseSensitivity());
			}

			look.Scale(scaleVector);
		}

		public void OnJump(InputValue value)
		{
			jump = value.isPressed;
		}

		public void OnSprint(InputValue value)
		{
			sprint = value.isPressed;
		}

		public void OnToggleWalk(InputValue value)
		{
			toggleWalk = !toggleWalk;

		}

		public void OnDodge(InputValue value)
		{
			if (value.isPressed)
			{
				onDodgeInput?.Invoke();
			}
		}

		public void OnMenu(InputValue value)
		{
			if (value.isPressed)
			{
				onMenuEvent?.Invoke();

			}
		}

		public void OnCustomizeCharacter(InputValue value)
		{
			if (value.isPressed)
			{
				useHotkeyToCustomizeCharacterStat.UpdateStat();
				onCustomizeCharacter?.Invoke();
			}
		}

		public void OnTab(InputValue value)
		{
			onToggleTwoHandsInput?.Invoke();
		}

		public void OnLightAttack(InputValue value)
		{
			onLightAttackInput?.Invoke();
		}

		public void OnBlock(InputValue value)
		{
			bool previousState = block;
			block = value.isPressed;

			if (previousState != block)
			{
				if (block)
				{
					onBlock_Start?.Invoke();
				}
				else
				{
					onBlock_End?.Invoke();
				}
			}

		}

		public void OnLockOn(InputValue value)
		{
			onLockOnInput?.Invoke();

		}

		public void OnHeavyAttack(InputValue value)
		{
			onHeavyAttackInput?.Invoke();

		}

		public void OnInteract(InputValue value)
		{
			interact = value.isPressed;

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
			consumeFavoriteItem = value.isPressed;

			onConsumeFavoriteItem?.Invoke();
		}

		public void OnQuickSave(InputValue value)
		{
		}
		public void OnQuickLoad(InputValue value)
		{
		}

		public void OnNextMenu(InputValue value)
		{
			onNextMenu?.Invoke();
		}
		public void OnPreviousMenu(InputValue value)
		{
			onPreviousMenu?.Invoke();
		}

		public void OnAdvanceOneHour()
		{
			onAdvanceOneHour?.Invoke();
		}
		public void OnGoBackOneHour()
		{
			onGoBackOneHour?.Invoke();
		}

		public RebindingOperation ChangeInput(InputAction inputAction)
		{
			return inputAction
				.PerformInteractiveRebinding()
				.WithControlsExcluding("<Mouse>/leftButton")
				.WithControlsExcluding("<Mouse>/rightButton")
				.WithControlsExcluding("<Keyboard>/f5")
				.WithControlsExcluding("<Keyboard>/f9")
				.WithControlsExcluding("<Keyboard>/escape")
				.WithCancelingThrough("<Keyboard>/escape")
				.Start();
		}

		public IEnumerator Rebind(string actionName, UnityAction<string> onRebindSuccessfull)
		{
			InputAction inputAction = playerInput
				.actions
				.FindAction(actionName);

			if (inputAction == null)
			{
				yield break;
			}

			inputAction.Disable();

			RebindingOperation rebindingOperation = ChangeInput(inputAction);

			yield return new WaitUntil(() =>
			{
				return rebindingOperation.completed;
			});

			onRebindSuccessfull.Invoke(rebindingOperation.action.SaveBindingOverridesAsJson());

			rebindingOperation.Dispose();
			inputAction.Enable();
		}

		public string GetCurrentKeyBindingForAction(string actionName)
		{
			InputAction inputAction = playerInput
				.actions
				.FindAction(actionName);

			if (inputAction == null)
			{
				return "";
			}

			return inputAction.GetBindingDisplayString();
		}

		public void RestoreDefaultKeyBindings()
		{
			playerInput.actions.RemoveAllBindingOverrides();
		}

		public void ApplyBindingOverride(string actionName, string overridePayload)
		{
			InputAction inputAction = playerInput
				.actions
				.FindAction(actionName);

			if (inputAction != null)
			{
				inputAction.LoadBindingOverridesFromJson(overridePayload);
			}
		}
	}
}
