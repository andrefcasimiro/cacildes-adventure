using UnityEngine;
using UnityEngine.InputSystem;

using AF.Stats;

namespace AF
{
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Stamina")]
        public int jumpStaminaCost = 15;
        public float runStaminaCost = .05f;

        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float WalkSpeed = 2.0f;

        public float RunSpeed = 7.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 10f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float LockOnSpeed = 6.5f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        public float JumpHeightBonus = 0f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        public float jumpAttackVelocity = -5f;

        // cinemachine
        public float _cinemachineTargetYaw = 0f;
        private float _cinemachineTargetPitch = 0f;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        public float verticalVelocityBonus = 0f;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        [HideInInspector] public int _animIDFreeFall;
        private int _animIDMotionSpeed;

        [Header("Components")]
        public PlayerManager playerManager;
        private PlayerInput _playerInput;
        [HideInInspector] public StarterAssetsInputs _input;

        public GameObject _mainCamera;
        public UIDocumentReceivedItemPrompt uIDocumentReceivedItemPrompt;

        Animator _animator;
        CharacterController _controller;
        ClimbController climbController;

        DodgeController dodgeController;
        PlayerCombatController playerCombatController;
        PlayerComponentManager playerComponentManager;
        StaminaStatManager staminaStatManager;
        EquipmentGraphicsHandler equipmentGraphicsHandler;
        FootstepListener footstepListener;
        PlayerShootingManager playerShootingManager;
        PlayerParryManager playerParryManager;
        StatsBonusController playerStatsBonusController;

        public ViewClockMenu viewClockMenu;

        public MenuManager menuManager;

        public LockOnManager lockOnManager;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private float maxEnableCooldown = 0.25f;
        private float enableCooldown = Mathf.Infinity;

        float defaultFieldOfView;

        public Cinemachine.CinemachineVirtualCamera virtualCamera;

        [Header("Sprint FOV")]
        public float sprintFieldOfViewSpeedBonus = 10f;
        public float sprintFieldOfViewSpeedTransition = 2f;

        [Header("Fall Damage")]
        public bool fallDamageInitialized = false;
        public bool trackFallDamage = true;
        public float minimumFallHeightToTakeDamage = 5f;
        public float damageMultiplierPerMeter = 25f;
        float fallBeganHeight;
        bool PreviousGrounded = true;

        public AudioSource jumpAndDodgeAudiosource;

        public bool canMove = true;

        public bool isSliding = false;
        public bool isSlidingOnIce = false;
        public bool skateRotation = false;



        public bool canRotateCharacter = true;

        [Header("Components")]

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        private void OnEnable()
        {
            this.enableCooldown = 0f;
        }

        private void Awake()
        {
            defaultFieldOfView = virtualCamera.m_Lens.FieldOfView;

            _animator = playerManager.animator;
            _controller = playerManager.characterController;
            climbController = playerManager.climbController;
            dodgeController = playerManager.dodgeController;
            playerCombatController = playerManager.playerCombatController;
            playerComponentManager = playerManager.playerComponentManager;
            staminaStatManager = playerManager.staminaStatManager;
            equipmentGraphicsHandler = playerManager.equipmentGraphicsHandler;
            footstepListener = playerManager.footstepListener;
            playerShootingManager = playerManager.playerShootingManager;
            playerParryManager = playerManager.playerParryManager;
            playerStatsBonusController = playerManager.statsBonusController;

        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = _animator;

            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            if (enableCooldown < maxEnableCooldown)
            {
                enableCooldown += Time.deltaTime;
            }

            bool isClimbing = climbController.climbState != ClimbController.ClimbState.NONE;

            if (playerShootingManager.IsShooting() == false)
            {
                if (isClimbing == false)
                {
                    JumpAndGravity();
                    GroundedCheck();

                    if (playerManager.IsBusy == false)
                    {
                        Move();
                    }
                }
                else
                {
                    Climb();
                }

            }

            if (Grounded == false)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, _controller.transform.up * -1f, out hit))
                {
                    if (hit.transform.tag == "Enemy")
                    {
                        lockOnManager.DisableLockOn();
                    }
                }
            }
        }

        private void LateUpdate()
        {
            /*if (lockOnManager.isLockedOn)
            {
                return;
            }*/

            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, _controller.transform.position.y - GroundedOffset,
               _controller.transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            if (fallDamageInitialized)
            {

                if (PreviousGrounded == true && Grounded == false)
                {
                    fallBeganHeight = _controller.transform.position.y;

                }
                else if (PreviousGrounded == false && Grounded == true)
                {
                    float fallEndHeight = _controller.transform.position.y;

                    var currentFallHeight = Mathf.Abs(fallBeganHeight - fallEndHeight);

                    // Takes fall damage?
                    if (currentFallHeight > minimumFallHeightToTakeDamage && trackFallDamage)
                    {
                        FindObjectOfType<PlayerHealthbox>(true).TakeEnvironmentalDamage(Mathf.RoundToInt(currentFallHeight * damageMultiplierPerMeter), currentFallHeight <= minimumFallHeightToTakeDamage + 3 ? 1 : 10, true, 0, WeaponElementType.None);
                    }
                }


                PreviousGrounded = Grounded;
            }



            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = 1.0f;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Climb()
        {
            float targetSpeed = WalkSpeed;

            if (_input.move == Vector2.zero)
            {
                targetSpeed = 0.0f;
            }

            _speed = targetSpeed;


            _animationBlend = Mathf.Lerp(_animationBlend, _input.sprint ? 1.5f : 1f, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f)
            {
                _animationBlend = 0f;
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            targetDirection.x = 0;
            targetDirection.z = 0;

            float direction = _input.move.y;

            if (direction > 0)
            {
                _verticalVelocity = _speed * Time.deltaTime;
            }
            else if (direction < 0)
            {
                _verticalVelocity = _speed * Time.deltaTime * -1;
            }
            else
            {
                _verticalVelocity = 0f;
            }

            climbController.Climb(_speed * Time.deltaTime * direction);

            // move the player
            if (_controller.enabled)
            {
                _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                                 new Vector3(0.0f, _verticalVelocity + verticalVelocityBonus, 0.0f) * Time.deltaTime);
            }

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
            }
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            bool isSprinting = _input.sprint && playerStatsDatabase.currentStamina > 1f && _input.move != Vector2.zero;
            float targetSpeed = isSprinting ? SprintSpeed : (_input.toggleWalk ? WalkSpeed : RunSpeed);


            if (lockOnManager.isLockedOn)
            {
                targetSpeed = LockOnSpeed;

                // Give more speed to gamepad left stick when strafing 
                if (Gamepad.current != null && Gamepad.current.leftStick.IsActuated())
                {
                    targetSpeed *= 1.25f;
                }

                if (isSprinting)
                {
                    targetSpeed *= 1.25f;
                }
            }

            var weightSpeed = playerStatsBonusController.weightPenalty > 0 ? playerStatsBonusController.weightPenalty : 0;

            targetSpeed -= weightSpeed;

            if (_input.sprint)
            {
                staminaStatManager.DecreaseStamina(runStaminaCost * Time.deltaTime);
            }

            if (isSprinting)
            {
                virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, defaultFieldOfView + sprintFieldOfViewSpeedBonus, sprintFieldOfViewSpeedTransition * Time.deltaTime);
            }
            else
            {
                virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, defaultFieldOfView, sprintFieldOfViewSpeedTransition * Time.deltaTime);
            }

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (
                climbController.climbState == ClimbController.ClimbState.ENTERING
                || climbController.climbState == ClimbController.ClimbState.EXITING
                || playerCombatController.isCombatting
            )
            {
                _speed = 0f;
            }
            else
            {
                // DARK SOULS
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero && canRotateCharacter == true)
            {

                if (lockOnManager.nearestLockOnTarget != null && lockOnManager.isLockedOn && dodgeController.IsDodging() == false)
                {

                    Vector3 targetRot = lockOnManager.nearestLockOnTarget.transform.position - _controller.transform.position;
                    targetRot.y = 0;
                    var t = Quaternion.LookRotation(targetRot);

                    _controller.transform.rotation = Quaternion.Lerp(transform.rotation, t, 100f * Time.deltaTime);
                }
                else
                {
                    _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                      _mainCamera.transform.eulerAngles.y;
                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                        RotationSmoothTime);

                    if (skateRotation)
                    {
                        rotation = Mathf.Clamp(rotation, 0f, 160f);
                    }

                    // rotate to face input direction relative to camera position
                    _controller.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }

            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            if (climbController.climbState == ClimbController.ClimbState.CLIMBING)
            {
                targetDirection.x = 0;
                targetDirection.z = 0;

                float direction = _input.move.y;

                if (direction > 0)
                {
                    _verticalVelocity = _speed * Time.deltaTime;
                }
                else if (direction < 0)
                {
                    _verticalVelocity = _speed * Time.deltaTime * -1;
                }
                else
                {
                    _verticalVelocity = 0f;
                }

                _animIDMotionSpeed = (int)(_speed * 2f);
                climbController.Climb(_speed * Time.deltaTime * direction);
            }

            if (dodgeController.IsDodging())
            {
                targetDirection = Vector3.zero;
            }

            if (isSlidingOnIce)
            {
                _controller.Move(transform.forward * 10f * Time.deltaTime +
                                 new Vector3(0.0f, _verticalVelocity + verticalVelocityBonus, 0.0f) * Time.deltaTime);
            }
            else if (lockOnManager.nearestLockOnTarget != null && lockOnManager.isLockedOn && dodgeController.IsDodging() == false)
            {
                float lockOnSpeed = _input.move.x != 0 && _input.move.y != 0 ? _speed : _speed * 1.5f;

                Vector3 targetPos = (transform.forward * (lockOnSpeed) * _input.move.y + _controller.transform.right * (lockOnSpeed) * _input.move.x);
                targetPos.y = _verticalVelocity + verticalVelocityBonus;
                _controller.Move(targetPos * Time.deltaTime);
            }
            else
            {
                _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                                 new Vector3(0.0f, _verticalVelocity + verticalVelocityBonus, 0.0f) * Time.deltaTime);
            }


            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);

                // Get movement penalties
                var jumpWeightSpeed = playerStatsBonusController.weightPenalty > 0 ? playerStatsBonusController.weightPenalty : 0;

                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude - jumpWeightSpeed);
            }
        }

        public void StopMovement()
        {
            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, 0f);
                _animator.SetFloat(_animIDMotionSpeed, 0f);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump
                    && _jumpTimeoutDelta <= 0.0f
                    && enableCooldown >= maxEnableCooldown
                    && uIDocumentReceivedItemPrompt.isActiveAndEnabled == false
                    && menuManager.IsMenuOpen() == false
                )
                {
                    if (CanJump())
                    {
                        _input.jump = false;

                        //lockOnManager.DisableLockOn();

                        float JumpWeightBonus = 0;
                        if (equipmentGraphicsHandler.IsLightWeight())
                        {
                            JumpWeightBonus = .5f;
                        }
                        else if (equipmentGraphicsHandler.IsHeavyWeight())
                        {
                            JumpWeightBonus = -1;
                        }


                        // the square root of H * -2 * G = how much velocity needed to reach desired height
                        _verticalVelocity = Mathf.Sqrt((JumpHeight + JumpHeightBonus + JumpWeightBonus) * -2f * Gravity);

                        var weightSpeed = playerStatsBonusController.weightPenalty > 0 ? playerStatsBonusController.weightPenalty : 0;

                        _verticalVelocity -= weightSpeed;


                        if (isSliding || isSlidingOnIce)
                        {

                            _controller.Move(transform.forward * (5f * Time.deltaTime));
                        }

                        // update animator if using character
                        if (_hasAnimator)
                        {
                            staminaStatManager.DecreaseStamina(jumpStaminaCost);

                            _animator.Play("JumpStart");
                        }
                    }
                    else
                    {
                        _input.jump = false;
                    }
                }
                else
                {
                    _input.jump = false;
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            if (!playerCombatController.IsStartingJumpAttack())
            {
                // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
                if (_verticalVelocity < _terminalVelocity)
                {
                    var jumpAttackVelocityFinal = jumpAttackVelocity;
                    if (equipmentDatabase.GetCurrentWeapon() != null)
                    {
                        jumpAttackVelocityFinal = equipmentDatabase.GetCurrentWeapon().jumpAttackVelocity;
                    }

                    _verticalVelocity += (Gravity * Time.deltaTime) + (playerCombatController.IsJumpAttacking() ? jumpAttackVelocityFinal : 0f);
                }
            }
            else if ((equipmentDatabase.GetCurrentWeapon() == null) || (equipmentDatabase.GetCurrentWeapon() != null && equipmentDatabase.GetCurrentWeapon().stopInAir == true))
            {
                _verticalVelocity = 0f;
            }
            else
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        bool CanJump()
        {
            if (viewClockMenu != null && viewClockMenu.isActiveAndEnabled)
            {
                return false;
            }

            return staminaStatManager.HasEnoughStaminaForAction(jumpStaminaCost) &&
                        dodgeController.IsDodging() == false &&
                        playerCombatController.isCombatting == false
                        && canMove;
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, _controller.transform.position.y - GroundedOffset, _controller.transform.position.z),
                GroundedRadius);
        }

        /// <summary>
        ///  Animation Event
        /// </summary>
        public void DisableCharacterRotation()
        {
            canRotateCharacter = false;
        }


        /*private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip,_controller.transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }*/
    }
}
