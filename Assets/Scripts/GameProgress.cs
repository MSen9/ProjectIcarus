using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public static GameProgress current;
    public bool tutorialCompleted = false;
    // Start is called before the first frame update
    void Start()
    {
       
        if(current != null)
        {
            Destroy(this.gameObject);
            return;
        }
        current = this;
        if (SaveAndLoad.current.hasOverallSaveInfo)
        {
            tutorialCompleted = SaveAndLoad.current.sInfo.tutorialCompleted;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



}