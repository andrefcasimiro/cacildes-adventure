using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class EV_Moment : EventBase
    {
        [Header("Actor")]
        public Character character;

        [Header("Message")]
        public LocalizedText positiveReputationMessage;
        public LocalizedText negativeReputationMessage;

        [Header("Choices")]
        public List<DialogueChoice> choices = new();

        [Header("Conditions - Skip Moment On Switch Value")]
        public SwitchEntry switchEntry;
        public bool switchValue;

        [Header("Conditions - Require Companion In Party")]
        [Tooltip("This moment only happens if the given companion is in the party")]
        public Companion companion;
        [Tooltip("If set, this moment only happens if both companions are in the party - think exchange between the two")]
        public Companion companion2;

        [Header("Animations")]
        public Animator animator;
        public string animationClip;

        [Header("Camera Priority")]
        public Cinemachine.CinemachineVirtualCamera desiredCamera;
        public bool usePlayerCameraBefore = false;
        public bool usePlayerCameraAfter = false;

        Cinemachine.CinemachineVirtualCamera previousCamera;

        [Header("Delay")]
        public float startDelay = 0f;
        public float endDelay = 0f;

        [Header("Music")]
        public AudioClip music;
        public bool playMapMusic;
        public bool stopMusic;

        [Header("SFX")]
        public AudioClip soundOnStart;

        [Header("Direction")]
        public Transform target;
        public Transform lookDirection;
        public bool facePlayer;
        Quaternion originalRotation;
        public bool resetOriginalRotationAfter = true;

        [Header("Events")]
        public UnityEvent onStart;
        public UnityEvent onEnd;

        // Components
        DialogueManager dialogueManager;

        Vector3 originalDesiredCameraPosition;
        Quaternion originalDesiredCameraRotatin;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        private void Awake()
        {
            dialogueManager = FindObjectOfType<DialogueManager>(true);

            if (desiredCamera != null)
            {
                this.originalDesiredCameraPosition = desiredCamera.transform.position;
                this.originalDesiredCameraRotatin = desiredCamera.transform.rotation;
            }
        }

        public override IEnumerator Dispatch()
        {
            yield return null;


            if (switchEntry != null && SwitchManager.instance.GetSwitchCurrentValue(switchEntry) != switchValue)
            {
                yield break;
            }

            if (companion != null)
            {
                //var desiredCompanion = Player.instance.companions.FirstOrDefault(x => x.companionId == companion.companionId);

                /*
                                if (desiredCompanion == null || desiredCompanion.isWaitingForPlayer)
                                {
                                    yield break;
                                }

                                if (companion2 != null)
                                {
                                    var desiredCompanion2 = Player.instance.companions.FirstOrDefault(x => x.companionId == companion2.companionId);

                                    if (desiredCompanion2 == null || desiredCompanion2.isWaitingForPlayer)
                                    {
                                        yield break;
                                    }
                                }*/
            }

            // Conditions have been met, run moment
            yield return new WaitForSeconds(startDelay);


            if (lookDirection != null || facePlayer && target != null)
            {
                originalRotation = target.transform.rotation;

                var lookDirection = transform;

                if (facePlayer)
                {
                    lookDirection = GameObject.FindWithTag("Player").transform;
                }

                var lookPos = lookDirection.transform.position - target.transform.position;
                lookPos.y = 0;
                target.transform.rotation = Quaternion.LookRotation(lookPos);
            }

            // Check sound
            if (soundOnStart != null)
            {
                BGMManager.instance.PlaySound(soundOnStart, null);
            }

            // Check music
            if (playMapMusic)
            {
                SceneSettings sceneSettings = FindObjectOfType<SceneSettings>(true);
                sceneSettings.HandleSceneSound();
            }
            else if (music != null)
            {
                BGMManager.instance.PlayMusic(music);
            }
            else if (stopMusic)
            {
                BGMManager.instance.StopMusic();
            }

            // Check camera operations first
            if (desiredCamera != null)
            {
                var currentCam = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera;
                if (currentCam != null)
                {
                    currentCam.Priority = 1;
                }


                desiredCamera.transform.position = originalDesiredCameraPosition;
                desiredCamera.transform.rotation = originalDesiredCameraRotatin;

                desiredCamera.Priority = 999;
            }
            else if (usePlayerCameraBefore)
            {
                var currentCam = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera;
                if (currentCam != null)
                {
                    currentCam.Priority = 1;
                }

                FindObjectOfType<PlayerCamera>(true).GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = 10;
            }

            // Animator Logic
            if (animator != null && !string.IsNullOrEmpty(animationClip))
            {
                animator.Play(animationClip);
            }

            // Event logic
            onStart.Invoke();

            LocalizedText message = positiveReputationMessage;
            if (negativeReputationMessage.localizedTexts != null && negativeReputationMessage.localizedTexts.Count() > 0)
            {
                if (playerStatsDatabase.GetCurrentReputation() < 0)
                {
                    message = negativeReputationMessage;
                }
            }


            yield return dialogueManager.ShowDialogueWithChoices(character, message, choices, 0.05f, false, null, null);

            yield return EndMoment();
        }

        IEnumerator EndMoment()
        {
            yield return null;

            if (!string.IsNullOrEmpty(animationClip))
            {
                animator.Play("NoGesture");
            }


            if (lookDirection != null || facePlayer && target != null)
            {
                if (resetOriginalRotationAfter)
                {
                    target.transform.rotation = originalRotation;
                }
            }

            if (usePlayerCameraAfter)
            {
                var currentCam = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera;
                if (currentCam != null)
                {
                    currentCam.Priority = 1;
                }

                FindObjectOfType<PlayerCamera>(true).GetComponent<Cinemachine.CinemachineVirtualCamera>().Priority = 10;
            }

            // On end
            onEnd.Invoke();

            yield return new WaitForSeconds(endDelay);
        }



    }

}