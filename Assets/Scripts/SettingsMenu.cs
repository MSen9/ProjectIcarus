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
        soundVolText = StringToVectorManager.current.StringToVectors("Sound Volume: " + (Settings.current.musicVolume * 50).ToString() + "%");
        soundVolText = StringToVectorManager.current.StringToVectors("Music Volume: " + (Settings.current.musicVolume * 50).ToString() + "%");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateSoundVolText()
    {

    }

    void UpdateMusicVolText()
    {

    }
}
