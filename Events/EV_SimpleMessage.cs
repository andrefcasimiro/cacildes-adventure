using System.Collections;
using System.Linq;
using AF;
using AF.Dialogue;
using UnityEngine;

public class EV_SimpleMessage : EventBase
{

    [Header("Components")]
    public UIDocumentDialogueWindow uIDocumentDialogueWindow;

    [Header("Actor")]
    public Character character;

    [TextArea]
    public string message;

    [Header("Responses")]
    public Response[] responses;

    public override IEnumerator Dispatch()
    {
        // Only consider responses that are active - we hide responses based on composition of nested objects
        Response[] filteredResponses = responses.Where(response => response.gameObject.activeInHierarchy).ToArray();

        yield return uIDocumentDialogueWindow.DisplayMessage(
            character, message, filteredResponses);
    }
}
