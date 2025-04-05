using System.Threading.Tasks;
using AarquieSolutions.SettingsSystem;
using UnityEngine;

public class SettingsManager
{
    public static Setting<float> masterVolume;
    public static Setting<float> sfxVolume;
    public static Setting<float> bgmVolume;

    [RuntimeInitializeOnLoadMethod((RuntimeInitializeLoadType.AfterSceneLoad))]
    public static void InitializeSettings()
    {
        LoadAudioSettings();
    }

    private static async void LoadAudioSettings()
    {
        //a delay is added since Unity doesn't allow setting values for Audio Mixers on Awake
        await Task.Delay((int)(Time.deltaTime * 1000)); //converting seconds to milliseconds before using it

        masterVolume = new Setting<float>("MasterVolume", 1f, AudioManager.Instance.SetMasterVolume);
        sfxVolume = new Setting<float>("SFXVolume", 1f, AudioManager.Instance.SetSoundEffectsVolume);
        bgmVolume = new Setting<float>("BGMVolume", 1f, AudioManager.Instance.SetBackgroundMusicVolume);
    }
}