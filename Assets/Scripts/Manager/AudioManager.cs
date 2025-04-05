using System.Collections.Generic;
using AarquieSolutions.Base.Singleton;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class AudioManager : Singleton<AudioManager>
{
    private const string Master_Volume_Exposed_Parameter = "Master Volume";
    private const string Sound_Effects_Volume_Exposed_Parameter = "SFX Volume";
    private const string Background_Music_Volume_Exposed_Parameter = "BGM Volume";

    //there are limitations to how many AudioSources can be played simultaneously 
    private const int Max_Count_For_Audio_Objects = 31;

    private ObjectPool<AudioObject> soundEffectsAudioObjects;

    private ObjectPool<AudioObject> SoundEffectsAudioObjects
    {
        get
        {
            if (soundEffectsAudioObjects == null)
            {
                soundEffectsAudioObjects = new ObjectPool<AudioObject>(InstantiateAudioObject, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, defaultCapacity: 4, maxSize: Max_Count_For_Audio_Objects);
            }

            return soundEffectsAudioObjects;
        }
    }

    private AudioObject backgroundMusicAudioObject;

    private AudioObject BackgroundMusicAudioObject
    {
        get
        {
            if (backgroundMusicAudioObject == null)
            {
                backgroundMusicAudioObject = InstantiateAudioObject();
            }

            return backgroundMusicAudioObject;
        }
    }

    private AudioMixer audioMixer;

    private AudioMixer AudioMixer
    {
        get
        {
            if (audioMixer == null)
            {
                audioMixer = Resources.Load("Audio/Application Audio Mixer") as AudioMixer;
            }

            return audioMixer;
        }
    }

    private AudioMixerGroup soundEffectsAudioMixerGroup;

    private AudioMixerGroup SoundEffectsAudioMixerGroup
    {
        get
        {
            if (soundEffectsAudioMixerGroup == null)
            {
                soundEffectsAudioMixerGroup = AudioMixer.FindAudioMixerGroup("SFX");
            }

            return soundEffectsAudioMixerGroup;
        }
    }

    private AudioMixerGroup backgroundMusicAudioMixerGroup;

    private AudioMixerGroup BackgroundMusicAudioMixerGroup
    {
        get
        {
            if (backgroundMusicAudioMixerGroup == null)
            {
                backgroundMusicAudioMixerGroup = AudioMixer.FindAudioMixerGroup("BGM");
            }

            return backgroundMusicAudioMixerGroup;
        }
    }

    private Dictionary<string, AudioClip> cachedAudioClips = new Dictionary<string, AudioClip>();

    #region Pooling

    private AudioObject InstantiateAudioObject()
    {
        AudioObject audioObject = Instantiate(Resources.Load("Audio\\Audio Object") as GameObject, this.transform).GetComponent<AudioObject>();
        audioObject.Release += ReturnPooledItem;
        return audioObject;
    }

    private void OnTakeFromPool(AudioObject audioObject)
    {
        audioObject.gameObject.SetActive(true);
    }

    private void OnReturnedToPool(AudioObject audioObject)
    {
        audioObject.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(AudioObject audioObject)
    {
        Destroy(audioObject.gameObject);
    }

    private void ReturnPooledItem(AudioObject audioObject)
    {
        SoundEffectsAudioObjects.Release(audioObject);
    }

    #endregion

    #region Sound_Effects

    public void PlaySoundEffect(AudioClip audioClip, Vector3 position, float volume = 1.0f)
    {
        SoundEffectsAudioObjects.Get().PlaySoundEffect(audioClip, SoundEffectsAudioMixerGroup, position, volume);
    }

    public void PlaySoundEffect(string audioClipName, Vector3 position, float volume = 1.0f, bool cacheAudioClip = true)
    {
        AudioClip audioClip = null;
        if (cacheAudioClip)
        {
            if (cachedAudioClips.ContainsKey(audioClipName))
            {
                audioClip = cachedAudioClips[audioClipName];
            }
            else
            {
                cachedAudioClips.Add(audioClipName, audioClip.LoadAudioClipFromResources(audioClipName));
            }
        }
        else
        {
            audioClip.LoadAudioClipFromResources(audioClipName);
        }

        PlaySoundEffect(audioClip, position, volume);
    }

    public void PlayUISoundEffects(string audioClip)
    {
        PlaySoundEffect(audioClip, Vector3.zero);
    }

    public void PlayUISoundEffect(AudioClip audioClip)
    {
        PlaySoundEffect(audioClip, Vector3.zero);
    }

    #endregion

    #region Background_Music

    public void PlayBackgroundMusic(AudioClip audioClip, float volume = 1.0f, float spatialBlend = 1.0f, Vector3 position = new Vector3())
    {
        BackgroundMusicAudioObject.PlayBackgroundMusic(audioClip, BackgroundMusicAudioMixerGroup, volume, spatialBlend, position);
    }

    public void PlayBackgroundMusic(string audioClipName, float volume = 1.0f, float spatialBlend = 1.0f, Vector3 position = new Vector3())
    {
        AudioClip audioClip = Resources.Load($"Audio/{audioClipName}") as AudioClip;
        BackgroundMusicAudioObject.PlayBackgroundMusic(audioClip, BackgroundMusicAudioMixerGroup, volume, spatialBlend, position);
    }

    public void StopBGM()
    {
        BackgroundMusicAudioObject.StopBackgroundMusic();
    }

    #endregion

    #region Settings

    private void SetVolume(string exposedParameter, float volume)
    {
        AudioMixer.SetBase10Volume(exposedParameter, volume);
    }

    public void SetMasterVolume(float volume)
    {
        SetVolume(Master_Volume_Exposed_Parameter, volume);
    }

    public void SetSoundEffectsVolume(float volume)
    {
        SetVolume(Sound_Effects_Volume_Exposed_Parameter, volume);
    }

    public void SetBackgroundMusicVolume(float volume)
    {
        SetVolume(Background_Music_Volume_Exposed_Parameter, volume);
    }

    #endregion
}