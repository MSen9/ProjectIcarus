using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using System.IO;


public class MapManager : MonoBehaviour
{
    float[] spawnSizes;
    public Light2D globalLight;
    float lightGoal = 3f;
    float lightChangeMult;
    // Start is called before the first frame update
    bool loadingMap;
    bool loadingUnits;
    public bool vectorLoad;
    public bool doneLoading;
    int currWave = 0;
    int totalWaves;
    int waveDifficultyScaling;
    float currWaveTime = 0;
    bool wavesDone = false;
    public GameObject spawnLocationParent;
    List<Transform> spawnLocations;
    GameObject[] walls;
    public static MapManager current;
    RunManager rm;
    float wavesDoneGracePeriod;
    List<GameObject> livingEnemies;
    public GameObject spawner;
    bool mapOver;
    public GameObject currencyThing;
    void OnEnable()
    {
        current = this;
        vectorLoad = true;
        doneLoading = false;
        lightGoal = 3f;
        lightChangeMult = 1;
        globalLight = GameObject.FindGameObjectWithTag("LightSource").GetComponent<Light2D>();
        globalLight.intensity = 0;
        currWave = 0;
        spawnLocations = new List<Transform>();
        for (int i = 0; i < spawnLocationParent.transform.childCount; i++)
        {
            spawnLocations.Add(spawnLocationParent.transform.GetChild(i));
        }

        //hardcoded values for now
        currWave = 0;
        waveDifficultyScaling = 5;
        totalWaves = 1;
        rm = GameObject.FindGameObjectWithTag("RunManager").GetComponent<RunManager>();
        livingEnemies = new List<GameObject>();
        wavesDoneGracePeriod = 2f;
        mapOver = false;
    }

    struct WaveInfo
    {
        public float waveTime;
        public List<List<EnemyInfo>> waveGroups;
        public WaveInfo(float waveTime, List<List<EnemyInfo>> waveGroups)
        {
            this.waveTime = waveTime;
            this.waveGroups = new List<List<EnemyInfo>>(waveGroups);
        }
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
        currWaveTime -= Time.deltaTime;
        if(currWaveTime < 0 && wavesDone == false)
        {
            wavesDone = !NewWave();
        }

        if (wavesDone && mapOver == false)
        {
            wavesDoneGracePeriod -= Time.deltaTime;
            if(wavesDoneGracePeriod <= 0)
            {
                if(livingEnemies.Count <= 0)
                {
                    //map is over
                    EndMap();
                }
            }
        }
    }

    void EndMap()
    {
        //disable all bullets
        GameObject bulletList = GameObject.FindGameObjectWithTag("BulletList");
        for (int i = 0; i < bulletList.transform.childCount; i++)
        {
            GameObject currBullet = bulletList.transform.GetChild(i).gameObject;
            if (currBullet.GetComponent<BulletMove>().bulletType == BulletType.damage)
            {
                Instantiate(currencyThing, currBullet.transform.position, currBullet.transform.rotation);
            }
            currBullet.GetComponent<AllPointManager>().BreakBullet();
        }
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerShooting>().EndShooting();
        mapOver = true;
        Debug.Log("Map is completed!");
    }

    bool NewWave()
    {

        if(totalWaves <= currWave)
        {
            return false;
        }
        currWave++;
        WaveInfo newestWave = DynamicWaveGen();
        int randomGroupOffset = Random.Range(0, spawnLocations.Count);
        currWaveTime = newestWave.waveTime;
        for (int i = 0; i < newestWave.waveGroups.Count; i++)
        {
            foreach (EnemyInfo eInfo in newestWave.waveGroups[i])
            {
                SpawnEnemy(eInfo, spawnLocations[(i + randomGroupOffset) % spawnLocations.Count]);
            }
        }
        

        return true;
    }

    void SpawnEnemy(EnemyInfo eInfo, Transform spawnLocation)
    {
        string ePath = Path.Combine("Enemies", eInfo.fileName);
        GameObject spawnEnemy = Resources.Load(ePath) as GameObject;
        if(spawnEnemy == null)
        {
            Debug.LogError("No valid enemy file for " + ePath);
            return;
        }
        Vector3 spawnOffset = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1));
        GameObject madeSpawner = Instantiate(spawner, spawnLocation.transform.position + spawnOffset, Quaternion.identity);
        SpawnManager sm = madeSpawner.GetComponent<SpawnManager>();
        sm.spawnObject = spawnEnemy;
        sm.spawnSize = eInfo.spawnSize;
    }
    //A random generator for waves
    WaveInfo DynamicWaveGen()
    {
        //Generates wabes based on these (input) factors
        //Current run difficulty
        //current map difficulty

        //Generates waves based on the input factors with these output factors
        //Number of enemies made and their respective difficulties
        //Wave time (higher time means more enemies)
        //for now just try to make each wave 10 seconds
        int overallDifficulty = rm.runDifficulty + currWave*waveDifficultyScaling;
        List<EnemyInfo> canSpawn = new List<EnemyInfo>();
        
        foreach(EnemyInfo eInfo in rm.enemies)
        {
            if(eInfo.difficulty < overallDifficulty && eInfo.spawnNormally)
            {
                canSpawn.Add(eInfo);
            }
        }

        if (canSpawn.Count <= 0)
        {
            Debug.LogError("No valid spawns for difficulty of: " + overallDifficulty.ToString());
            return new WaveInfo();
        }

        List<EnemyInfo> allEnemies = new List<EnemyInfo>();
        //generate the enemies
        while (overallDifficulty > 0)
        {

            int randomEnemy = Random.Range(0, canSpawn.Count);
            EnemyInfo selectedEnemy = canSpawn[randomEnemy];
            if(selectedEnemy.difficulty > overallDifficulty)
            {
                canSpawn.Remove(selectedEnemy);
                if(canSpawn.Count <= 0)
                {
                    overallDifficulty = 0;
                }
            } else
            {
                overallDifficulty -= selectedEnemy.difficulty;
                allEnemies.Add(selectedEnemy);
            }
        }

        //now have a list on enemies, decide how many groups to use
        int groups = Random.Range(1, spawnLocations.Count + 1);
        List<List<EnemyInfo>> enemyGroups = new List<List<EnemyInfo>>();
        for (int i = 0; i < groups; i++)
        {
            enemyGroups.Add(new List<EnemyInfo>());
        }
        for (int i = 0; i < allEnemies.Count; i++)
        {
            enemyGroups[i % groups].Add(allEnemies[i]);
        }
        float waveTime = 10f;
        if (totalWaves <= currWave)
        {
            waveTime = 0f;
        }
        return new WaveInfo(waveTime, enemyGroups);
    }
    
    public void EnemySpawn(GameObject enemy)
    {
        livingEnemies.Add(enemy);
    }

    public void EnemyDeath(GameObject enemy)
    {
        livingEnemies.Remove(enemy);
    }
}
