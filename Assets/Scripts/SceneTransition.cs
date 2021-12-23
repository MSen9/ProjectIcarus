using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    // Start is called before the first frame update
    public static SceneTransition current;
    void Start()
    {
        if(current != null)
        {
            Destroy(gameObject);
            return;
        }
        current = this;
    }

    public void GoToScene(string sceneName)
    {
        if (MusicPlayer.current != null)
        {
            MusicPlayer.current.StopSong();
        }
        if (sceneName == "Menu")
        {
            Destroy(RunRouteManager.current.gameObject);
            Destroy(BetweenMapInfo.current.gameObject);
            SaveAndLoad.current.loadingRun = false;
            //RunRouteManager.current.firstOpen = true;
        }
       
        SceneManager.LoadScene(sceneName);
    }
}
