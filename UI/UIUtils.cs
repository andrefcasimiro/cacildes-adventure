using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{
    public static class UIUtils
    {
        public static void SetupButton(Button button, UnityAction callback)
        {
            SetupButton(button, callback, null, null, true);
        }

        public static void SetupButton(
            Button button,
            UnityAction callback,
            UnityAction onFocusInCallback,
            UnityAction onFocusOutCallback,
            bool hasPopupAnimation)
        {
            button.RegisterCallback<ClickEvent>(ev =>
            {
                if (hasPopupAnimation)
                {
                    PlayPopAnimation(button);
                }

                Soundbank.instance.PlayUIDecision();
                callback.Invoke();
            });
            button.RegisterCallback<NavigationSubmitEvent>(ev =>
            {
                if (hasPopupAnimation)
                {
                    PlayPopAnimation(button);
                }

                Soundbank.instance.PlayUIDecision();
                callback.Invoke();
            });

            button.RegisterCallback<FocusInEvent>(ev =>
            {
                if (hasPopupAnimation)
                {
                    PlayPopAnimation(button);
                }

                Soundbank.instance.PlayUIHover();
                onFocusInCallback?.Invoke();
            });
            button.RegisterCallback<PointerEnterEvent>(ev =>
            {
                if (hasPopupAnimation)
                {
                    PlayPopAnimation(button);
                }

                Soundbank.instance.PlayUIHover();
                onFocusInCallback?.Invoke();
            });


            button.RegisterCallback<FocusOutEvent>(ev =>
            {
                onFocusOutCallback?.Invoke();
            });
            button.RegisterCallback<PointerOutEvent>(ev =>
            {
                onFocusOutCallback?.Invoke();
            });
        }


        public static void PlayPopAnimation(VisualElement button)
        {
            PlayPopAnimation(button, Vector3.zero);
        }
        public static void PlayPopAnimation(VisualElement button, Vector3 startingScale)
        {
            button.transform.scale = Vector3.one;

            DOTween.To(
                () => startingScale,
                scale => button.transform.scale = scale,
                Vector3.one,
                0.5f
            ).SetEase(Ease.OutElastic);
        }
    }
}
