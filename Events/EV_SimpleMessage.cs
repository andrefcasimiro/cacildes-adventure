using System.Collections;
using AF;
using UnityEngine;

public class EV_SimpleMessage : EventBase
{

    [Header("Components")]
    public DialogueManager dialogueManager;

    [Header("Actor")]
    public Character character;

    [TextArea]
    public string message;

    public override IEnumerator Dispatch()
    {

        // TODO: For now, keep this wrapper until we remove localization

        LocalizedTextEntry enLocalizedTextEntry = new()
        {
            gameLanguage = AF.GamePreferences.GameLanguage.ENGLISH,
            text = message,
        };
        LocalizedTextEntry ptLocalizedTextEntry = new()
        {
            gameLanguage = AF.GamePreferences.GameLanguage.PORTUGUESE,
            text = message,
        };

        LocalizedText localizedText = new LocalizedText()
        {
            localizedTexts = new LocalizedTextEntry[] { enLocalizedTextEntry, ptLocalizedTextEntry }
        };

        yield return dialogueManager.ShowDialogueWithChoices(
            character, localizedText, null, 0.05f, false, null, null);

    }
}
