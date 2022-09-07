using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

namespace AF
{

    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class Player : Character, ISaveable
    {
        public readonly int hashBusy = Animator.StringToHash("Busy");

        // Combat
        public readonly int hashCombatting = Animator.StringToHash("Combatting");
        public readonly int hashAttacking1 = Animator.StringToHash("Attacking1");
        public readonly int hashAttacking2 = Animator.StringToHash("Attacking2");
        public readonly int hashAttacking3 = Animator.StringToHash("Attacking3");
        public readonly int hashBlocking = Animator.StringToHash("Blocking");
        public readonly int hashBlockingHit = Animator.StringToHash("BlockingHit");
        public readonly int hashDead = Animator.StringToHash("Dead");
        public readonly int hashGrabbingObject = Animator.StringToHash("GrabbingObject");

        [Header("Movement")]
        public float walkSpeed = 6;
        public float runSpeed = 9;
        public float rotationSpeed = 8;
        Vector3 desiredMoveDirection;

        [Header("Flags")]
        public bool isAttacking = false;
        public bool isParrying = false;
        public bool isSprinting = false;
        public bool isBlocking = false;
        public bool isDodging = false;
        public bool isDead = false;

        [Header("References")]
        public Transform headTransform;

        [HideInInspector] public Animator animator => GetComponent<Animator>();
        [HideInInspector] public Rigidbody rigidbody => GetComponent<Rigidbody>();
        [HideInInspector] public CapsuleCollider capsuleCollider => GetComponent<CapsuleCollider>();

        public InputActions inputActions;

        [HideInInspector]
        public PlayerCombatManager playerCombatManager => GetComponent<PlayerCombatManager>();

        MenuManager menuManager;

        EquipmentGraphicsHandler equipmentGraphicsHandler;

        public float SPRINTING_STAMINA_COST = .5f;
        public float DODGE_STAMINA_COST = 30;

        Vector3 moveDirectionResult;

        void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new InputActions();
            }

            // Movement Input
            inputActions.PlayerActions.Movement.performed += ctx => desiredMoveDirection = ctx.ReadValue<Vector2>();
            inputActions.PlayerActions.Movement.canceled += ctx => desiredMoveDirection = Vector3.zero;

            inputActions.PlayerActions.Sprint.performed += ctx => isSprinting = true;
            inputActions.PlayerActions.Sprint.canceled += ctx => isSprinting = false;

            inputActions.PlayerActions.Roll.performed += ctx => HandleRoll();

            // Combat Input
            inputActions.PlayerActions.Attack.performed += ctx =>
            {
                playerCombatManager.HandleAttack();
            };

            inputActions.PlayerActions.Guard.performed += ctx => playerCombatManager.Guard();
            inputActions.PlayerActions.Guard.canceled += ctx => playerCombatManager.StopGuard();

            // UI
            inputActions.PlayerActions.MainMenu.performed += ctx =>
            {
                Player player = FindObjectOfType<Player>(true);

                // Stop player
                animator.SetFloat("movementSpeed", 0f);
            };

            inputActions.PlayerActions.QuickSave.performed += ctx =>
            {
                SaveSystem.instance.SaveGameData();
            };
            inputActions.PlayerActions.QuickLoad.performed += ctx =>
            {
                SaveSystem.instance.LoadGameData();
            };

            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        void Awake()
        {
            menuManager = FindObjectOfType<MenuManager>(true);
            equipmentGraphicsHandler = FindObjectOfType<EquipmentGraphicsHandler>(true);

            StartCoroutine(SpawnPlayer());
        }

        private IEnumerator SpawnPlayer() {
            yield return null;
            MapManager.instance.SpawnPlayer(this.gameObject);
        }

        private void Start()
        {
            equipmentGraphicsHandler.HideShield();
        }

        protected void Update()
        {
            isBlocking = animator.GetBool(hashBlocking);
            isDodging = animator.GetBool(hashDodging);
            isAttacking = animator.GetBool(hashCombatting);
            isDead = animator.GetBool(hashDead);

            if (equipmentGraphicsHandler.shieldInstance != null)
            {
                equipmentGraphicsHandler.ToggleShield(isBlocking);
            }

            moveDirectionResult = GetMoveDirection();
        }

        protected void FixedUpdate()
        {
            if (IsBusy())
            {
                return;
            }

            HandleMovement();

            if (isSprinting)
            {
                if (moveDirectionResult.magnitude <= 0 || PlayerStatsManager.instance.HasEnoughStaminaForAction(SPRINTING_STAMINA_COST) == false)
                {
                    isSprinting = false;
                }
                else
                {
                    // Handle issues with floating values on limit (causing footstep sounds to play overlapped)
                    if (animator.GetFloat(hashMovementSpeed) >= 0.95f)
                    {
                        animator.SetFloat(hashMovementSpeed, 1f);
                    }
                    else
                    {
                        animator.SetFloat(hashMovementSpeed, 1f, 0.05f, Time.fixedDeltaTime);
                    }

                    PlayerStatsManager.instance.DecreaseStamina(SPRINTING_STAMINA_COST);
                }
            }
            else if (moveDirectionResult.magnitude == 0)
            {
                // Handle issues with floating values on limit (causing footstep sounds to play overlapped)
                if (animator.GetFloat(hashMovementSpeed) <= 0.05f)
                {
                    animator.SetFloat(hashMovementSpeed, 0f);
                }
                else
                {
                    animator.SetFloat(hashMovementSpeed, 0f, 0.05f, Time.fixedDeltaTime);
                }
            }
            else
            {
                // Handle issues with floating values on limit (causing footstep sounds to play overlapped)
                if (animator.GetFloat(hashMovementSpeed) <= 0.55f || animator.GetFloat(hashMovementSpeed) >= 0.45f)
                {
                    animator.SetFloat(hashMovementSpeed, 0.5f);
                }
                else
                {
                    animator.SetFloat(hashMovementSpeed, 0.5f, 0.05f, Time.fixedDeltaTime);
                }
            }
        }

        #region Movement
        void HandleMovement()
        {
            if (isAttacking || isDodging)
            {
                return;
            }

            Vector3 targetVector = GetMoveDirection();
            var rotation = Quaternion.LookRotation(targetVector);

            if (isBlocking)
            {
                if (isSprinting)
                {
                    playerCombatManager.StopGuard();
                }
                else
                {
                    if (
                        equipmentGraphicsHandler.shieldInstance != null
                        && equipmentGraphicsHandler.shieldGraphic != null
                        && equipmentGraphicsHandler.shieldGraphic.activeSelf)
                    {
                        // Lock on to target logic
                        Character closestCharacter = equipmentGraphicsHandler.shieldInstance.FindClosestCharacter(this.transform.position);

                        if (closestCharacter != null)
                        {
                            var lookPos = closestCharacter.transform.position - transform.position;
                            lookPos.y = 0;
                            rotation = Quaternion.LookRotation(lookPos);
                        }
                    }
                }
            }

            if (moveDirectionResult.magnitude != 0)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotationSpeed);
            }

            bool isCollidingWithObject = Physics.Raycast(this.transform.position, this.transform.forward, 1f);

            var speed = ((isSprinting && isCollidingWithObject == false)
                ? runSpeed
                : walkSpeed) * Time.fixedDeltaTime;
            
            if (isCollidingWithObject)
            {
                targetVector.y = 0;
            }
            
            var targetPosition = transform.position + targetVector * speed;

            rigidbody.position = targetPosition;
        }

        protected void HandleRoll()
        {
            if (IsBusy() || isAttacking || PlayerStatsManager.instance.HasEnoughStaminaForAction(DODGE_STAMINA_COST) == false)
            {
                return;
            }

            if (isBlocking)
            {
                playerCombatManager.StopGuard();
            }

            var rotation = Quaternion.LookRotation(GetMoveDirection());

            if (moveDirectionResult.magnitude != 0)
            {
                transform.rotation = rotation;
            }

            animator.CrossFade(hashDodging, 0.05f);

            PlayerStatsManager.instance.DecreaseStamina(DODGE_STAMINA_COST);
        }

        public Vector3 GetMoveDirection()
        {
            bool cameraInverted = Camera.main.transform.forward.z <= 0;

            float invertion = cameraInverted ? -1 : 1;

            float x = desiredMoveDirection.x;
            float z = desiredMoveDirection.y;

            x *= invertion;
            z *= invertion;

            return new Vector3(x, 0, z);
        }
        #endregion

        public bool IsBusy()
        {
            return isDead || menuManager.IsMenuOpened() || animator.GetBool(hashBusy);
        }

        public void SetBusy(bool isBusy)
        {
            if (isBusy)
            {
                this.animator.SetFloat(hashMovementSpeed, 0f);
            }

            this.animator.SetBool(hashBusy, isBusy);
            menuManager.canOpenMenu = !isBusy;
        }

        public void OnGameLoaded(GameData gameData)
        {
            PlayerData playerData = gameData.playerData;

            if (playerData == null)
            {
                return;
            }

            transform.position = playerData.position;
            transform.rotation = playerData.rotation;
        }
    }
}
