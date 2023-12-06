using DG.Tweening;
using UnityEngine;
namespace AF.Dialogue
{

    public class GreetingMessage : MonoBehaviour
    {
        [Header("Settings")]
        [TextArea] public string greetingMessage;

        [Header("Components")]
        public CanvasGroup canvasGroup;
        public TMPro.TextMeshProUGUI textMeshPro;

        private void Awake()
        {
            canvasGroup.alpha = 0;
        }

        public void Display()
        {
            textMeshPro.text = greetingMessage;

            canvasGroup.DOFade(1f, 0.5f);
        }

        public void Hide()
        {
            canvasGroup.DOFade(0f, 0.5f);
        }
    }
}
