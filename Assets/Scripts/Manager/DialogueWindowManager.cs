using System;
using AarquieSolutions.Base.Singleton;
using UnityEngine;

public class DialogueWindowManager : Singleton<DialogueWindowManager>
{
    private const string DialogueWindowPrefabLocation = "Prefabs/Dialogue Window";
    private const string DialogueWindowGameObjectName = "Dialogue Window";

    private GameObject dialogueWindowGameObjectReference;
    private GameObject DialogueWindowGameObjectReference
    {
        get
        {
            if (dialogueWindowGameObjectReference == null)
            {
                dialogueWindowGameObjectReference = GameObject.Find(DialogueWindowGameObjectName);
                if (dialogueWindowGameObjectReference == null)
                {
                    dialogueWindowGameObjectReference = Instantiate(Resources.Load(DialogueWindowPrefabLocation) as GameObject, FindObjectOfType<Canvas>().transform);
                    dialogueWindowGameObjectReference.name = DialogueWindowGameObjectName;
                }
            }
            return dialogueWindowGameObjectReference;
        }
        set => dialogueWindowGameObjectReference = value;
    }

    private UIDialogueWindow dialogueWindow;
    private UIDialogueWindow DialogueWindow 
    { 
        get
        {
            if (dialogueWindow == null)
            {
                dialogueWindow = DialogueWindowGameObjectReference.GetComponent<UIDialogueWindow>();
            }
            return dialogueWindow;
        }
    }
    
    public void ShowAcknowledgeWindow(string titleText, string contentText, string acceptButtonText = "Ok", Action acceptAction = null)
    {
        DialogueWindow.ShowAcknowledgeWindow(titleText, contentText, acceptButtonText, acceptAction);
    }
    
    public void ShowAcceptDeclineWindow(string titleText, string contentText, string acceptButtonText = "Confirm", string declineButtonText = "Decline", Action acceptAction = null, Action declineAction = null)
    {
        DialogueWindow.ShowAcceptDeclineWindow(titleText, contentText, acceptButtonText, declineButtonText, acceptAction, declineAction);
    }
}
