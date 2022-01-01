using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pauser : MonoBehaviour
{
    public bool isPaused = false;
    public static Pauser current;
    public GameObject pauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        current = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                UnPauseGame();
            } else
            {
                PauseGame();
            }
        }
    }
    public void PauseGame()
    {
        if(LightTransition.current.fadingOut || LightTransition.current.fadingIn)
        {
            return;
        }
        isPaused = true;
        if (MapManager.current != null)
        {
            MapManager.current.MapSetPause(true);
        }
        pauseMenu.SetActive(true);
    }
    
    public void UnPauseGame()
    {
        isPaused = false;
        if (MapManager.current != null)
        {
            MapManager.current.MapSetPause(false);
        }
        pauseMenu.SetActive(false);
    }
}
