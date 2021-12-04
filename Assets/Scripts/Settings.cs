using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
