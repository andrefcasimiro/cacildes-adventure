using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class PlayerCameraHelper : MonoBehaviour
    {
        public float defaultCameraDistance = 4;

        public float zoomValue = 3;

        public Cinemachine.CinemachineVirtualCamera playerCamera;



        public void ZoomIn()
        {
            CinemachineComponentBase componentBase = playerCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);

            if (defaultCameraDistance == -1)
            {

                defaultCameraDistance = (componentBase as Cinemachine3rdPersonFollow).CameraDistance;

            }

            (componentBase as Cinemachine3rdPersonFollow).CameraDistance = zoomValue;

        }

        public void ResetZoom()
        {

            CinemachineComponentBase componentBase = playerCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);

            (componentBase as Cinemachine3rdPersonFollow).CameraDistance = defaultCameraDistance;
        }
    }

}
