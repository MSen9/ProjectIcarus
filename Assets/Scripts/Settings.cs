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
            soundEffectVolume = SaveAndLoad.current.sInfo.soundEffectVolume;
            musicVolume = SaveAndLoad.current.sInfo.musicVolume;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
