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

        Animator playerAnimator;
        public Transform playerHeadRef;

        StarterAssets.StarterAssetsInputs inputs;

        public GameObject defaultCamera;
        public GameObject lockOnCamera;

       public float maximumLockOnDistance = 15;

        public LockOnRef nearestLockOnTarget;

        public LockOnRef leftLockTarget;
        public LockOnRef rightLockTarget;

        public LayerMask blockLayer;

        public float mouseSensitivityForNearbyTargets = 1.25f;

        int LayerEnvironment;

        float maxTargetSwitchingCooldown = .3f;
        float targetSwitchingCooldown = Mathf.Infinity;

        private void Awake()
        {
            playerAnimator = FindObjectOfType<PlayerCombatController>(true).GetComponent<Animator>();

            inputs = FindObjectOfType<StarterAssets.StarterAssetsInputs>(true);

            LayerEnvironment = LayerMask.NameToLayer("Environment");
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
                    EnableLockOn();
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

                RaycastHit hit;
                if (Physics.Linecast(playerHeadRef.transform.position, nearestLockOnTarget.transform.position, out hit))
                {
                    if (hit.transform.gameObject.layer == LayerEnvironment)
                    {
                        DisableLockOn();
                        return;
                    }
                }

                if (inputs.sprint)
                {
                    DisableLockOn();
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

        public void SwitchToLeftTarget()
        {
            if (!leftLockTarget.CanLockOn())
            {
                return;
            }

            nearestLockOnTarget = leftLockTarget;

            lockOnCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().m_LookAt = (nearestLockOnTarget.transform);
        }

        public void SwitchToRightTarget()
        {
            if (!rightLockTarget.CanLockOn())
            {
                return;
            }

            nearestLockOnTarget = rightLockTarget;

            lockOnCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().m_LookAt = (nearestLockOnTarget.transform);
        }

        public void EnableLockOn()
        {
            Camera.main.GetComponent<Cinemachine.CinemachineBrain>().m_DefaultBlend.m_Time = .5f;
            isLockedOn = true;
            lockOnCamera.gameObject.SetActive(true);
            defaultCamera.gameObject.SetActive(false);
            playerAnimator.SetBool(hashIsLockedOn, true);

            this.lockOnUi.gameObject.SetActive(true);
        }

        public void DisableLockOn()
        {
            Camera.main.GetComponent<Cinemachine.CinemachineBrain>().m_DefaultBlend.m_Time = 0f;
            this.lockOnUi.gameObject.SetActive(false);
            isLockedOn = false;
            defaultCamera.gameObject.SetActive(true);
            lockOnCamera.gameObject.SetActive(false);
            playerAnimator.SetBool(hashIsLockedOn, false);

            nearestLockOnTarget = null;
            rightLockTarget = null;
            leftLockTarget = null;

            playerAnimator.SetFloat(hashStrafeHorizontal, 0);
            playerAnimator.SetFloat(hashStrafeVertical, 0);
        }

        public void HandleLockOnClick() {
            List<LockOnRef> availableTargets = new List<LockOnRef>();

            float shortestDistance = Mathf.Infinity;
            Collider[] colliders = Physics.OverlapSphere(playerAnimator.transform.position, isLockedOn ? 26 * 2 : 26);

            for (int i = 0; i < colliders.Length; i++)
            {
                LockOnRef enemy = colliders[i].GetComponent<LockOnRef>();

                if (enemy != null)
                {
                    Vector3 lockTargetDirection = enemy.transform.position - playerAnimator.transform.position;
                    float distanceFromTarget = Vector3.Distance(enemy.transform.position, playerAnimator.transform.position);
                    float viewableAngle = Vector3.Angle(lockTargetDirection, Camera.main.transform.forward);

                    if (enemy.transform.root != playerAnimator.transform.root
                        && viewableAngle > -90 && viewableAngle < 90
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
                        nearestLockOnTarget = availableTargets[i];
                    }
                }
            }

            if (nearestLockOnTarget != null)
            {
                if (targetSwitchingCooldown < maxTargetSwitchingCooldown)
                {
                    return;
                }

                targetSwitchingCooldown = 0f;
                lockOnCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().m_LookAt = (nearestLockOnTarget.transform);
            }
            else
            {
                DisableLockOn();
            }
        }

        public void HandleTargetSwitching()
        {
            List<LockOnRef> availableTargets = new List<LockOnRef>();

            float shortestDistanceOfLeftTarget = Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(playerAnimator.transform.position, isLockedOn ? 26 / 2 : 26);

            for (int i = 0; i < colliders.Length; i++)
            {
                LockOnRef enemy = colliders[i].GetComponent<LockOnRef>();

                if (enemy != null)
                {
                    Vector3 lockTargetDirection = enemy.transform.position - playerAnimator.transform.position;
                    float distanceFromTarget = Vector3.Distance(enemy.transform.position, playerAnimator.transform.position);
                    float viewableAngle = Vector3.Angle(lockTargetDirection, Camera.main.transform.forward);

                    if (enemy.transform.root != playerAnimator.transform.root
                        && viewableAngle > -90 && viewableAngle < 90
                        && distanceFromTarget <= maximumLockOnDistance)
                    {
                        availableTargets.Add(enemy);
                    }
                }
            }

            for (int i = 0; i < availableTargets.Count; i++)
            {
                Vector3 relativeEnemyPosition = nearestLockOnTarget.transform.InverseTransformPoint(availableTargets[i].transform.position);
                var distanceFromLeftTarget = nearestLockOnTarget.transform.position.x - availableTargets[i].transform.position.x;
                var distanceFromRightTarget = nearestLockOnTarget.transform.position.x + availableTargets[i].transform.position.x;

                if (relativeEnemyPosition.x > 0.00 && distanceFromLeftTarget <= shortestDistanceOfLeftTarget)
                {
                    shortestDistanceOfLeftTarget = distanceFromLeftTarget;

                    if (availableTargets[i].CanLockOn())
                    {
                        leftLockTarget = availableTargets[i];
                    }
                }
                if (relativeEnemyPosition.x < 0.00 && distanceFromRightTarget <= shortestDistanceOfRightTarget)
                {
                    shortestDistanceOfRightTarget = distanceFromRightTarget;

                    if (availableTargets[i].CanLockOn())
                    {
                        rightLockTarget = availableTargets[i];
                    }
                }

                if (inputs.look != Vector2.zero)
                {
                    if (
                        (inputs.look.x < -mouseSensitivityForNearbyTargets || Gamepad.current != null && Gamepad.current.rightStick.left.isPressed)
                        && leftLockTarget != null)
                    {
                        if (targetSwitchingCooldown < maxTargetSwitchingCooldown) { return; }

                        SwitchToLeftTarget();
                        targetSwitchingCooldown = 0f;
                        return;
                    }
                    else if (
                        (inputs.look.x > mouseSensitivityForNearbyTargets || Gamepad.current != null && Gamepad.current.rightStick.right.isPressed)
                        && rightLockTarget != null)
                    {
                        if (targetSwitchingCooldown < maxTargetSwitchingCooldown) { return; }

                        SwitchToRightTarget();
                        targetSwitchingCooldown = 0f;
                        return;
                    }
                }


            }
        }

    }

}