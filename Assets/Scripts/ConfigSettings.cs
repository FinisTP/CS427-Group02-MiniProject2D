using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ConfigSettings : MonoBehaviour
{
    public AudioMixer mixer;
    public Image BrightnessCover;

    private void Awake()
    {
        BrightnessCover = GameObject.Find("BrightnessCover").GetComponent<Image>();
    }

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

    public void ResetProgress()
    {
        GameManager_.Instance.tracker.ClearProgress();
    }

    public void SelectResolution(int id)
    {
        switch (id)
        {
            case 0:
                Screen.SetResolution(960, 540, Screen.fullScreen); break;
            default:
            case 1:
                Screen.SetResolution(1280, 720, Screen.fullScreen); break;
            case 2:
                Screen.SetResolution(1366, 768, Screen.fullScreen); break;
            case 3:
                Screen.SetResolution(1920, 1080, Screen.fullScreen); break;
        }
    }
}
