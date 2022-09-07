using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    public class DefaultAnimation : MonoBehaviour
    {
        Animator animator => GetComponent<Animator>();

        public string animationName;

        private void Start()
        {
            if (animator != null)
            {
                animator.Play(animationName);
            }
        }
    }

}
