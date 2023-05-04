using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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
        int LayerDefault;

        float maxTargetSwitchingCooldown = 1f;
        float targetSwitchingCooldown = Mathf.Infinity;

        public List<LockOnRef> availableTargets = new List<LockOnRef>();

        public float timeBeforeDisengaging = 0.5f;
        bool evaluatingIfShouldDisengage = false;

        private void Awake()
        {
            playerAnimator = FindObjectOfType<PlayerCombatController>(true).GetComponent<Animator>();

            inputs = FindObjectOfType<StarterAssets.StarterAssetsInputs>(true);

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

        IEnumerator CheckIfShouldDisengage()
        {
            yield return new WaitForSeconds(timeBeforeDisengaging);

            RaycastHit hit;
            if (Physics.Linecast(playerHeadRef.transform.position, nearestLockOnTarget.transform.position, out hit))
            {
                if (hit.transform.gameObject.layer == LayerEnvironment || hit.transform.gameObject.layer == LayerDefault)
                {
                    DisableLockOn();
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
            nearestLockOnTarget = null;
            availableTargets.Clear();

            float shortestDistance = Mathf.Infinity;
            Collider[] colliders = Physics.OverlapSphere(playerAnimator.transform.position, 26);

            for (int i = 0; i < colliders.Length; i++)
            {
                LockOnRef enemy = colliders[i].GetComponent<LockOnRef>();

                if (enemy != null)
                {
                    Vector3 lockTargetDirection = enemy.transform.position - playerAnimator.transform.position;
                    float distanceFromTarget = Vector3.Distance(enemy.transform.position, playerAnimator.transform.position);
                    float viewableAngle = Vector3.Angle(lockTargetDirection, Camera.main.transform.forward);

                    if (enemy.transform.root != playerAnimator.transform.root
                        && viewableAngle > -50 && viewableAngle < 50
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

                        RaycastHit hit;
                        if (Physics.Linecast(playerHeadRef.transform.position, availableTargets[i].transform.position, out hit))
                        {
                            if (hit.transform.gameObject.layer != LayerEnvironment && hit.transform.gameObject.layer != LayerDefault)
                            {
                                nearestLockOnTarget = availableTargets[i];
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

                targetSwitchingCooldown = 0f;
                lockOnCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>().m_LookAt = (nearestLockOnTarget.transform);
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
            if (nearestLockOnTarget == null)
            {
                return;
            }

            if (inputs.look.x == 0f || Gamepad.current != null && Gamepad.current.rightStick.IsActuated())
            {
                return;
            }
                
            availableTargets.Clear();
            leftLockTarget = null;
            rightLockTarget = null;


            float shortestDistanceLeftTarget = Mathf.Infinity;
            float shortestDistanceRightTarget = Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(playerAnimator.transform.position, 13);

            for (int i = 0; i < colliders.Length; i++)
            {
                LockOnRef enemy = colliders[i].GetComponent<LockOnRef>();

                if (enemy != null)
                {
                    Vector3 lockTargetDirection = enemy.transform.position - playerAnimator.transform.position;
                    float distanceFromTarget = Vector3.Distance(enemy.transform.position, playerAnimator.transform.position);

                    if (enemy.transform.root != playerAnimator.transform.root
                        && InScreen(enemy)
                        && distanceFromTarget <= maximumLockOnDistance)
                    {
                        availableTargets.Add(enemy);
                    }
                }
            }

            for (int k = 0; k < availableTargets.Count; k++)
            {
                Vector3 relativePlayerPostion = playerAnimator.transform.InverseTransformPoint(availableTargets[k].transform.position);
                var distanceFromLeftTarget = 1000f; //nearestLockOnTarget.transform.position.x - availableTargets[k].transform.position.x;
                var distanceFromRightTarget = 1000f; // nearestLockOnTarget.transform.position.x + availableTargets[k].transform.position.x;

                if (relativePlayerPostion.x < 0.00 && availableTargets[k] != nearestLockOnTarget)
                {
                    distanceFromLeftTarget = Vector3.Distance(nearestLockOnTarget.transform.position, availableTargets[k].transform.position);
                }
                else if (relativePlayerPostion.x > 0.00 && availableTargets[k] != nearestLockOnTarget)
                {
                    distanceFromRightTarget = Vector3.Distance(nearestLockOnTarget.transform.position, availableTargets[k].transform.position);
                }

                if (relativePlayerPostion.x < 0.00 && distanceFromLeftTarget < shortestDistanceLeftTarget)
                {
                    shortestDistanceLeftTarget = distanceFromLeftTarget;
                    if (availableTargets[k].CanLockOn() && nearestLockOnTarget != availableTargets[k])
                    {
                        leftLockTarget = availableTargets[k];
                    }
                }

                if (relativePlayerPostion.x > 0.00 && distanceFromRightTarget < shortestDistanceRightTarget)
                {
                    shortestDistanceRightTarget = distanceFromRightTarget;
                    if (availableTargets[k].CanLockOn() && nearestLockOnTarget != availableTargets[k])
                    {
                        rightLockTarget = availableTargets[k];
                    }
                }

                if (leftLockTarget != null &&  (Mathf.Round(inputs.look.x) < 0 || Gamepad.current != null && Gamepad.current.rightStick.left.isPressed))
                {
                    if (targetSwitchingCooldown < maxTargetSwitchingCooldown)
                    {
                        return;
                    }

                    SwitchToLeftTarget();
                    targetSwitchingCooldown = 0f;
                    return;
                }
                else if (rightLockTarget != null && (Mathf.Round(inputs.look.x) > 0 || Gamepad.current != null && Gamepad.current.rightStick.right.isPressed))
                {
                    if (targetSwitchingCooldown < maxTargetSwitchingCooldown)
                    {
                        return;
                    }

                    SwitchToRightTarget();
                    targetSwitchingCooldown = 0f;
                    return;
                }


            }
        }

    }

}
