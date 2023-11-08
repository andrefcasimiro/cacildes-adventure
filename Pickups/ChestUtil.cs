using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Pickups
{
    public class ChestUtil : MonoBehaviour
    {

        [Header("Jump Parameters")]
        public float jumpPower = 2f;
        public int jumpCount = 1;
        public float jumpDuration = 0.3f;

        [Header("Scale Parameters")]
        public Vector3 endScale = new Vector3(1.1f, 1.1f, 1.1f); // Slightly larger scale during the animation
        public float scaleDuration = 0.15f;

        [Header("Events")]
        public UnityEvent onAnimationBegin;
        public UnityEvent onAnimationEnd;
        [Header("Animation Settings")]
        public float animationDuration = .3f;

        private void AnimateChest()
        {
            // Initial position and scale of the chest
            Vector3 originalPosition = transform.position;
            Vector3 originalScale = transform.localScale;

            // Sequence for the chest animation
            Sequence chestSequence = DOTween.Sequence();

            // Jump and scale up
            chestSequence.Append(transform.DOJump(originalPosition, jumpPower, jumpCount, jumpDuration).SetEase(Ease.OutQuad));
            chestSequence.Join(transform.DOScale(endScale, scaleDuration).SetEase(Ease.OutQuad));

            // Drop and scale down
            chestSequence.Append(transform.DOJump(originalPosition, 0.1f, 1, jumpDuration).SetEase(Ease.InOutQuad));
            chestSequence.Join(transform.DOScale(originalScale, scaleDuration).SetEase(Ease.InQuad));

            // Start the sequence
            chestSequence.Play();
        }

        public void Trigger()
        {
            onAnimationBegin?.Invoke();

            AnimateChest();

            Invoke(nameof(InvokeAnimationEnd), animationDuration);
        }

        void InvokeAnimationEnd()
        {
            onAnimationEnd?.Invoke();
        }

    }

}
