using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDialogueWindow : MonoBehaviour
{
    [SerializeField] private Text titleText;
    [SerializeField] private Text contentText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Text acceptButtonText;
    [SerializeField] private Button declineButton;
    [SerializeField] private Text declineButtonText;

    
    private event Action OnAcceptButtonClicked;
    private event Action OnDeclineButtonClicked;
    
    private void Awake()
    {
        ResetModalWindow();
        acceptButton.onClick.AddListener(AcceptButtonClicked);
        declineButton.onClick.AddListener(DeclineButtonClicked);
    }

    private void ShowWindow()
    {
        gameObject.SetActive(true);
    }

    private void CloseWindow()
    {
        gameObject.SetActive(false);
    }

    public void ShowAcknowledgeWindow(string titleText, string contentText, string acceptButtonText , Action acceptAction)
    {
        ShowWindow();
        ResetModalWindow();
        this.titleText.text = titleText;
        this.contentText.text = contentText;
        this.acceptButtonText.text = acceptButtonText;
        OnAcceptButtonClicked = acceptAction;
    }
    
    public void ShowAcceptDeclineWindow(string titleText, string contentText, string acceptButtonText, string declineButtonText , Action acceptAction , Action declineAction )
    {
        ShowAcknowledgeWindow(titleText, contentText, acceptButtonText, acceptAction);

        declineButton.gameObject.SetActive(true);
        this.declineButtonText.text = declineButtonText;
        OnDeclineButtonClicked = declineAction;
    }

    private void ResetModalWindow()
    {
        OnAcceptButtonClicked = null;
        OnDeclineButtonClicked = null;
        declineButton.gameObject.SetActive(false);
    }
    
    private void AcceptButtonClicked()
    {
        OnAcceptButtonClicked?.Invoke();
        CloseWindow();
    }

    private void DeclineButtonClicked()
    {
        OnDeclineButtonClicked?.Invoke();
        CloseWindow();
    }
    
    private void OnDestroy()
    {
        acceptButton.onClick.RemoveAllListeners();
        declineButton.onClick.RemoveAllListeners();
    }
}
