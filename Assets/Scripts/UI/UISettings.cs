using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;

    private void Start()
    {
        masterVolumeSlider.value = SettingsManager.masterVolume.Value;
    }



    public void SetMasterVolume(float sliderValue)
    {
        SettingsManager.masterVolume.Value = sliderValue;
    }

    public void SetSFXVolume(float sliderValue)
    {
        SettingsManager.sfxVolume.Value = sliderValue;
    }

    public void SetBGMVolume(float sliderValue)
    {
        SettingsManager.bgmVolume.Value = sliderValue;
    }
}
