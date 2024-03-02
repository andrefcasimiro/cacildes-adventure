using System.Collections;
using System.Linq;
using AF;
using AF.Dialogue;
using UnityEngine;

public class EV_SimpleMessage : EventBase
{

    [Header("UI (Assignment is optional, but recommended)")]
    public UIDocumentDialogueWindow uIDocumentDialogueWindow;

    [Header("Actor")]
    public Character character;

    [TextAreaAttribute(minLines: 10, maxLines: 20)]
    public string message;

    [Header("Responses")]
    public Response[] responses;

    public override IEnumerator Dispatch()
    {
        // Only consider responses that are active - we hide responses based on composition of nested objects
        Response[] filteredResponses = responses.Where(response => response.gameObject.activeInHierarchy).ToArray();

        yield return GetUIDocumentDialogueWindow().DisplayMessage(
            character, message, filteredResponses);
    }

    UIDocumentDialogueWindow GetUIDocumentDialogueWindow()
    {
        if (uIDocumentDialogueWindow == null)
        {
            uIDocumentDialogueWindow = FindAnyObjectByType<UIDocumentDialogueWindow>(FindObjectsInactive.Include);
        }

        return uIDocumentDialogueWindow;
    }
}
