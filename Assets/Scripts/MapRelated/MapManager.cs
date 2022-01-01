using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using System.IO;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    float[] spawnSizes;
    public Light2D globalLight;
    public bool vectorLoad;
    public bool doneLoading;
    int currWave = 0;
    int totalWaves;
    int waveDifficultyScaling;
    float currWaveTime = 0;
    bool wavesDone = false;
    public GameObject spawnLocationParent;
    public GameObject wallParent;
    List<Transform> spawnLocations;
    GameObject[] walls;
    public static MapManager current;
    RunManager rm;
    float wavesDoneGracePeriod;
    List<GameObject> livingEnemies;
    bool waveDeath = false;
    public GameObject spawner;
    public bool mapOver;
    public GameObject currencyThing;
    bool allMustDie = false;
    public bool deathPause = false;
    bool isPaused = false;
    bool backToMenu = false;

    public GameObject shop;
    public GameObject gameEnd;
    float WALL_DESTROY_TIME = 2;
    float SHOP_SPAWN_TIME = 3;
    float SPAWN_LEVEL_TRANS = 4;
    float SPAWN_BOSS_TIME = 3;
    float END_SPAWN_TIME = 3;
    public bool goingToNextLevel;
    public bool nextLevelFadeOut;
    public AudioClip defaultSong;
    public AudioClip bossMusic;
    public AudioClip gameOverMusic;
    public float fadeOutTime;
    

    public GameObject levelTransition;
    GameObject player;
    

    public GameObject wall;
    public GameObject spawnSpot;
    RunNode currMapInfo;
    public bool spawnsAllowed = true;
    float allowedSpawnOffset;
    public GameObject waveTextSpot;
    GameObject waveText;
    List<EnemyInfo> canSpawn;
    List<EnemyInfo> weightedSpawnList;
    //bool debugOn = true;
    GameObject eList;
    GameObject bulletList;
    GameObject spawnerList;

    void OnEnable()
    {
        current = this;
        vectorLoad = true;
        doneLoading = false;
        goingToNextLevel = false;
        nextLevelFadeOut = false;
        currMapInfo = RunRouteManager.current.GetCurrentRunNode();
        spawnLocations = new List<Transform>();
        ExtractMap();
        allowedSpawnOffset = currMapInfo.mapSize * 0.1f;

        //hardcoded values for now
        currWave = 0;
        waveDifficultyScaling = Mathf.RoundToInt(2 * Mathf.Pow(currMapInfo.heightPos,1/3f));
        totalWaves = currMapInfo.waves;

        rm = GameObject.FindGameObjectWithTag("RunManager").GetComponent<RunManager>();
        livingEnemies = new List<GameObject>();
        wavesDoneGracePeriod = 2f;
        mapOver = false;
        fadeOutTime = 2f;

        player = GameObject.FindGameObjectWithTag("Player");
        GetSpawnAbles();

        eList = GameObject.FindGameObjectWithTag("EnemyList");
        bulletList = GameObject.FindGameObjectWithTag("BulletList");
        spawnerList = GameObject.FindGameObjectWithTag("SpawnerList");

        MusicPlayer.current.PlaySong(defaultSong);
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
        if (nextLevelFadeOut)
        {
            if(LightTransition.current.fadedOut)
            {
                BackToRunMap();
            }
        } else if (backToMenu)
        {
            if (LightTransition.current.fadedOut)
            {
                SceneTransition.current.GoToScene("Menu");
            }
        }

        if(doneLoading == false)
        {
            if (LightTransition.current.fullBright)
            {
                doneLoading = true;
            }
            return;
        }
        if(isPaused == false)
        {
            currWaveTime -= Time.deltaTime;
        } else
        {
            return;
        }
        
        if((waveDeath && livingEnemies.Count <= 0) || (currWaveTime < 0 && allMustDie == false) )
        {
            if (wavesDone == false)
            {
                wavesDone = !NewWave();
            }
        }
        

        if (wavesDone && mapOver == false && isPaused == false)
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

        if (Input.GetKeyDown(KeyCode.F2) && mapOver == false && currMapInfo.bossLevel == true)
        {
            DestroyAllEnemies();
            currWave = totalWaves;
            wavesDone = !NewWave();
        }

        if (Input.GetKeyDown(KeyCode.F1) && mapOver == false)
        {
            DestroyAllEnemies();
            wavesDone = true;
            EndMap();
        }
    }

    /*
    public void BackToMenu()
    {
        LightTransition.current.StartFadeOut(1);
        backToMenu = true;
    }
    */
    void DestroyAllEnemies()
    {
        for (int i = 0; i < eList.transform.childCount; i++)
        {
            GameObject currChild = eList.transform.GetChild(i).gameObject;
            HealthHandling hh = currChild.GetComponent<HealthHandling>();
            if(hh != null)
            {
                hh.Death();
            }
        }
    }
    public void MapDeath()
    {
        spawnsAllowed = false;
        mapOver = true;
        deathPause = true;
        MapSetPause(true);
        HealthPowerUpTracker.current.gameOverInfo.SetActive(true);
        MusicPlayer.current.PlaySong(gameOverMusic);
    }

   

    public void MapSetPause(bool doPause)
    {
        isPaused = doPause;
       
        for (int i = 0; i < bulletList.transform.childCount; i++)
        {
            GameObject currBullet = bulletList.transform.GetChild(i).gameObject;
            currBullet.GetComponent<BulletMove>().enabled = !doPause;
            if (currBullet.GetComponent<AutoScaling>() != null)
            {
                currBullet.GetComponent<AutoScaling>().enabled = !doPause;
            }

            if (currBullet.GetComponent<FollowPlayer>() != null)
            {
                currBullet.GetComponent<FollowPlayer>().enabled = !doPause;
            }

        }

        for (int i = 0; i < eList.transform.childCount; i++)
        {
            GameObject currChild = eList.transform.GetChild(i).gameObject;
            currChild.GetComponent<HealthHandling>().enabled = !doPause;
            EnemyShooting[] eShootings = currChild.GetComponents<EnemyShooting>();
            for (int j = 0; j < eShootings.Length; j++)
            {
                eShootings[j].enabled = !doPause;
            }
            currChild.GetComponent<EnemyMovement>().enabled = !doPause;
        }

        for (int i = 0; i < spawnerList.transform.childCount; i++)
        {
            GameObject currChild = spawnerList.transform.GetChild(i).gameObject;
            currChild.GetComponent<SpawnManager>().enabled = !doPause;
        }
        
    }
    
    
    void ClearBullets(bool spawnMoney = true)
    {
        
        for (int i = 0; i < bulletList.transform.childCount; i++)
        {
            GameObject currBullet = bulletList.transform.GetChild(i).gameObject;
            if (currBullet.GetComponent<BulletMove>().bulletType == BulletType.damage)
            {
                //Checks if money can spawn and bullet is still within the map bounds
                if (spawnMoney && currBullet.transform.position.magnitude < currMapInfo.mapSize*2f)
                {
                    Instantiate(currencyThing, currBullet.transform.position, currBullet.transform.rotation);
                }
                
            }
            currBullet.GetComponent<AllPointManager>().BreakBullet();
            
        }
    }
    void EndMap()
    {
        //disable all bullets
        spawnsAllowed = false;
        mapOver = true;
        ClearBullets();
        
        
        //destroy the walls after a delay
        StartCoroutine(DestroyWalls());
        player.GetComponent<PlayerShooting>().EndShooting();
        player.GetComponent<PlayerMovement>().SetCamBounds();
        CamManager.current.EndMapCam(player.transform.position);
        //Spawn shop shortly after walls are destroyed
        if (currMapInfo.bossLevel)
        {
            //TODO: End the game
            GameEnd();
            return;
        }
        StartCoroutine(SpawnShop());
        //Spawns level transition shortly after shop is spawned
        StartCoroutine(SpawnLevelTransition());

        //disable player shooting
        
        
        Debug.Log("Map is completed!");
    }

    void GameEnd()
    {
        Debug.Log("Game is completed!");
        StartCoroutine(SpawnGameEnd());
    }


    IEnumerator DestroyWalls()
    {
        yield return new WaitForSeconds(WALL_DESTROY_TIME);
        GameObject wallList = GameObject.FindGameObjectWithTag("WallContainer");
        for (int i = 0; i < wallList.transform.childCount; i++)
        {
            GameObject currWall = wallList.transform.GetChild(i).gameObject;
            AllPointManager apm = currWall.GetComponent<AllPointManager>();
            if(apm != null)
            {
                apm.DeathAnimation(2,0,2f);
                Destroy(currWall, 2f);
            }
        }
    }
    IEnumerator SpawnLevelTransition()
    {
        //float minDistForNormalSpawn = 5;
        Vector3 spawnLocation = player.transform.position + new Vector3(0,-4);
        yield return new WaitForSeconds(SPAWN_LEVEL_TRANS);
        GameObject madeLevelTransition= Instantiate(levelTransition, spawnLocation,Quaternion.identity);
    }

    IEnumerator SpawnShop()
    {
        float shopYSpawn = player.transform.position.y + 22;
        float shopYEnd = player.transform.position.y + 11.5f;
        float shopXSpawn = player.transform.position.x;
        yield return new WaitForSeconds(SHOP_SPAWN_TIME);
        GameObject madeShop = Instantiate(shop, new Vector3(shopXSpawn, shopYSpawn), Quaternion.identity);
        madeShop.GetComponent<ShopManager>().yEnd = shopYEnd;
        madeShop.GetComponent<ShopManager>().mapDepth = currMapInfo.heightPos;
    }

    IEnumerator SpawnGameEnd()
    {
        float endYSpawn = player.transform.position.y - 30;
        float endYEnd = player.transform.position.y + 11.5f;
        float endXSpawn = player.transform.position.x;
        yield return new WaitForSeconds(END_SPAWN_TIME);
        
        Vector3 startPos = new Vector3(endXSpawn, endYSpawn);
        GameObject madeEnd = Instantiate(gameEnd, startPos, Quaternion.identity);
        madeEnd.GetComponent<GameEnd>().yEnd = endYEnd;
        madeEnd.GetComponent<GameEnd>().posStart = startPos;
    }

    bool NewWave()
    {
        waveDeath = false;
        if (currMapInfo.bossLevel && currWave == totalWaves-1)
        {
            allMustDie = true;
        }
        if (currMapInfo.bossLevel && currWave == totalWaves)
        {
            allMustDie = true;
            ClearBullets(false);
            MusicPlayer.current.PlaySong(bossMusic);
            StartCoroutine(SpawnBoss());
            currWave++;
            return true;
        }
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
                SpawnEnemy(eInfo, spawnLocations[(i + randomGroupOffset) % spawnLocations.Count].position);
            }
        }
        

        return true;
    }

    IEnumerator SpawnBoss()
    {
        
        yield return new WaitForSeconds(SPAWN_BOSS_TIME);
        SpawnEnemy(RunManager.current.GetFinalEnemy(), Vector3.zero);
    }

    void SpawnEnemy(EnemyInfo eInfo, Vector3 spawnLocation)
    {
        string ePath = Path.Combine("Enemies", eInfo.fileName);
        GameObject spawnEnemy = Resources.Load(ePath) as GameObject;
        if(spawnEnemy == null)
        {
            Debug.LogError("No valid enemy file for " + ePath);
            return;
        }
        Vector3 spawnOffset = new Vector3(Random.Range(-1*allowedSpawnOffset, allowedSpawnOffset), Random.Range(-1* allowedSpawnOffset, allowedSpawnOffset));
        GameObject madeSpawner = Instantiate(spawner, spawnLocation + spawnOffset, Quaternion.identity);
        SpawnManager sm = madeSpawner.GetComponent<SpawnManager>();
        sm.spawnObject = spawnEnemy;
        sm.spawnSize = eInfo.spawnSize;
        madeSpawner.transform.parent = spawnerList.transform;
    }
    //A random generator for waves

    void GetSpawnAbles()
    {
        canSpawn = new List<EnemyInfo>();
        foreach (EnemyInfo eInfo in rm.enemies)
        {
            if (eInfo.spawnNormally && eInfo.minMapSpawn <= currMapInfo.heightPos && currMapInfo.heightPos <= eInfo.maxMapSpawn)
            {
                canSpawn.Add(eInfo);
            }
        }

        weightedSpawnList = new List<EnemyInfo>();
        foreach(EnemyInfo eInfo in canSpawn)
        {
            int waveGap = eInfo.maxMapSpawn - eInfo.minMapSpawn;
            float spawnProg = (float)(currMapInfo.heightPos - eInfo.minMapSpawn) / (float)waveGap;
            int trueSpawnRate = Mathf.FloorToInt(Mathf.Lerp(eInfo.startSpawnRate,eInfo.endSpawnRate, spawnProg));
            for (int i = 0; i < trueSpawnRate; i++)
            {
                weightedSpawnList.Add(eInfo);
            }
        }
    }
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
        

        if (canSpawn.Count <= 0)
        {
            Debug.LogError("No valid spawns for difficulty of: " + overallDifficulty.ToString());
            return new WaveInfo();
        }

        List<EnemyInfo> allEnemies = new List<EnemyInfo>();
        //generate the enemies
        while (overallDifficulty > 0)
        {

            int randomEnemy = Random.Range(0, weightedSpawnList.Count);
            EnemyInfo selectedEnemy = weightedSpawnList[randomEnemy];
            overallDifficulty -= selectedEnemy.difficulty;
            allEnemies.Add(selectedEnemy);
        }

        //now have a list on enemies, decide how many groups to use
        int groups = Random.Range(2, spawnLocations.Count + 1);
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
        UpdateWaveText();

        return new WaveInfo(waveTime, enemyGroups);
    }
    
    void UpdateWaveText()
    {

        if(waveText != null)
        {
            StringToVectorManager.current.DestroyString(waveText);
        }
        waveText = StringToVectorManager.current.StringToVectors("Wave:" + currWave.ToString() + "/" + totalWaves.ToString(), 1, StringAlignment.right);
        waveText.transform.position = waveTextSpot.transform.position;
        waveText.transform.parent = waveTextSpot.transform;
    }
    public void EnemySpawn(GameObject enemy)
    {
        livingEnemies.Add(enemy);
    }

    public void EnemyDeath(GameObject enemy)
    {
        livingEnemies.Remove(enemy);
        waveDeath = true;
    }

    public void BackToRunMap()
    {
        BetweenMapInfo.current.SaveMapInfo();
        RunManager.current.runDifficulty += 5;
        SceneTransition.current.GoToScene("RunMap");
    }

    

    void DynamicMapGen(float mapSize, int mapPoints)
    {
        //Generates a set of walls and spawnpoints based on the map size
        //capable of generating 90+ degree angles
        //Generates with symmetry: Vertical, horizontal, or both
        //centers on 0,0 coords
        //easy spawn point rule: Have spawn points show up between player and start point in walls, about 25% inwards from wall point to player
        //maps can potentially be made with 4,5,6+ points, all walls must be 90+ degree angles
        List<GameObject> wallsMade = new List<GameObject>();
        //0 is directly in a cardinal direction
        //1 is directly in a diagonal direction
        //2 is anywhere inbetween

        float equWallAngle = 180 * ((mapPoints - 2) / (float)mapPoints);
        int point1Allignment;
        Vector3 pointPos = Vector3.zero;
        Vector3 lastEndPointPos = Vector3.zero;
        float angleFromPlayer;
        GameObject madeWall;
        for (int i = 0; i < mapPoints; i++)
        {
            if (i == 0)
            {
                //firstPointGen
                point1Allignment = Random.Range(0, 3);
                if(point1Allignment == 0)
                {
                    pointPos = new Vector3(mapSize, 0);
                } else if (point1Allignment == 1)
                {
                    pointPos = new Vector3(mapSize / Mathf.Sqrt(2), mapSize / Mathf.Sqrt(2));
                } else
                {
                    angleFromPlayer = Random.Range(0f, 90f) * Mathf.Deg2Rad;
                    pointPos = new Vector3(Mathf.Cos(angleFromPlayer) * mapSize, Mathf.Sin(angleFromPlayer) * mapSize);
                }

                //See if you should flip the x and y values
                //flip x value
                if (Random.Range(0, 2) == 1)
                {
                    pointPos = new Vector3(pointPos.x *-1, pointPos.y);
                }
                //flip y value
                if (Random.Range(0, 2) == 1)
                {
                    pointPos = new Vector3(pointPos.x, pointPos.y*-1);
                }

                
                //take the angle between the player and the new wall then rotate it by half the wall angle
                
            } else
            {
                pointPos = lastEndPointPos;
                
            }
            madeWall = Instantiate(wall, pointPos, Quaternion.identity);
            float angleBetween = HelperFunctions.AngleBetween(Vector3.zero, pointPos);
            madeWall.transform.rotation = Quaternion.Euler(0, 0, angleBetween - equWallAngle/2 + 90);
            float endAngle = (angleBetween + (90 - equWallAngle)) * Mathf.Deg2Rad;
            lastEndPointPos = new Vector3(Mathf.Cos(endAngle) * mapSize, Mathf.Sin(endAngle) * mapSize);
            float wallLen = Vector3.Magnitude(pointPos - lastEndPointPos);
            madeWall.GetComponent<AllPointManager>().pointObjs[1].transform.localPosition = new Vector3(wallLen,0);
            madeWall.transform.parent = wallParent.transform;
            wallsMade.Add(madeWall);
        }
        float spawnerFromWallDist = 0.35f;
        foreach (GameObject eachWall in wallsMade)
        {
            Vector3 spawnPointPos = Vector3.Lerp(eachWall.transform.position, Vector3.zero, spawnerFromWallDist);
            GameObject madeSpawn =  Instantiate(spawnSpot, spawnPointPos, Quaternion.identity);
            madeSpawn.transform.parent = spawnLocationParent.transform;

            //eachWall.GetComponent<WallManager>().UpdateWallCollider();
        }
    }

    void ExtractMap()
    {
        GameObject runNode = RunRouteManager.current.activeNodeObj;
        runNode.transform.position = Vector3.zero;
        GameObject wallHolder = GameObject.FindGameObjectWithTag("WallContainer");
        wallHolder.transform.parent = this.transform;
        wallHolder.transform.localScale = Vector3.one/RunRouteManager.current.scaleMod;
        Destroy(runNode);
        float spawnerFromWallDist = 0.2f;
        for (int i = 0; i < wallHolder.transform.childCount; i++)
        {
            GameObject eachWall = wallHolder.transform.GetChild(i).gameObject;
            Vector3 spawnPointPos = Vector3.Lerp(eachWall.transform.position, Vector3.zero, spawnerFromWallDist);
            GameObject madeSpawn = Instantiate(spawnSpot, spawnPointPos, Quaternion.identity);
            madeSpawn.transform.parent = spawnLocationParent.transform;
        }

        for (int i = 0; i < spawnLocationParent.transform.childCount; i++)
        {
            spawnLocations.Add(spawnLocationParent.transform.GetChild(i));
        }
    }
}
