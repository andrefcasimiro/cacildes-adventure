using DG.Tweening;
using UnityEngine;
namespace AF.Dialogue
{

    public class GreetingMessageUI : MonoBehaviour
    {

        [Header("Components")]
        public CanvasGroup canvasGroup;
        public TMPro.TextMeshProUGUI textMeshPro;

        private void Awake()
        {
            canvasGroup.alpha = 0;
        }

        public void Display(string greetingMessage)
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
