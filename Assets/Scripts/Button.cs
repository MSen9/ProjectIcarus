using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public enum ButtonFunctionality
{
    newGame,
    loadGame,
    saveGame,
    settings,
    credits,
    backToMenu,
    exitToDesktop,
    raiseSoundVolume,
    lowerSoundVolume,
    raiseMusicVolume,
    lowerMusicVolume,
    battleMapToMenu,
    howToPlay,
    unPause,
    fullScreen
    
}
public class Button : MonoBehaviour
{
    public string buttonText;
    public float textScale = 1;
    public GameObject buttonTextObj;
    public ButtonFunctionality buttonMode;
    bool isClicked = false;
    public bool selectableButton = true;
    public Vector3 buttonDimensions = new Vector3(5,1.5f, 0);
    public Vector3 textOffset = new Vector3(0,-.5f,0);
    Dictionary<ButtonFunctionality, Action> buttonToAction;
    Dictionary<ButtonFunctionality, Action> buttonToOpenCheck;
    float clickedScale = 1.1f;

    public GameObject[] buttonCorners;
    public GameObject checkMark;
    BoxCollider2D bCol;
    // Start is called before the first frame update
    void Start()
    {
        if(buttonTextObj == null)
        {
            MakeTextObj();
        }
        


        ResizeButton();
        DefineButtonDefinitions();
        if (buttonToOpenCheck.ContainsKey(buttonMode))
        {
            buttonToOpenCheck[buttonMode]();
        }
        
    }


    public void ResizeButton()
    {
        buttonCorners[0].transform.localPosition = new Vector3(buttonDimensions.x * -1, buttonDimensions.y, 0);
        buttonCorners[1].transform.localPosition = buttonDimensions;
        buttonCorners[2].transform.localPosition = new Vector3(buttonDimensions.x * -1, buttonDimensions.y * -1, 0);
        buttonCorners[3].transform.localPosition = new Vector3(buttonDimensions.x, buttonDimensions.y * -1, 0);
        GetComponent<AllPointManager>().InstantGrowAllVertexes();
        bCol = GetComponent<BoxCollider2D>();
        bCol.size = buttonDimensions * 2;
    }

    public void MakeTextObj()
    {
        buttonTextObj = StringToVectorManager.current.StringToVectors(buttonText, textScale, StringAlignment.center);
        buttonTextObj.transform.parent = this.transform;
        buttonTextObj.transform.position = this.transform.position + textOffset;
    }

    public void EditorMakeTextObj()
    {
        if(buttonTextObj != null)
        {
            DestroyImmediate(buttonTextObj);
        }
        StringToVectorManager sToV = GameObject.FindGameObjectWithTag("StoVManager").GetComponent<StringToVectorManager>();
        buttonTextObj = sToV.MakeEditorString(buttonText,textScale, StringAlignment.center);
        buttonTextObj.transform.parent = this.transform;
        buttonTextObj.transform.position = this.transform.position + textOffset;
    }

    void DefineButtonDefinitions()
    {
        buttonToOpenCheck = new Dictionary<ButtonFunctionality, Action>();

        buttonToAction = new Dictionary<ButtonFunctionality, Action>();
        buttonToAction.Add(ButtonFunctionality.newGame, StartNewGame);
        buttonToAction.Add(ButtonFunctionality.loadGame, LoadGame);
        buttonToAction.Add(ButtonFunctionality.settings, GoToSettings);
        buttonToAction.Add(ButtonFunctionality.exitToDesktop,ExitToDesktop);
        buttonToAction.Add(ButtonFunctionality.credits, GoToCredits);
        buttonToAction.Add(ButtonFunctionality.backToMenu, BackToMenu);
        buttonToAction.Add(ButtonFunctionality.raiseSoundVolume, RaiseSoundEffectVolume);
        buttonToAction.Add(ButtonFunctionality.lowerSoundVolume, LowerSoundEffectVolume);
        buttonToAction.Add(ButtonFunctionality.raiseMusicVolume, RaiseMusicVolume);
        buttonToAction.Add(ButtonFunctionality.lowerMusicVolume, LowerMusicVolume);
        buttonToAction.Add(ButtonFunctionality.battleMapToMenu, OtherSceneToMenu);
        buttonToAction.Add(ButtonFunctionality.howToPlay, GoToHowToPlay);
        buttonToAction.Add(ButtonFunctionality.unPause, UnPauseGame);
        buttonToAction.Add(ButtonFunctionality.fullScreen, ToggleFullscreen);
    }
    // Update is called once per frame
    void Update()
    {
        if (isClicked)
        {
            transform.localScale = Vector3.one * clickedScale;
        } else
        {
            transform.localScale = Vector3.one;
        }
    }

    void OnMouseDown()
    {
        if (selectableButton)
        {
            isClicked = true;
            transform.localScale = Vector3.one * clickedScale;
        }
       
    }

    void OnMouseUp()
    {
        if (isClicked)
        {
            if (buttonToAction.ContainsKey(buttonMode))
            {
                buttonToAction[buttonMode]();
            } else
            {
                Debug.LogError("Error: No button for button type: " + buttonMode.ToString());
            }
            isClicked = false;
            transform.localScale = Vector3.one;
        }
    }

    private void OnMouseExit()
    {
        isClicked = false;
        transform.localScale = Vector3.one;
    }


    public void ExplodeButton()
    {
        GetComponent<AllPointManager>().DeathAnimation(6f, 30, 1.5f);
        StringToVectorManager.current.StringExplode(buttonTextObj, 1.5f, 4f);
    }

    void ToggleCheckMark()
    {
        checkMark.SetActive(!checkMark.activeSelf);
    }
    //Button effect list
    void StartNewGame()
    {
        MenuManager.current.TransitionToNewGame();
    }

    void OtherSceneToMenu()
    {
        RunManager.current.BackToMenu();
        LightTransition.current.ignorePause = true;
    }

    void LoadGame()
    {

        MenuManager.current.TransitionToLoadGame();
    }

    void GoToSettings()
    {
        CamManager.current.SetPoint(new Vector3(0, -30, -10));
    }

    void GoToHowToPlay()
    {
        CamManager.current.SetPoint(new Vector3(0, -90, -10));
    }

    void GoToCredits()
    {
        CamManager.current.SetPoint(new Vector3(0, -60, -10));
    }

    void ExitToDesktop()
    {
        Debug.Log("Quit the normal way.");
        Application.Quit();
    }
    void BackToMenu()
    {
        CamManager.current.SetPoint(new Vector3(0, 0, -10));
    }

    void RaiseSoundEffectVolume()
    {
        float soundIncrement = 0.2f;
        Settings.current.soundEffectVolume += soundIncrement;
        Settings.current.soundEffectVolume = Mathf.Clamp(Settings.current.soundEffectVolume, 0, 2);
        //Play a sound to show audio difference
        transform.parent.GetComponent<SoundPlayer>().PlaySound(null, 0.5f);
        transform.parent.GetComponent<SettingsMenu>().UpdateSoundText();
    }
    void LowerSoundEffectVolume()
    {
        float soundIncrement = 0.2f;
        Settings.current.soundEffectVolume -= soundIncrement;
        Settings.current.soundEffectVolume = Mathf.Clamp(Settings.current.soundEffectVolume, 0, 2);
        //Play a sound to show audio difference
        transform.parent.GetComponent<SoundPlayer>().PlaySound(null, 0.5f);
        transform.parent.GetComponent<SettingsMenu>().UpdateSoundText();
    }

    void RaiseMusicVolume()
    {
        float soundIncrement = 0.2f;
        Settings.current.musicVolume += soundIncrement;
        Settings.current.musicVolume = Mathf.Clamp(Settings.current.musicVolume, 0, 2);
        transform.parent.GetComponent<SettingsMenu>().UpdateMusicText();
        MusicPlayer.current.UpdateVolume();
    }
    void LowerMusicVolume()
    {
        float soundIncrement = 0.2f;
        Settings.current.musicVolume -= soundIncrement;
        Settings.current.musicVolume =  Mathf.Clamp(Settings.current.musicVolume, 0, 2);
        transform.parent.GetComponent<SettingsMenu>().UpdateMusicText();
        MusicPlayer.current.UpdateVolume();
    }

    void ToggleFullscreen()
    {
        ToggleCheckMark();
        Settings.current.ToggleFullscreen();
    }
    void UnPauseGame()
    {
        Pauser.current.UnPauseGame();
    }
}
