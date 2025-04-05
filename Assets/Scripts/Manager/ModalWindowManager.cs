using System;
using AarquieSolutions.Base.Singleton;
using UnityEngine;

public class ModalWindowManager : Singleton<ModalWindowManager>
{
    private const string ModalWindowPrefabLocation = "Prefabs/Modal Window";
    private const string ModalWindowGameObjectName = "Modal Window";

    private GameObject modalWindowGameObjectReference;
    private GameObject ModalWindowGameObjectReference
    {
        get
        {
            if (modalWindowGameObjectReference == null)
            {
                modalWindowGameObjectReference = GameObject.Find(ModalWindowGameObjectName);
                if (modalWindowGameObjectReference == null)
                {
                    modalWindowGameObjectReference = Instantiate(Resources.Load(ModalWindowPrefabLocation) as GameObject, FindObjectOfType<Canvas>().transform);
                    modalWindowGameObjectReference.name = ModalWindowGameObjectName;
                }
            }
            return modalWindowGameObjectReference;
        }
        set => modalWindowGameObjectReference = value;
    }

    private UIModalWindow modalWindow;
    private UIModalWindow ModalWindow 
    { 
        get
        {
            if (modalWindow == null)
            {
                modalWindow = ModalWindowGameObjectReference.GetComponent<UIModalWindow>();
            }
            return modalWindow;
        }
    }

    public void ShowAcknowledgeWindow(string titleText, string contentText, string acceptButtonText = "Ok", Action acceptAction = null)
    {
        ModalWindow.ShowAcknowldgeWindow(titleText, contentText, acceptButtonText, acceptAction);
    }

    public void ShowAcknowledgeWindowWithImage(string titleText, string contentText, Sprite contentImage, string acceptButtonText = "Ok", Action acceptAction = null)
    {
        ModalWindow.ShowAcknowledgeWindowWithImage(titleText, contentText, contentImage, acceptButtonText, acceptAction);
    }

    public void ShowAcceptDeclineWindow(string titleText, string contentText, string acceptButtonText = "Confirm", string declineButtonText = "Decline", Action acceptAction = null, Action declineAction = null)
    {
        ModalWindow.ShowAcceptDeclineWindow(titleText, contentText, acceptButtonText, declineButtonText, acceptAction, declineAction);
    }

    public void ShowAcceptDeclineWindowWithImage(string titleText, string contentText, Sprite contentImage, string acceptButtonText = "Confirm", string declineButtonText = "Decline", Action acceptAction = null, Action declineAction = null)
    {
        ModalWindow.ShowAcceptDeclineWindowWithImage(titleText, contentText, contentImage, acceptButtonText, declineButtonText, acceptAction, declineAction);
    }

    public void ShowAcceptDeclineAlternateWindow(string titleText, string contentText, string acceptButtonText = "Confirm", string declineButtonText = "Decline", string alternateButtonText = "Maybe", Action acceptAction = null, Action declineAction = null, Action alternateAction = null)
    {
        ModalWindow.ShowAcceptDeclineAlternateWindow(titleText, contentText, acceptButtonText, declineButtonText, alternateButtonText, acceptAction, declineAction, alternateAction);
    }

    public void ShowAcceptDeclineAlternateWindowWithImage(string titleText, string contentText, Sprite contentImage, string acceptButtonText = "Confirm", string declineButtonText = "Decline", string alternateButtonText = "Maybe", Action acceptAction = null, Action declineAction = null, Action alternateAction = null)
    {
        ModalWindow.ShowAcceptDeclineAlternateWindowWithImage(titleText, contentText, contentImage, acceptButtonText, declineButtonText, alternateButtonText, acceptAction, declineAction, alternateAction);
    }
}
