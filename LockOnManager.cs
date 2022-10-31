using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        List<LockOnRef> availableTargets = new List<LockOnRef>();
       public float maximumLockOnDistance = 15;

        public LockOnRef nearestLockOnTarget;

        public LockOnRef leftLockTarget;
        public LockOnRef rightLockTarget;

        public LayerMask blockLayer;

        public float mouseSensitivityForNearbyTargets = 2f;

        //public float timeBeforeUnlocking = .5f;
        //float unlockTimer = 0f;

        int LayerEnvironment;

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

            if (isLockedOn)
            {
                HandleLockOn();
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

            availableTargets.Clear();

            playerAnimator.SetFloat(hashStrafeHorizontal, 0);
            playerAnimator.SetFloat(hashStrafeVertical, 0);
        }

        public void HandleLockOn()
        {
            float shortestDistance = Mathf.Infinity;
            float shortestDistanceOfLeftTarget = Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(playerAnimator.transform.position, 26);

            for (int i = 0; i < colliders.Length; i++)
            {
                LockOnRef enemy = colliders[i].GetComponent<LockOnRef>();

                if (enemy != null)
                {
                    Vector3 lockTargetDirection = enemy.transform.position - playerAnimator.transform.position;
                    float distanceFromTarget = Vector3.Distance(enemy.transform.position, playerAnimator.transform.position);
                    //float viewableAngle = Vector3.Angle(lockTargetDirection, Camera.main.transform.forward);

                    if (enemy.transform.root != playerAnimator.transform.root 
                        //&& viewableAngle > -50 && viewableAngle < 50
                        && distanceFromTarget <= maximumLockOnDistance)
                    {
                        availableTargets.Add(enemy);
                    }
                }
            }

            for (int i = 0; i < availableTargets.Count; i++)
            {
                float distanceFromTarget = Vector3.Distance(playerAnimator.transform.position, availableTargets[i].transform.position);

                if (distanceFromTarget < shortestDistance && nearestLockOnTarget == null)
                {
                    shortestDistance = distanceFromTarget;
                    if (availableTargets[i].CanLockOn())
                    {
                        nearestLockOnTarget = availableTargets[i];
                    }
                }

                if (nearestLockOnTarget == null) {
                    break;
                }

                Vector3 relativeEnemyPosition = nearestLockOnTarget.transform.InverseTransformPoint(availableTargets[i].transform.position);
                var distanceFromLeftTarget = nearestLockOnTarget.transform.position.x - availableTargets[i].transform.position.x;
                var distanceFromRightTarget = nearestLockOnTarget.transform.position.x + availableTargets[i].transform.position.x;

                if (relativeEnemyPosition.x > 0.00 && distanceFromLeftTarget < shortestDistanceOfLeftTarget)
                {
                    shortestDistanceOfLeftTarget = distanceFromLeftTarget;

                    if (availableTargets[i].CanLockOn())
                    {
                        leftLockTarget = availableTargets[i];
                    }
                }
                if (relativeEnemyPosition.x < 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                {
                    shortestDistanceOfRightTarget = distanceFromRightTarget;

                    if (availableTargets[i].CanLockOn())
                    {
                        rightLockTarget = availableTargets[i];
                    }
                }

                if (inputs.look != Vector2.zero)
                {
                    if (inputs.look.x < -mouseSensitivityForNearbyTargets && leftLockTarget != null)
                    {
                        SwitchToLeftTarget();
                    }
                    else if (inputs.look.x > mouseSensitivityForNearbyTargets && rightLockTarget != null)
                    {
                        SwitchToRightTarget();
                    }
                }
            }

            if (nearestLockOnTarget != null)
            {
                lockOnCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().m_LookAt = (nearestLockOnTarget.transform);
            }
            else
            {
                DisableLockOn();
            }
        }

    }

}
