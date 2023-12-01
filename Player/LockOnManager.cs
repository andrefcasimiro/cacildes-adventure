using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AF
{

    public class LockOnManager : MonoBehaviour
    {
        public readonly int hashIsLockedOn = Animator.StringToHash("IsLockedOn");
        public readonly int hashStrafeHorizontal = Animator.StringToHash("StrafeHorizontal");
        public readonly int hashStrafeVertical = Animator.StringToHash("StrafeVertical");

        public bool isLockedOn = false;

        public GameObject lockOnUi;

        public Animator playerAnimator;
        public Transform playerHeadRef;

        public StarterAssetsInputs inputs;

        public GameObject defaultCamera;
        public GameObject lockOnCamera;

        public float maximumLockOnDistance = 15;

        public LockOnRef nearestLockOnTarget;

        public LockOnRef leftLockTarget;
        public LockOnRef rightLockTarget;

        public float mouseSensitivityForNearbyTargets = 1.25f;

        public int LayerEnvironment;
        public int LayerDefault;

        [Header("Target Switching")]
        public int mouseXSwitchThreshold = 2;
        public float maxTargetSwitchingCooldown = 1f;
        [HideInInspector] public float targetSwitchingCooldown = Mathf.Infinity;

        public List<LockOnRef> availableTargets = new List<LockOnRef>();

        bool evaluatingIfShouldDisengage = false;

        public float MAX_TIME_BEFORE_DISENGAGING = 1f;

        private void Awake()
        {
            LayerEnvironment = LayerMask.NameToLayer("Environment");
            LayerDefault = LayerMask.NameToLayer("Default");
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

            if (inputs.lockOn)
            {
                inputs.lockOn = false;

                if (isLockedOn)
                {
                    DisableLockOn();
                }
                else
                {
                    HandleLockOnClick();
                }
            }

            if (nearestLockOnTarget != null)
            {
                if (Vector3.Distance(playerAnimator.transform.position, nearestLockOnTarget.transform.position) > maximumLockOnDistance)
                {
                    DisableLockOn();
                    return;
                }

                playerAnimator.SetFloat(hashStrafeHorizontal, inputs.move.x);
                playerAnimator.SetFloat(hashStrafeVertical, inputs.move.y);

                if (!evaluatingIfShouldDisengage)
                {
                    RaycastHit hit;
                    if (Physics.Linecast(playerHeadRef.transform.position, nearestLockOnTarget.transform.position, out hit))
                    {
                        if (hit.transform.gameObject.layer == LayerEnvironment || hit.transform.gameObject.layer == LayerDefault)
                        {
                            evaluatingIfShouldDisengage = true;

                            StartCoroutine(CheckIfShouldDisengage());
                        }
                    }
                }

            }

            /*if (isLockedOn)
            {
                HandleLockOn();
            }*/

            if (isLockedOn)
            {
                HandleTargetSwitching();
            }
        }

        IEnumerator CheckIfShouldDisengage()
        {
            yield return new WaitForSeconds(MAX_TIME_BEFORE_DISENGAGING);

            RaycastHit hit;

            if (nearestLockOnTarget != null)
            {
                if (Physics.Linecast(playerHeadRef.transform.position, nearestLockOnTarget.transform.position, out hit))
                {
                    if (hit.transform.gameObject.layer == LayerEnvironment || hit.transform.gameObject.layer == LayerDefault)
                    {
                        DisableLockOn();
                    }
                }
            }


            evaluatingIfShouldDisengage = false;
        }

        public void SwitchToLeftTarget()
        {
            if (!leftLockTarget.CanLockOn())
            {
                return;
            }

            RaycastHit[] hits;
            hits = Physics.RaycastAll(playerHeadRef.transform.position, leftLockTarget.transform.position - playerHeadRef.transform.position, maximumLockOnDistance, LayerMask.GetMask("Enemy"));

            foreach (var hit in hits)
            {
                // Check if the hit object has a specific MonoBehaviour component
                LockOnRef component = hit.collider.GetComponent<LockOnRef>() ?? hit.collider.GetComponentInChildren<LockOnRef>();


                if (component != null)
                {
                    if (component == leftLockTarget)
                    {
                        nearestLockOnTarget = leftLockTarget;
                        lockOnCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().m_LookAt = (nearestLockOnTarget.transform);
                        Soundbank.instance.PlayLockOnSwitchTarget();


                        SnapPlayerRotationToLockOnTarget();
                    }
                }
            }
        }

        public void SwitchToRightTarget()
        {
            if (!rightLockTarget.CanLockOn())
            {
                return;
            }

            RaycastHit[] hits;
            hits = Physics.RaycastAll(playerHeadRef.transform.position, rightLockTarget.transform.position - playerHeadRef.transform.position, maximumLockOnDistance, LayerMask.GetMask("Enemy"));

            foreach (var hit in hits)
            {
                // Check if the hit object has a specific MonoBehaviour component
                LockOnRef component = hit.collider.GetComponent<LockOnRef>() ?? hit.collider.GetComponentInChildren<LockOnRef>();

                if (component != null)
                {
                    if (component == rightLockTarget)
                    {
                        nearestLockOnTarget = rightLockTarget;
                        lockOnCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().m_LookAt = (nearestLockOnTarget.transform);
                        Soundbank.instance.PlayLockOnSwitchTarget();


                        SnapPlayerRotationToLockOnTarget();
                    }
                }
            }


        }

        public void SnapPlayerRotationToLockOnTarget()
        {
            if (nearestLockOnTarget == null)
            {
                return;
            }

            Vector3 targetRot = nearestLockOnTarget.transform.position - playerAnimator.transform.position;
            targetRot.y = 0;
            var t = Quaternion.LookRotation(targetRot);

            playerAnimator.transform.rotation = t;
        }

        public void EnableLockOn()
        {
            lockOnCamera.gameObject.SetActive(true);
            defaultCamera.gameObject.SetActive(false);

            this.lockOnUi.gameObject.SetActive(true);

            // playerAnimator.SetBool(hashIsLockedOn, true);

            isLockedOn = true;
        }

        public void DisableLockOn()
        {
            Camera.main.GetComponent<Cinemachine.CinemachineBrain>().m_DefaultBlend.m_Time = 0f;
            this.lockOnUi.gameObject.SetActive(false);
            isLockedOn = false;
            defaultCamera.gameObject.SetActive(true);
            lockOnCamera.gameObject.SetActive(false);
            //            playerAnimator.SetBool(hashIsLockedOn, false);

            nearestLockOnTarget = null;
            rightLockTarget = null;
            leftLockTarget = null;

            playerAnimator.SetFloat(hashStrafeHorizontal, 0);
            playerAnimator.SetFloat(hashStrafeVertical, 0);
        }

        public void HandleLockOnClick()
        {
            nearestLockOnTarget = null;
            availableTargets.Clear();

            float shortestDistance = Mathf.Infinity;
            LockOnRef[] colliders = FindObjectsByType<LockOnRef>(FindObjectsInactive.Exclude, FindObjectsSortMode.None); //Physics.OverlapSphere(playerAnimator.transform.position, 26);

            for (int i = 0; i < colliders.Length; i++)
            {
                LockOnRef enemy = colliders[i];

                if (enemy != null)
                {
                    float distanceFromTarget = Vector3.Distance(enemy.transform.position, playerAnimator.transform.position);

                    if (enemy.transform.root != playerAnimator.transform.root
                        && InScreen(enemy)
                        && distanceFromTarget <= maximumLockOnDistance)
                    {
                        availableTargets.Add(enemy);
                    }
                }

            }

            for (int i = 0; i < availableTargets.Count; i++)
            {
                float distanceFromTarget = Vector3.Distance(playerAnimator.transform.position, availableTargets[i].transform.position);

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
                        hits = Physics.RaycastAll(start, direction, maximumLockOnDistance, LayerMask.GetMask("Enemy"));

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

                Soundbank.instance.PlayLockOn();

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

            return true;
        }


        public void HandleTargetSwitching()
        {
            bool lookedRight = Mathf.FloorToInt(Input.GetAxisRaw("Mouse X")) >= mouseXSwitchThreshold || (Gamepad.current != null && Gamepad.current.rightStick.right.IsActuated());
            bool lookedLeft = Mathf.FloorToInt(Input.GetAxisRaw("Mouse X")) <= -mouseXSwitchThreshold || (Gamepad.current != null && Gamepad.current.rightStick.left.IsActuated());

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
                Vector3 relativePlayerPosition = playerAnimator.transform.InverseTransformPoint(target.transform.position);
                float distanceToPlayer = Vector3.Distance(target.transform.position, playerAnimator.transform.position);

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
                    SwitchToLeftTarget();
                    targetSwitchingCooldown = 0f;
                }
            }
            else if (lookedRight && rightLockTarget != null)
            {
                if (targetSwitchingCooldown >= maxTargetSwitchingCooldown)
                {
                    SwitchToRightTarget();
                    targetSwitchingCooldown = 0f;
                }
            }
        }
    }

}
