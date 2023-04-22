using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class DisablePlayOnAwakeOnStart : MonoBehaviour
    {
        AudioSource audioSource => GetComponent<AudioSource>();

        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnDisable()
        {
            audioSource.playOnAwake = false;
        }
    }

}
