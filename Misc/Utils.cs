using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

namespace AF
{

    public static class Utils
    {

        public static void FaceTarget(Transform origin, Transform target)
        {
            var lookPos = target.position - origin.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);

            origin.rotation = rotation;
        }

        public static void ShowCursor()
        {
            if (Gamepad.current == null)
            {
                Cursor.visible = true;
            }
            else
            {
                Cursor.visible = false;

                MonoBehaviour.FindObjectOfType<GamepadCursor>(true).gameObject.SetActive(true);
            }

            Cursor.lockState = CursorLockMode.None;
        }

        public static void HideCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public static Vector3 GetNearestNavMeshPoint(Vector3 reference)
        {
            // Teleport near player
            NavMeshHit hit;
            NavMesh.SamplePosition(reference, out hit, Mathf.Infinity, NavMesh.AllAreas);

            return hit.position;
        }

    }

}
