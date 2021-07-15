using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ConfigSettings : MonoBehaviour
{
    public AudioMixer mixer;
    public Image BrightnessCover;

    public void SetLevel (float sliderValue)
    {
        mixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
    }

    public void SetBrightness (float sliderValue)
    {
        if (sliderValue <= 0.5f)
        BrightnessCover.color = new Color(0, 0, 0, 0.5f - sliderValue);
        else
        {
            BrightnessCover.color = new Color(1, 1, 1, sliderValue - 0.5f);
        }
    }
}
