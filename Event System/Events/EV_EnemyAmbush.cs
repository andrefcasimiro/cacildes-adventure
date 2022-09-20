using System.Collections;
using UnityEngine;

namespace AF
{

    public class EV_EnemyAmbush : EventBase
    {
        public GameObject enemy;
        public Animator enemyAnimator;
        public string ambushAnimationName;
        public AudioClip soundClip;
        public AudioSource audioSource;

        public override IEnumerator Dispatch()
        {
            yield return null;

            enemy.SetActive(true);
            enemyAnimator.gameObject.SetActive(true);
            enemyAnimator.Play(ambushAnimationName);

            if (soundClip != null)
            {
                if (audioSource != null)
                {
                    audioSource.PlayOneShot(soundClip);
                }
                else
                {
                    BGMManager.instance.PlaySound(soundClip, audioSource);
                }
            }
        }
    }

}
