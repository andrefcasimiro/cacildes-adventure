using System.Collections;
using System.Collections.Generic;
using AF.Events;
using TigerForge;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AF
{

    public class LockOnManager : MonoBehaviour
    {
        public readonly int hashIsLockedOn = Animator.StringToHash("IsLockedOn");
        public readonly int hashStrafeHorizontal = Animator.StringToHash("StrafeHorizontal");
        public readonly int hashStrafeVertical = Animator.StringToHash("StrafeVertical");

        [Header("UI")]
        public GameObject lockOnUi;
        [Header("Flags")]
        public bool isLockedOn = false;

        [Header("Components")]
        public PlayerManager playerManager;
        public Transform playerHeadRef;
        public Soundbank soundbank;
        public StarterAssetsInputs inputs;

        [Header("Cameras")]
        public GameObject defaultCamera;
        public GameObject lockOnCamera;

        [Header("Lock On Settings")]
        public float maximumLockOnDistance = 15;
        public float MAX_TIME_BEFORE_DISENGAGING = 1f;

        [Header("Lock On References")]

        public LockOnRef nearestLockOnTarget;

        public LockOnRef leftLockTarget;
        public LockOnRef rightLockTarget;

        [Header("Layers")]
        public LayerMask detectionLayer;
        public LayerMask blockLayers;


        [Header("Target Switching")]
        public float mouseXSwitchThreshold = 0.5f;
        public float maxTargetSwitchingCooldown = 1f;
        [HideInInspector] public float targetSwitchingCooldown = Mathf.Infinity;
        Vector2 previousInputsLook = Vector2.zero;

        // Internal
        public List<LockOnRef> availableTargets = new List<LockOnRef>();

        bool evaluatingIfShouldDisengage = false;
        Coroutine CheckIfShouldDisengageCoroutine;
        Coroutine EvaluateLockOnAfterKillingEnemyCoroutine;

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_CHARACTER_KILLED, OnEnemyKilledCheckIfShouldDisengageLockOn);
        }

        private void Start()
        {
            DisableLockOn();
        }

        private void Update()
        {
            if (targetSwitchingCooldown < maxTargetSwitchingCooldown)
            {
                targetSwitchingCooldown += Time.deltaTime;
            }

            if (nearestLockOnTarget != null)
            {
                if (Vector3.Distance(playerManager.transform.position, nearestLockOnTarget.transform.position) > maximumLockOnDistance)
                {
                    DisableLockOn();
                    return;
                }

                playerManager.animator.SetFloat(hashStrafeHorizontal, inputs.move.x);
                playerManager.animator.SetFloat(hashStrafeVertical, inputs.move.y);

                if (!evaluatingIfShouldDisengage)
                {
                    if (IsViewBlocked())
                    {
                        // Something was hit between the player and the target
                        evaluatingIfShouldDisengage = true;

                        if (CheckIfShouldDisengageCoroutine != null)
                        {
                            StopCoroutine(CheckIfShouldDisengageCoroutine);
                        }

                        CheckIfShouldDisengageCoroutine = StartCoroutine(CheckIfShouldDisengage_Coroutine());
                    }
                }
            }

            if (isLockedOn && Vector2.Distance(previousInputsLook, inputs.look) >= mouseXSwitchThreshold)
            {
                HandleTargetSwitching();
            }

            previousInputsLook = inputs.look;
        }

        bool IsViewBlocked()
        {
            if (Physics.Linecast(playerHeadRef.transform.position, nearestLockOnTarget.transform.position, out RaycastHit hit, blockLayers))
            {
                if (hit.transform != null)
                {
                    return true;
                }
            }

            return false;
        }

        IEnumerator CheckIfShouldDisengage_Coroutine()
        {
            yield return new WaitForSeconds(MAX_TIME_BEFORE_DISENGAGING);

            if (nearestLockOnTarget != null)
            {
                if (IsViewBlocked())
                {
                    DisableLockOn();
                }
            }

            evaluatingIfShouldDisengage = false;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnLockOnInput()
        {
            if (isLockedOn)
            {
                DisableLockOn();
            }
            else
            {
                HandleLockOnClick(false);
            }
        }


        public void SnapPlayerRotationToLockOnTarget()
        {
            if (nearestLockOnTarget == null)
            {
                return;
            }

            Vector3 targetRot = nearestLockOnTarget.transform.position - playerManager.animator.transform.position;
            targetRot.y = 0;
            var t = Quaternion.LookRotation(targetRot);

            playerManager.transform.rotation = t;
        }

        public void EnableLockOn()
        {
            lockOnCamera.gameObject.SetActive(true);
            defaultCamera.gameObject.SetActive(false);

            this.lockOnUi.gameObject.SetActive(true);

            playerManager.animator.SetBool(hashIsLockedOn, true);

            isLockedOn = true;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="duration"></param>
        public void DisableLockOnAfter(float duration)
        {
            IEnumerator DisableLockOnAfter()
            {
                yield return new WaitForSeconds(duration);
                DisableLockOn();
            }

            StartCoroutine(DisableLockOnAfter());
        }

        public void DisableLockOn()
        {
            Camera.main.GetComponent<Cinemachine.CinemachineBrain>().m_DefaultBlend.m_Time = 0f;
            this.lockOnUi.gameObject.SetActive(false);
            isLockedOn = false;
            defaultCamera.gameObject.SetActive(true);
            lockOnCamera.gameObject.SetActive(false);
            playerManager.animator.SetBool(hashIsLockedOn, false);

            nearestLockOnTarget = null;
            rightLockTarget = null;
            leftLockTarget = null;

            playerManager.animator.SetFloat(hashStrafeHorizontal, 0);
            playerManager.animator.SetFloat(hashStrafeVertical, 0);
        }

        bool CanLockOn()
        {
            if (playerManager.playerShootingManager.isAiming)
            {
                return false;
            }

            return true;
        }

        public void HandleLockOnClick(bool shouldLookForActiveEnemies)
        {
            if (!CanLockOn())
            {
                return;
            }

            nearestLockOnTarget = null;
            availableTargets.Clear();

            float shortestDistance = Mathf.Infinity;
            LockOnRef[] colliders = FindObjectsByType<LockOnRef>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            for (int i = 0; i < colliders.Length; i++)
            {
                LockOnRef enemy = colliders[i];

                if (enemy != null)
                {
                    if (shouldLookForActiveEnemies)
                    {
                        CharacterManager characterManager = enemy.GetComponentInParent<CharacterManager>();

                        if (characterManager != null && characterManager?.targetManager?.currentTarget == null)
                        {
                            continue;
                        }
                    }

                    float distanceFromTarget = Vector3.Distance(enemy.transform.position, playerManager.transform.position);

                    if (enemy.transform.root != playerManager.transform.root
                        && InScreen(enemy)
                        && distanceFromTarget <= maximumLockOnDistance)
                    {
                        availableTargets.Add(enemy);
                    }
                }
            }

            for (int i = 0; i < availableTargets.Count; i++)
            {
                float distanceFromTarget = Vector3.Distance(playerManager.transform.position, availableTargets[i].transform.position);

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    if (availableTargets[i].CanLockOn())
                    {
                        Vector3 start = playerHeadRef.transform.position;
                        Vector3 direction = availableTargets[i].transform.position - start;

                        // Draw a debug ray from the start position in the specified direction
                        Debug.DrawRay(start, direction, Color.red);

                        RaycastHit[] hits;
                        hits = Physics.RaycastAll(start, direction, maximumLockOnDistance, detectionLayer);

                        foreach (var hit in hits)
                        {
                            // Check if the hit object has a specific MonoBehaviour component
                            LockOnRef component = hit.collider.GetComponent<LockOnRef>() ?? hit.collider.GetComponentInChildren<LockOnRef>();

                            if (component != null)
                            {
                                if (component.gameObject == availableTargets[i].gameObject)
                                {
                                    nearestLockOnTarget = availableTargets[i];
                                }
                            }

                        }
                    }
                }
            }

            if (nearestLockOnTarget != null)
            {
                if (targetSwitchingCooldown < maxTargetSwitchingCooldown)
                {
                    return;
                }

                soundbank.PlaySound(soundbank.uiLockOn);

                targetSwitchingCooldown = 0f;
                lockOnCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().m_LookAt = (nearestLockOnTarget.transform);

                SnapPlayerRotationToLockOnTarget();

                EnableLockOn();
            }
            else
            {
                DisableLockOn();
            }
        }

        bool InScreen(LockOnRef target)
        {
            Vector3 viewPortPosition = Camera.main.WorldToViewportPoint(target.transform.position);

            if (!(viewPortPosition.x > 0) || !(viewPortPosition.x < 1))
            {
                return false;
            }

            if (!(viewPortPosition.y > 0) || !(viewPortPosition.y < 1))
            {
                return false;
            }

            if (!(viewPortPosition.z > 0))
            {
                return false;
            }

            // Calculate the direction from the camera to the target
            Vector3 direction = target.transform.position - Camera.main.transform.position;

            // Create a ray from the camera position with the calculated direction
            Ray ray = new(Camera.main.transform.position, direction);

            // Perform the raycast with the maximum distance
            if (Physics.Raycast(
                ray, out RaycastHit hit, Vector3.Distance(target.transform.position, Camera.main.transform.position), blockLayers) && hit.transform != null)
            {
                return false;
            }

            return true;
        }


        public void HandleTargetSwitching()
        {
            bool lookedRight = inputs.look.x > 0 || (Gamepad.current != null && Gamepad.current.rightStick.right.IsActuated());
            bool lookedLeft = inputs.look.x < 0 || (Gamepad.current != null && Gamepad.current.rightStick.left.IsActuated());

            if (nearestLockOnTarget == null || lookedRight == false && lookedLeft == false)
            {
                return;
            }

            inputs.look.x = 0;
            inputs.look.y = 0;

            availableTargets.Clear();
            leftLockTarget = null;
            rightLockTarget = null;

            // Define the lock-on sphere's radius and center position
            float lockOnSphereRadius = 13f;
            Vector3 lockOnSphereCenter = playerHeadRef.transform.position;

            // Find all colliders within the lock-on sphere
            Collider[] colliders = Physics.OverlapSphere(lockOnSphereCenter, lockOnSphereRadius);

            foreach (var collider in colliders)
            {
                LockOnRef enemy = collider.GetComponent<LockOnRef>();

                if (enemy != null)
                {
                    // Calculate the direction and distance from the player to the target
                    Vector3 lockTargetDirection = enemy.transform.position - lockOnSphereCenter;
                    float distanceFromTarget = lockTargetDirection.magnitude;

                    if (enemy.transform.root != playerHeadRef.transform.root && InScreen(enemy) && distanceFromTarget <= maximumLockOnDistance)
                    {
                        availableTargets.Add(enemy);
                    }
                }
            }

            float shortestDistanceLeftTarget = Mathf.Infinity;
            float shortestDistanceRightTarget = Mathf.Infinity;

            foreach (var target in availableTargets)
            {
                Vector3 relativePlayerPosition = playerManager.transform.InverseTransformPoint(target.transform.position);
                float distanceToPlayer = Vector3.Distance(target.transform.position, playerManager.transform.position);

                if (relativePlayerPosition.x < 0.00 && distanceToPlayer < shortestDistanceLeftTarget)
                {
                    shortestDistanceLeftTarget = distanceToPlayer;
                    if (target.CanLockOn() && nearestLockOnTarget != target)
                    {
                        leftLockTarget = target;
                    }
                }
                else if (relativePlayerPosition.x > 0.00 && distanceToPlayer < shortestDistanceRightTarget)
                {
                    shortestDistanceRightTarget = distanceToPlayer;
                    if (target.CanLockOn() && nearestLockOnTarget != target)
                    {
                        rightLockTarget = target;
                    }
                }
            }

            if (lookedLeft && leftLockTarget != null)
            {
                if (targetSwitchingCooldown >= maxTargetSwitchingCooldown)
                {
                    SwitchLockOnTarget(leftLockTarget);
                    targetSwitchingCooldown = 0f;
                }
            }
            else if (lookedRight && rightLockTarget != null)
            {
                if (targetSwitchingCooldown >= maxTargetSwitchingCooldown)
                {
                    SwitchLockOnTarget(rightLockTarget);
                    targetSwitchingCooldown = 0f;
                }
            }
        }

        public void SwitchLockOnTarget(LockOnRef newTarget)
        {
            if (!newTarget.CanLockOn())
            {
                return;
            }

            RaycastHit[] hits;
            Vector3 direction = newTarget.transform.position - playerHeadRef.transform.position;

            hits = Physics.RaycastAll(playerHeadRef.transform.position, direction, maximumLockOnDistance, detectionLayer);

            foreach (var hit in hits)
            {
                LockOnRef component = hit.collider.GetComponent<LockOnRef>() ?? hit.collider.GetComponentInChildren<LockOnRef>();

                if (component != null && component == newTarget)
                {
                    nearestLockOnTarget = newTarget;
                    lockOnCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().m_LookAt = nearestLockOnTarget.transform;
                    soundbank.PlaySound(soundbank.uiLockOnSwitchTarget);

                    SnapPlayerRotationToLockOnTarget();
                    break;
                }
            }
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnEnemyKilledCheckIfShouldDisengageLockOn()
        {
            if (!isLockedOn)
            {
                return;
            }

            if (EvaluateLockOnAfterKillingEnemyCoroutine != null)
            {
                StopCoroutine(EvaluateLockOnAfterKillingEnemyCoroutine);
            }

            EvaluateLockOnAfterKillingEnemyCoroutine = StartCoroutine(EvaluateLockOnAfterKillingEnemy_Coroutine());
        }

        IEnumerator EvaluateLockOnAfterKillingEnemy_Coroutine()
        {
            yield return new WaitForSeconds(2f);
            HandleLockOnClick(true);
        }
    }
}
