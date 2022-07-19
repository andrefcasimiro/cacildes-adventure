using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

namespace AF
{
    public class UIDocumentBase : InputListener
    {
        SFXManager sfxManager;

        protected UIDocument uiDocument => GetComponent<UIDocument>();

        public VisualElement root;

        [Header("SFX")]
        public AudioClip selectSfx;
        public AudioClip decisionSfx;
        public AudioClip cancelSfx;

        private void Awake()
        {
            sfxManager = FindObjectOfType<SFXManager>(true);
        }

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
            if (selectSfx == null || sfxManager == null)
            {
                return;
            }

            sfxManager.PlaySound(selectSfx, null);
        }
        public void PlayDecisionSfx()
        {
            if (selectSfx == null || sfxManager == null)
            {
                return;
            }

            sfxManager.PlaySound(decisionSfx, null);
        }
        public void PlayCancelSfx()
        {
            if (selectSfx == null || sfxManager == null)
            {
                return;
            }

            sfxManager.PlaySound(cancelSfx, null);
        }
    }
}