using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject soundVolTextSpot;
    public GameObject musicVolTextSpot;
    GameObject soundVolText;
    GameObject musicVolText;
    void Start()
    {
        UpdateSoundText();
        UpdateMusicText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateSoundText()
    {
        if(soundVolText != null)
        {
            StringToVectorManager.current.DestroyString(soundVolText);
        }
        soundVolText = StringToVectorManager.current.StringToVectors("Sound Volume: " + Mathf.RoundToInt(Settings.current.soundEffectVolume * 50).ToString() + "%");
        soundVolText.transform.position = soundVolTextSpot.transform.position;
        soundVolText.transform.parent = soundVolText.transform;
    }

    public void UpdateMusicText()
    {
        if (musicVolText != null)
        {
            StringToVectorManager.current.DestroyString(musicVolText);
        }
        musicVolText = StringToVectorManager.current.StringToVectors("Music Volume: " + Mathf.RoundToInt(Settings.current.musicVolume * 50).ToString() + "%");
        musicVolText.transform.position = musicVolTextSpot.transform.position;
        musicVolText.transform.parent = musicVolText.transform;
    }
}
