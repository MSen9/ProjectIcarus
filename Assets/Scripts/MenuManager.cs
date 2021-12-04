using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    bool menuTransition = false;
    bool startNewMap = false;
    bool settingsOpen = false;
    bool creditsOpen = false;
    public static MenuManager current;
    public GameObject title;
    public GameObject subTitle;
    public GameObject[] menuButtons;
    
    // Start is called before the first frame update
    void Start()
    {
        current = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (LightTransition.current.fadedOut)
        {
            SceneManager.LoadScene("RunMap");
        }
    }

    public void MassMenuExplode()
    {
        for (int i = 0; i < menuButtons.Length; i++)
        {
            //menuButtons[i].GetComponent<Button>().ExplodeButton();
        }
        title.GetComponent<TitleMesser>().TitleExplode();
        subTitle.GetComponent<TitleMesser>().TitleExplode();
    }
}
