using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Video;

namespace AF
{
    [System.Serializable]
    public class SwitchRandomMessages
    {

        public LocalizedText message;

        [Header("Switch")]
        public SwitchEntry switchDependant;
        public bool requiredSwitchValueToDisplayMessage;


        [Header("Companion")]
        public Companion companionDependant;

    }

    public class EV_DisplayMessage : EventBase
    {
        [Header("Actor")]
        public Character character;
        public bool isCompanion = false;

        [Header("Message")]
        public LocalizedText message;
        public LocalizedText[] randomMessages;
        public LocalizedText[] negativeReputationMessages;

        [Header("Conditional Random Messages")]
        public SwitchRandomMessages[] switchRandomMessages;


        [Header("Choices")]
        public List<DialogueChoice> choices = new List<DialogueChoice>();

        [Header("Settings")]
        public float textDelay = 0.05f;
        public bool showHint = false;
        public bool facePlayer = false;
        public Transform characterTransform;

        [Header("Conditions - Switches")]
        public SwitchEntry switchEntry;
        public bool switchValue;

        [Header("Conditions - Variables")]
        public VariableEntry variableEntry;
        public bool mustEqualVariableValue = true;
        public float requiredVariableValue = -1;

        [Header("Animations")]
        public Animator animator;
        public string animationClip;

        DialogueManager dialogueManager;

        [Header("Companion Settings")]
        public bool shouldCheckIfAnEnemyWasKilledAndCommentOnThat = false;

        [Header("Edge Cases")]
        public bool isArenaFinalScoreMessage;

        [Header("Show Image")]
        public Texture2D image;

        [Header("Show Video")]
        public VideoPlayer video;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;

        private void Awake()
        {
            dialogueManager = FindObjectOfType<DialogueManager>(true);

            if (isCompanion)
            {
                character = GetComponentInParent<CompanionManager>(true).companion.character;
            }
        }


        public override IEnumerator Dispatch()
        {
            bool skip = false;

            if (switchEntry != null)
            {
                // If depends on switch, evaluate value:
                ; if (SwitchManager.instance.GetSwitchCurrentValue(switchEntry) == switchValue)
                {
                    skip = false;
                }
                else
                {
                    skip = true;
                }
            }

            if (variableEntry != null)
            {
                // If depends on variable, evaluate value:
                var variableValue = VariableManager.instance.GetVariableValue(variableEntry);

                if (
                    mustEqualVariableValue && variableValue == requiredVariableValue
                    || mustEqualVariableValue == false && variableValue != requiredVariableValue)
                {
                    skip = false;
                }
                else
                {
                    skip = true;
                }
            }

            if (skip == false)
            {
                // In the case where we have enemies like Petra that may have very nested events in them, sometimes dialogue manager is null on Awake
                if (dialogueManager == null)
                {
                    dialogueManager = FindObjectOfType<DialogueManager>(true);
                }

                if (facePlayer)
                {
                    FacePlayer();
                }

                if (!string.IsNullOrEmpty(animationClip))
                {
                    animator.Play(animationClip);
                }

                List<LocalizedText> randomMessagesTable = randomMessages.ToList();
                if (switchRandomMessages != null && switchRandomMessages.Length > 0)
                {
                    foreach (var switchRandomMessage in switchRandomMessages)
                    {
                        //First check if switch allows message
                        bool shouldAdd = switchRandomMessage.switchDependant == null
|| SwitchManager.instance.GetSwitchCurrentValue(switchRandomMessage.switchDependant) == switchRandomMessage.requiredSwitchValueToDisplayMessage;

                        //  then check if an additional companion is required
                        if (shouldAdd && switchRandomMessage.companionDependant != null)
                        {
                            // shouldAdd = Player.instance.companions.Exists(x => x.companionId == switchRandomMessage.companionDependant.companionId);
                        }

                        if (shouldAdd)
                        {
                            randomMessagesTable.Add(switchRandomMessage.message);
                        }
                    }
                }

                var displayMessage = message;

                if (randomMessagesTable.Count > 0)
                {
                    var idx = Random.Range(0, randomMessagesTable.Count);
                    displayMessage = randomMessagesTable[idx];
                }
                else if (negativeReputationMessages != null && negativeReputationMessages.Length > 0)
                {
                    if (playerStatsDatabase.GetCurrentReputation() < 0)
                    {
                        var idx = Random.Range(0, negativeReputationMessages.Length);
                        displayMessage = negativeReputationMessages[idx];
                    }

                }

                if (shouldCheckIfAnEnemyWasKilledAndCommentOnThat && Player.instance.lastEnemyKilled != null)
                {

                    if (character.name == "Alcino" && Player.instance.lastEnemyKilled.alcinoCommentsUponKillingEnemy.localizedTexts.Count() > 0)
                    {
                        displayMessage = Player.instance.lastEnemyKilled.alcinoCommentsUponKillingEnemy;
                    }
                    else if (character.name == "Hugo" && Player.instance.lastEnemyKilled.hugoCommentsOnKillingEnemy.localizedTexts.Count() > 0)
                    {
                        displayMessage = Player.instance.lastEnemyKilled.hugoCommentsOnKillingEnemy;
                    }
                    else if (character.name == "Balbino" && Player.instance.lastEnemyKilled.balbinoCommentsOnKillingEnemy.localizedTexts.Count() > 0)
                    {
                        displayMessage = Player.instance.lastEnemyKilled.balbinoCommentsOnKillingEnemy;
                    }

                    Player.instance.lastEnemyKilled = null;
                }

                LocalizedText messageToShow = displayMessage;
                if (isArenaFinalScoreMessage)
                {
                    var roundManager = FindAnyObjectByType<RoundManager>(FindObjectsInactive.Include);
                    bool hasWon = true;

                    messageToShow.localizedTexts[0].gameLanguage = GamePreferences.GameLanguage.PORTUGUESE;
                    messageToShow.localizedTexts[1].gameLanguage = GamePreferences.GameLanguage.ENGLISH;

                    if (hasWon)
                    {
                        messageToShow.localizedTexts[0].text = $"Parabéns! Bateste um novo recorde. Aguentaste {roundManager.currentRound} rondas durante um tempo total de {roundManager.GetFormmatedTimed(roundManager.elapsedTime)}!\n Lutaste ferozmente, e agora é hora de voltar!";
                        messageToShow.localizedTexts[1].text = $"Congratulations! You've beaten a new record. You held your ground for {roundManager.currentRound} rounds during a total time of {roundManager.GetFormmatedTimed(roundManager.elapsedTime)}!\n You fought bravely and now is time to return.";
                    }

                }

                yield return dialogueManager.ShowDialogueWithChoices(
                    character, displayMessage, choices, textDelay, showHint, image, video);


                if (!string.IsNullOrEmpty(animationClip))
                {
                    animator.Play("NoGesture");
                }
            }
        }

        void FacePlayer()
        {
            if (characterTransform == null)
            {
                characterTransform = this.transform;
            }

            Transform player = GameObject.FindWithTag("Player").transform;

            var npcLookPos = player.transform.position - characterTransform.transform.position;
            npcLookPos.y = 0;

            characterTransform.transform.rotation = Quaternion.LookRotation(npcLookPos);
        }
    }
}
