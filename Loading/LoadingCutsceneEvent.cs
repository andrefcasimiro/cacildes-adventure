using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace AF
{
    [System.Serializable]
    public class LoadingCutsceneMoment
    {
        public AudioClip musicToPlay;

        public Sprite image;

        public string actorName = "";

        [TextArea]
        public string message = "";

        public float waitToAppear = 0.1f;
    }

    public class LoadingCutsceneEvent : InputListener
    {
        public List<LoadingCutsceneMoment> events = new List<LoadingCutsceneMoment>();

        public string sceneName;

        AsyncOperation sceneLoadingOperation;

        public UnityEvent eventOnCutsceneEnding;

        UIDocument uiDocument => GetComponent<UIDocument>();

        VisualElement slideshow;

        DialogueManager dialogueManager;

        private void Awake()
        {
            dialogueManager = FindObjectOfType<DialogueManager>(true);
        }

        private void Start()
        {
            slideshow = uiDocument.rootVisualElement.Q<VisualElement>("Container");

            sceneLoadingOperation = SceneManager.LoadSceneAsync(sceneName);
            sceneLoadingOperation.allowSceneActivation = false;

            StartCoroutine(Begin());
        }


        IEnumerator Begin()
        {
            yield return null;

            foreach (LoadingCutsceneMoment ev in events)
            {
                yield return new WaitUntil(() => hasPressedConfirmButton == false);

                yield return new WaitForSeconds(ev.waitToAppear);

                if (ev.musicToPlay != null)
                {
                    BGMManager.instance.PlayMusic(ev.musicToPlay);
                }

                Background b = new Background();
                b.sprite = ev.image;
                slideshow.style.backgroundImage = b;

                yield return dialogueManager.ShowDialogue(ev.actorName, ev.message);

                slideshow.style.backgroundImage = null;
            }


            yield return new WaitUntil(() => sceneLoadingOperation.progress >= 0.9f);

            yield return null;

            LoadNextScene();

        }

        public void LoadNextScene()
        {
            MapManager.instance.SetSpawnGameObjectNameRef("Initial Spawnpoint");

            sceneLoadingOperation.allowSceneActivation = true;
        }

    }

}
