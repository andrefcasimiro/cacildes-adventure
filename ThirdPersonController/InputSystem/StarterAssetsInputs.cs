using UnityEngine;
using UnityEngine.InputSystem;

using UnityEngine.Events;

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

		[Header("UI")]
		public UnityEvent onMenuEvent;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorInputForLook = true;

		[Header("Main Menu")]
		public UnityEvent onNextMenu;
		public UnityEvent onPreviousMenu;

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

		public void OnTab(InputValue value)
		{
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

	}
}
