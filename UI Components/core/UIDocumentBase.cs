using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

namespace AF
{
    public class UIDocumentBase : InputListener
    {
        protected UIDocument uiDocument => GetComponent<UIDocument>();

        public VisualElement root;

        [Header("SFX")]
        public AudioClip selectSfx;
        public AudioClip decisionSfx;
        public AudioClip cancelSfx;

        protected virtual void Start()
        {
            this.root = uiDocument.rootVisualElement;

            // Add keyboard movement sounds 
            this.root.RegisterCallback<NavigationMoveEvent>(ev =>
            {
                PlaySelectSfx();
            });
        }

        public void SetupButtonClick(Button btn, UnityAction callback)
        {
            btn.RegisterCallback<NavigationSubmitEvent>(ev =>
            {
                PlayDecisionSfx();

                callback.Invoke();
            });

            btn.RegisterCallback<ClickEvent>(ev =>
            {
                PlayDecisionSfx();

                callback.Invoke();
            });
        }

        public virtual void Enable()
        {
            this.root.RemoveFromClassList("hide");
        }

        public virtual void Disable()
        {
            this.root.AddToClassList("hide");
        }

        public bool IsVisible()
        {
            if (this.root == null)
            {
                return false;
            }

            return this.root.ClassListContains("hide") == false;
        }

        public void PlaySelectSfx()
        {
            BGMManager.instance.PlaySound(selectSfx, null);
        }
        public void PlayDecisionSfx()
        {
            BGMManager.instance.PlaySound(decisionSfx, null);
        }
        public void PlayCancelSfx()
        {
            BGMManager.instance.PlaySound(cancelSfx, null);
        }
    }
}