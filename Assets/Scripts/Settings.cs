using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Settings : MonoBehaviour
{
    // Start is called before the first frame update
    public static Settings current;
    public float soundEffectVolume = 1f;
    public float musicVolume = 1f;
    public bool fullScreen = false;
    void Start()
    {
        if(current != null)
        {
            Destroy(this.gameObject);
            return;
        }
        current = this;
        DontDestroyOnLoad(this.gameObject);
        if (SaveAndLoad.current.hasOverallSaveInfo)
        {
            OverallSaveInfo sInfo = SaveAndLoad.current.sInfo;
            soundEffectVolume = sInfo.soundEffectVolume;
            musicVolume = sInfo.musicVolume;
            fullScreen = sInfo.fullScreen;
            
        }
        ToggleFullscreen(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleFullscreen(bool switchMode = true)
    {
        if (switchMode)
        {
            fullScreen = !fullScreen;
        }
       
        if (fullScreen)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        } else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }
    }
}
