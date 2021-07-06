using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundClip
{
    public string name;
    public bool isSFX;
    public AudioClip track;
}
public class SoundManager : MonoBehaviour
{
    // Start is called before the first frame update
    public SoundClip[] SoundClips;
    public void PlayClip(string name, float volume = -1)
    {
        foreach (SoundClip clip in SoundClips)
        {
            if (clip.name == name)
            {
                if (volume == -1)
                {
                    GetComponent<AudioSource>().PlayOneShot(clip.track);
                }
                else
                    GetComponent<AudioSource>().PlayOneShot(clip.track, volume);
                break;
            }
        }
    }
    public void PlayLoop(string name)
    {
        GetComponent<AudioSource>().Stop();
        foreach (SoundClip clip in SoundClips)
        {
            if (clip.name == name)
            {
                GetComponent<AudioSource>().clip = clip.track;
                GetComponent<AudioSource>().loop = true;
                GetComponent<AudioSource>().Play();
                break;
            }

        }
    }

    public void SetVolume(float value)
    {
        GetComponent<AudioSource>().volume = value;
    }
}
