using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public static class AudioExtensions
{
    public static AudioMixerGroup FindAudioMixerGroup(this AudioMixer audioMixer, string groupName)
    {
        return audioMixer.FindMatchingGroups(groupName)[0];
    }

    public static void SetBase10Volume(this AudioMixer audioMixer, string exposedParameter, float volume)
    {
        float decibelVolume;

        if (volume.AlmostEquals(0.0f, float.Epsilon))
        {
            decibelVolume = -80f;
        }
        else
        {
            decibelVolume = 20f * Mathf.Log10(volume);
        }

        audioMixer.SetFloat(exposedParameter, decibelVolume);
    }

    public static AudioClip LoadAudioClipFromResources(this AudioClip audioClip, string audioClipNameWithPath)
    {
        audioClip = Resources.Load(audioClipNameWithPath) as AudioClip;
        return audioClip;
    }
}
