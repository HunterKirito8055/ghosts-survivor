using AarquieSolutions.DependencyInjection.ComponentField;
using AarquieSolutions.InspectorAttributes;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlayAudioOnButtonClick : MonoBehaviour
{
    [GetComponent] private Button selfButton;

    [SerializeField] private bool useStringToPlayAudioClip = false;
    [SerializeField] [ShowIf("useStringToPlayAudioClip")]private string sfxToPlay;
    [SerializeField] [HideIf("useStringToPlayAudioClip")]private AudioClip audioClipSFXToPlay;
    
    private void Awake()
    {
        this.InitializeDependencies();
        selfButton.onClick.AddListener(PlayAudio);
    }

    private void PlayAudio()
    {
        if (useStringToPlayAudioClip)
        {
            AudioManager.Instance.PlayUISoundEffects(sfxToPlay);
        }
        else
        {
            AudioManager.Instance.PlayUISoundEffect(audioClipSFXToPlay);
        }
    }

    private void OnDestroy()
    {
        selfButton.onClick.RemoveListener(PlayAudio);
    }
}
