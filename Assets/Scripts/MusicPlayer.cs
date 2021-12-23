using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    AudioClip currentSong;
    public float currVolume = 0.2f;
    AudioSource aSource;
    public static MusicPlayer current;
     public bool playingSong = false;

    void Start()
    {
        if (current != null)
        {
            Destroy(gameObject);
            return;
        }
        current = this;
        
        aSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);

    }

    // Update is called once per frame

    public void SetSongVals(AudioClip ac, float volume = 1)
    {
        currentSong = ac;
        currVolume = volume;
    }
    public void PlaySong(AudioClip ac, float volume = -1)
    {
        if(volume < 0)
        {
            volume = currVolume;
        }
        playingSong = true;
        aSource.clip = ac;
        aSource.volume = volume * Settings.current.musicVolume;
        aSource.Play();
    }

    public void StopSong()
    {
        if(playingSong == false)
        {
            return;
        }
        playingSong = false;
        aSource.Stop();
    }
}
