using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AF
{
    public class TitleScreenManager : MonoBehaviour
    {
        public readonly int hashIsSleeping = Animator.StringToHash("IsSleeping");

        PlayerComponentManager playerComponentManager;
        Animator playerAnimator;

        public Transform playerSleepTransform;
        public Transform playerAwakeTransform;

        public int gameStartHour = 11;

        private void Awake()
        {
        }

        private void Start()
        {
            if (Player.instance.hasShownTitleScreen)
            {
                this.gameObject.SetActive(false);
                return;
            }

            if (Gamepad.current != null)
            {
                FindObjectOfType<GamepadCursor>(true).gameObject.SetActive(true);
            }

            FindObjectOfType<UIBlackCanvas>(true).gameObject.SetActive(true);

            playerComponentManager = FindObjectOfType<PlayerComponentManager>(true);

            playerComponentManager.DisableCharacterController();
            playerComponentManager.DisableComponents();

            playerAnimator = playerComponentManager.GetComponent<Animator>();
            playerAnimator.transform.position = playerSleepTransform.position;
            playerAnimator.transform.rotation = playerSleepTransform.rotation;

            playerAnimator.SetBool(hashIsSleeping, true);

            FindObjectOfType<UIBlackCanvas>(true).StartFade();

            StartCoroutine(SetTimeOfDay());
        }

        IEnumerator SetTimeOfDay()
        {
            yield return new WaitForSeconds(0.1f);

            FindObjectOfType<DayNightManager>(true).SetTimeOfDay(gameStartHour, 0);
        }

        public void StartGame()
        {
            Player.instance.hasShownTitleScreen = true;

            StartCoroutine(OnGameStart());
        }

        public IEnumerator OnGameStart()
        {
            FindObjectOfType<GamepadCursor>(true).gameObject.SetActive(false);

            EventBase[] events = GetComponents<EventBase>();

            foreach (EventBase ev in events)
            {
                if (ev != null)
                {
                    yield return StartCoroutine(ev.Dispatch());
                }
            }

            playerAnimator.SetBool(hashIsSleeping, false);
            playerAnimator.transform.position = playerAwakeTransform.position;
            playerComponentManager.EnableCharacterController();
            playerComponentManager.EnableComponents();

            // Save game
            yield return new WaitForEndOfFrame();
            SaveSystem.instance.SaveGameData("autoSave");

            yield return new WaitForEndOfFrame();

            this.gameObject.SetActive(false);
        }
    }

}
