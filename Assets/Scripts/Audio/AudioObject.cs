using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using AarquieSolutions.DependencyInjection.ComponentField;

public class AudioObject : MonoBehaviour
{
    [GetComponent] private AudioSource selfAudioSource;

    private void Awake()
    {
        this.InitializeDependencies();
    }

    public event Action<AudioObject> Release;

    public void PlaySoundEffect(AudioClip audioClip, AudioMixerGroup audioMixerGroup, Vector3 position, float volume)
    {
        selfAudioSource.Stop();
        selfAudioSource.clip = audioClip;
        transform.position = position;
        selfAudioSource.volume = volume;
        selfAudioSource.outputAudioMixerGroup = audioMixerGroup;
        selfAudioSource.Play();
        StartCoroutine(StopSoundEffect(audioClip));
    }

    private IEnumerator StopSoundEffect(AudioClip audioClip)
    {
        yield return new WaitForSeconds(audioClip.length);
        selfAudioSource.Stop();
        Release?.Invoke(this);
    }

    public void PlayBackgroundMusic(AudioClip audioClip, AudioMixerGroup audioMixerGroup, float volume, float spatialBlend, Vector3 position)
    {
        selfAudioSource.Stop();
        selfAudioSource.clip = audioClip;
        selfAudioSource.loop = true;
        transform.position = position;
        selfAudioSource.volume = volume;
        selfAudioSource.outputAudioMixerGroup = audioMixerGroup;
        selfAudioSource.spatialBlend = spatialBlend;
        selfAudioSource.Play();
    }

    public void StopBackgroundMusic()
    {
        selfAudioSource.Stop();
    }
}