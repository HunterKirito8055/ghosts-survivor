using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIModalWindow : MonoBehaviour
{
    [Header("Header")]
    [SerializeField] private Text titleText;

    [Header("Content")]
    [SerializeField] private Text contentText;
    [SerializeField] private Image contentImage;

    [Header("Footer")]
    [SerializeField] private Button acceptButton;
    [SerializeField] private Image acceptButtonColor;
    [SerializeField] private Text acceptButtonText;
    [SerializeField] private Button declineButton;
    [SerializeField] private Image declineButtonColor;
    [SerializeField] private Text declineButtonText;
    [SerializeField] private Button alternateButton;
    [SerializeField] private Image alternateButtonColor;
    [SerializeField] private Text alternateButtonText;

    private event Action OnAcceptButtonClicked;
    private event Action OnDeclineButtonClicked;
    private event Action OnAlternateButtonClicked;

    private void Awake()
    {
        acceptButton.onClick.AddListener(AcceptButtonClicked);
        declineButton.onClick.AddListener(DeclineButtonClicked);
        alternateButton.onClick.AddListener(AlternateButtonClicked);

        ResetModalWindow();
    }

    private void ShowWindow()
    {
        gameObject.SetActive(true);
    }

    public void ResetModalWindow()
    {
        contentImage.gameObject.SetActive(false);
        declineButton.gameObject.SetActive(false);
        alternateButton.gameObject.SetActive(false);
    }

    public void ShowAcknowldgeWindow(string titleText, string contentText, string acceptButtonText , Action acceptAction)
    {
        ShowWindow();
        ResetModalWindow();
        this.titleText.text = titleText;
        this.contentText.text = contentText;
        this.acceptButtonText.text = acceptButtonText;
        OnAcceptButtonClicked = acceptAction;
    }

    public void ShowAcknowledgeWindowWithImage(string titleText, string contentText, Sprite contentImage, string acceptButtonText, Action acceptAction)
    {
        ShowAcknowldgeWindow(titleText, contentText, acceptButtonText, acceptAction);

        this.contentImage.gameObject.SetActive(true);
        this.contentImage.sprite = contentImage;
    }

    public void ShowAcceptDeclineWindow(string titleText, string contentText, string acceptButtonText, string declineButtonText , Action acceptAction , Action declineAction )
    {
        ShowAcknowldgeWindow(titleText, contentText, acceptButtonText, acceptAction);

        declineButton.gameObject.SetActive(true);
        this.declineButtonText.text = declineButtonText;
        OnDeclineButtonClicked = declineAction;
    }

    public void ShowAcceptDeclineWindowWithImage(string titleText, string contentText, Sprite contentImage, string acceptButtonText, string declineButtonText, Action acceptAction, Action declineAction)
    {
        ShowAcceptDeclineWindow(titleText, contentText, acceptButtonText, declineButtonText, acceptAction, declineAction);

        this.contentImage.gameObject.SetActive(true);
        this.contentImage.sprite = contentImage;
    }

    public void ShowAcceptDeclineAlternateWindow(string titleText, string contentText, string acceptButtonText, string declineButtonText, string alternateButtonText, Action acceptAction, Action declineAction, Action alternateAction)
    {
        ShowAcceptDeclineWindow(titleText, contentText, acceptButtonText, declineButtonText, acceptAction, declineAction);

        alternateButton.gameObject.SetActive(true);
        this.alternateButtonText.text = alternateButtonText;
        OnAlternateButtonClicked = alternateAction;
    }

    public void ShowAcceptDeclineAlternateWindowWithImage(string titleText, string contentText, Sprite contentImage, string acceptButtonText, string declineButtonText, string alternateButtonText, Action acceptAction, Action declineAction, Action alternateAction)
    {
        ShowAcceptDeclineAlternateWindow(titleText, contentText, acceptButtonText, declineButtonText, alternateButtonText, acceptAction, declineAction, alternateAction);

        this.contentImage.gameObject.SetActive(true);
        this.contentImage.sprite = contentImage;
    }

    private void AcceptButtonClicked()
    {
        OnAcceptButtonClicked?.Invoke();
        Close();
    }

    private void DeclineButtonClicked()
    {
        OnDeclineButtonClicked?.Invoke();
        Close();
    }

    private void AlternateButtonClicked()
    {
        OnAlternateButtonClicked?.Invoke();
        Close();
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        acceptButton.onClick.RemoveAllListeners();
        declineButton.onClick.RemoveAllListeners();
        alternateButton.onClick.RemoveAllListeners();
    }
}
