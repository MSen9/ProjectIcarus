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
    exitToDesktop,
    raiseSoundVolume,
    lowerSoundVolume,
    raiseMusicVolume,
    lowerMusicVolume
}
public class Button : MonoBehaviour
{
    public string buttonText;
    public float textScale = 1;
    GameObject buttonTextObj;
    public ButtonFunctionality buttonMode;
    bool isClicked = false;
    public bool selectableButton = true;
    public Vector3 buttonDimensions = new Vector3(5,1.5f, 0);
    public Vector3 textOffset = new Vector3(0,-1f,0);
    Dictionary<ButtonFunctionality, Action> buttonToAction;
    float clickedScale = 1.1f;

    public GameObject[] buttonCorners;
    BoxCollider2D bCol;
    // Start is called before the first frame update
    void Start()
    {
        buttonTextObj = StringToVectorManager.current.StringToVectors(buttonText, textScale, StringAlignment.center);
        buttonTextObj.transform.parent = this.transform;
        buttonTextObj.transform.position = this.transform.position + textOffset;

        buttonCorners[0].transform.localPosition = new Vector3(buttonDimensions.x*-1,buttonDimensions.y,0);
        buttonCorners[1].transform.localPosition = buttonDimensions;
        buttonCorners[2].transform.localPosition = new Vector3(buttonDimensions.x*-1, buttonDimensions.y*-1, 0);
        buttonCorners[3].transform.localPosition = new Vector3(buttonDimensions.x, buttonDimensions.y * -1, 0);
        GetComponent<AllPointManager>().InstantGrowAllVertexes();
        bCol = GetComponent<BoxCollider2D>();
        bCol.size = buttonDimensions * 2;

        buttonToAction = new Dictionary<ButtonFunctionality, Action>();
        buttonToAction.Add(ButtonFunctionality.newGame, StartNewGame);
        buttonToAction.Add(ButtonFunctionality.settings, GoToSettings);
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

    //Button effect list
    void StartNewGame()
    {
        LightTransition.current.StartFadeOut(2f);
        MenuManager.current.MassMenuExplode();
    }

    void GoToSettings()
    {
        Camera.main.transform.position = new Vector3(0, -30, -10);
    }

    void BackToMenu()
    {
        Camera.main.transform.position = new Vector3(0, 0, -10);
    }


}
