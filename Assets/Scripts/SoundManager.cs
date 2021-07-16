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
    public AudioSource BGM;
    public AudioSource Ambience1;
    public AudioSource Ambience2;
    public AudioSource Footstep;
    public AudioSource Trance;

    public AudioClip TitleClip;
    public AudioClip TitleAmbience;

    public void PlayTitleClip()
    {
        if (BGM.clip != TitleClip)
        {
            StopAllTrack();
            PlayBGM(TitleClip, TitleAmbience);
        }
        
    }


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

    public void PlayBGM(AudioClip clip = null, AudioClip amb1 = null, AudioClip amb2 = null)
    {
        StopAllTrack();
        if (clip != null)
        {
            BGM.clip = clip;
            BGM.loop = true;
            BGM.Play();
        }
        if (amb1 != null)
        {
            Ambience1.clip = amb1;
            Ambience1.loop = true;
            Ambience1.Play();
        }
        if (amb2 != null)
        {
            Ambience2.clip = amb1;
            Ambience2.loop = true;
            Ambience2.Play();
        }
    }

    public void StopAllTrack()
    {
        if (BGM.isPlaying) BGM.Stop();
        if (Ambience1.isPlaying) Ambience1.Stop();
        if (Ambience2.isPlaying) Ambience2.Stop();
        if (Footstep.isPlaying) Footstep.Stop();
        if (Trance.isPlaying) Trance.Stop();
    }


    public void SetVolume(float value)
    {
        GetComponent<AudioSource>().volume = value;
    }

    public AudioClip GetTrackFromName(string name)
    {
        foreach (SoundClip clip in SoundClips)
        {
            if (clip.name == name) return clip.track;
        }
        return null;
    }

    public void PlayFootStep(bool state)
    {
        if (state == true)
        {
            if (!Footstep.isPlaying)
            {
                Footstep.Play();
            }
        } else
        {
            Footstep.Stop();
        }
    }

    public void PauseAllTracks()
    {
        if (BGM.isPlaying) BGM.Pause();
        if (Ambience1.isPlaying) Ambience1.Pause();
        if (Ambience2.isPlaying) Ambience2.Pause();
    }

    public void ContinueAllTracks()
    {
        if (!BGM.isPlaying) BGM.Play();
        if (!Ambience1.isPlaying) Ambience1.Play();
        if (!Ambience2.isPlaying) Ambience2.Play();
    }

    public void PlayTrance(bool state)
    {
        if (state == true) {
            if (!Trance.isPlaying) Trance.Play();
            PauseAllTracks();
        }  
        else
        {
            Trance.Pause();
            ContinueAllTracks();
        }
    }



}
