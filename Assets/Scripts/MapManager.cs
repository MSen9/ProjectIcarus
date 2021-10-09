using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public enum EnemyIds
{
    BasicFireRate,
    BasicShotSize,
    BasicManaGen
}
public class MapManager : MonoBehaviour
{
    public GameObject[] enemies; // a list of enemy prefabs that matches to the EnemyIds enum by index
    float[] spawnSizes;
    public Light2D globalLight;
    float lightGoal = 3f;
    float lightChangeMult;
    // Start is called before the first frame update
    public bool normalMapLoad;
    bool loadingMap;
    bool loadingUnits;
    public bool vectorLoad;
    public bool doneLoading;
    int currWave;
    int totalWaves;
    int[,] waveEnemies;
    float[] waveTimes;
    GameObject[] spawnLocations;
    GameObject[] walls;
    public static MapManager current;
    void Awake()
    {
        current = this;
        vectorLoad = true;
        doneLoading = false;
        normalMapLoad = false;
        lightGoal = 3f;
        lightChangeMult = 1;
        globalLight = GameObject.FindGameObjectWithTag("LightSource").GetComponent<Light2D>();
        globalLight.intensity = 0;
        currWave = 0;

    }

    
    // Update is called once per frame
    void Update()
    {


        if(doneLoading == false)
        {
            globalLight.intensity += Time.deltaTime * lightChangeMult;
            if(globalLight.intensity > lightGoal)
            {
                globalLight.intensity = lightGoal;
                doneLoading = true;
            }
            return;
        }
        if(waveTimes[currWave] < 0)
        {

        }
    }
}
