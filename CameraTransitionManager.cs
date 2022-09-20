using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class CameraTransitionManager : MonoBehaviour
    {
        public void SetTransition(float amount)
        {
            GetComponent<Cinemachine.CinemachineBrain>().m_DefaultBlend.m_Time = amount;
        }
    }

}