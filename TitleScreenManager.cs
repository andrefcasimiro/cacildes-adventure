using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (Player.instance.hasShownTitleScreen)
            {
                this.gameObject.SetActive(false);
                return;
            }
            else
            {
                Player.instance.hasShownTitleScreen = true;
            }


            FindObjectOfType<UIBlackCanvas>(true).gameObject.SetActive(true);

            playerComponentManager = FindObjectOfType<PlayerComponentManager>(true);

            playerComponentManager.DisableCharacterController();
            playerComponentManager.DisableComponents();

            playerAnimator = playerComponentManager.GetComponent<Animator>();
            playerAnimator.transform.position = playerSleepTransform.position;
            playerAnimator.transform.rotation = playerSleepTransform.rotation;

            playerAnimator.SetBool(hashIsSleeping, true);
        }

        private void Start()
        {
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
            StartCoroutine(OnGameStart());
        }

        public IEnumerator OnGameStart()
        {
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
            this.gameObject.SetActive(false);
        }
    }

}
