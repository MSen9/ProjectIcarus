using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    AudioSource aSource;
    public bool onlyPlaySound = false;
    public AudioClip sound;
    public float volume;
    void Start()
    {
        if (onlyPlaySound)
        {
            PlaySoundThenDestroy(sound,volume);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static void MakeOnlySound(AudioClip sound, float volume = 1)
    {
        GameObject soundMaker = Resources.Load("SoundMaker") as GameObject;
        GameObject madeSoundmaker = Instantiate(soundMaker);
        SoundPlayer sp = madeSoundmaker.GetComponent<SoundPlayer>();
        sp.sound = sound;
        sp.volume = volume * Settings.current.soundEffectVolume;
    }

    public void PlaySoundThenDestroy(AudioClip sound, float volume = 1)
    {
        PlaySound(sound, volume);
        Destroy(gameObject, sound.length + 0.1f);
    }

    //responsible for playing sound effects in game, subject to sound effect options and modifiers
    public void PlaySound(AudioClip sound, float volume = 1)
    {
        if(sound == null)
        {
            sound = this.sound;
        }
        aSource = GetComponent<AudioSource>();
        if (aSource == null)
        {
            Debug.LogError("No audiosource for object: " + name);
            return;
        }
        //add sound effect option modifier
        aSource.volume = volume * Settings.current.soundEffectVolume;
        aSource.clip = sound;
        aSource.Play();
    }
}
