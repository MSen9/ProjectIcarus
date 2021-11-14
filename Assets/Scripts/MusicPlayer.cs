using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    AudioClip currentSong;
    float currVolume = 0.25f;
    AudioSource aSource;
    public static MusicPlayer current;
    void Start()
    {
        current = this;
        aSource = GetComponent<AudioSource>();
        currentSong = MapManager.current.defaultSong;
        PlaySong(currentSong, currVolume);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSongVals(AudioClip ac, float volume = 1)
    {
        currentSong = ac;
        currVolume = volume;
    }
    public void PlaySong(AudioClip ac, float volume = 1)
    {
        aSource.clip = ac;
        aSource.volume = volume;
        aSource.Play();
    }
}
