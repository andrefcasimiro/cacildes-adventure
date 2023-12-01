using DG.Tweening;
using UnityEngine;
namespace AF.Dialogue
{

    public class GreetingMessage : MonoBehaviour
    {
        [Header("Settings")]
        [TextArea] public string greetingMessage;

        [Header("Components")]
        public CanvasGroup content;
        public TMPro.TextMeshProUGUI textMeshPro;

        public void Display()
        {
            textMeshPro.text = greetingMessage;

            content.DOFade(1f, 0.5f);
        }

        public void Hide()
        {
            content.DOFade(0f, 0.5f);

        }
    }
}
