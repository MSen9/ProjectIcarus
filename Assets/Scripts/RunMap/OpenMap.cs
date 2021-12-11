using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (RunRouteManager.current.firstOpen)
        {
            RunRouteManager.current.firstOpen = false;
            
        } else
        {
            RunRouteManager.current.BeatLevel();
            RunRouteManager.current.BuildRunMap();
            SaveAndLoad.current.SaveRunInfo();
        }
        
    }
}
