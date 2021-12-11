using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class SaveAndLoad : MonoBehaviour
{
    // Start is called before the first frame update
    public static SaveAndLoad current;
    public OverallSaveInfo sInfo;
    public RunSaveInfo runSInfo;
    public bool hasOverallSaveInfo;
    public bool hasRunSaveInfo;
    public bool loadingRun = false;
    void OnEnable()
    {
        
        if(current == null)
        {
            current = this;
        }

        hasOverallSaveInfo = LoadOverallInfo();
        hasRunSaveInfo = LoadRunInfo();
    }

    public void SaveOverallInfo()
    {
        sInfo = new OverallSaveInfo();
        Settings s = Settings.current;
        GameProgress gp = GameProgress.current;
        sInfo.tutorialCompleted = gp.tutorialCompleted;
        sInfo.soundEffectVolume = s.soundEffectVolume;
        sInfo.musicVolume = s.musicVolume;
        string saveText = JsonUtility.ToJson(sInfo);
        File.WriteAllText(Application.persistentDataPath + "/OverallSave.json", saveText);
    }
    public bool LoadOverallInfo()
    {
        if (File.Exists(Application.persistentDataPath + "/OverallSave.json"))
        {
            string loadString = File.ReadAllText(Application.persistentDataPath + "/OverallSave.json");
            sInfo = JsonUtility.FromJson<OverallSaveInfo>(loadString);
            return true;
        }
        return false;
    }
    void OnApplicationQuit()
    {
        SaveOverallInfo();
    }

    public void SaveRunInfo()
    {
        //saves all the info for the current run
        //Saves player info between maps and 
        runSInfo = new RunSaveInfo();
        runSInfo.psInfo = BetweenMapInfo.current.savedInfo;
        runSInfo.runNodes = new List<RunNode>(RunRouteManager.current.runNodes);
        runSInfo.activeNode = RunRouteManager.current.activeNode;
        runSInfo.beatenNodes = new List<int>(RunRouteManager.current.beatenNodes);
        string saveText = JsonUtility.ToJson(runSInfo);
        File.WriteAllText(Application.persistentDataPath + "/RunSave.json", saveText);
    }

    public bool LoadRunInfo()
    {
        if (File.Exists(Application.persistentDataPath + "/RunSave.json"))
        {
            string loadString = File.ReadAllText(Application.persistentDataPath + "/RunSave.json");
            runSInfo = JsonUtility.FromJson<RunSaveInfo>(loadString);
            return true;
        }
        return false;
    }

}
[System.Serializable]
public class OverallSaveInfo
{
    public bool tutorialCompleted;
    public float soundEffectVolume;
    public float musicVolume;
}

[System.Serializable]
public class RunSaveInfo
{
    public PlayerSaveInfo psInfo;
    public List<RunNode> runNodes;
    public int activeNode;
    public List<int> beatenNodes;
}
