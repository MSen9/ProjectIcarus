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
    public GameObject spawner;
    public bool mapOver;
    public GameObject currencyThing;

    public GameObject shop;
    float WALL_DESTROY_TIME = 2;
    float SHOP_SPAWN_TIME = 3;
    float SPAWN_LEVEL_TRANS = 4;
    public bool goingToNextLevel;
    public bool nextLevelFadeOut;
    public AudioClip defaultSong;
    public float fadeOutTime;

    public GameObject levelTransition;
    GameObject player;

    public GameObject wall;
    public GameObject spawnSpot;
    
    //bool debugOn = true;
    void OnEnable()
    {
        current = this;
        vectorLoad = true;
        doneLoading = false;
        goingToNextLevel = false;
        nextLevelFadeOut = false;
        currWave = 0;
        DynamicMapGen(20f, 20);
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
        fadeOutTime = 2f;

        player = GameObject.FindGameObjectWithTag("Player");

        
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
                GoToNextMap();
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
        mapOver = true;
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
        
        //destroy the walls after a delay
        StartCoroutine(DestroyWalls());
        //Spawn shop shortly after walls are destroyed
        StartCoroutine(SpawnShop());
        //Spawns level transition shortly after shop is spawned
        StartCoroutine(SpawnLevelTransition());

        //disable player shooting
        player.GetComponent<PlayerShooting>().EndShooting();
        player.GetComponent<PlayerMovement>().SetCamBounds();
        CamManager.current.EndMapCam(player.transform.position);
        
        Debug.Log("Map is completed!");
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

    public void GoToNextMap()
    {
        BetweenMapInfo.current.SaveMapInfo();
        RunManager.current.runDifficulty += 10;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartFadeOut()
    {
        LightTransition.current.fadingOut = true;
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
        float spawnerFromWallDist = 0.2f;
        foreach (GameObject eachWall in wallsMade)
        {
            Vector3 spawnPointPos = Vector3.Lerp(eachWall.transform.position, Vector3.zero, spawnerFromWallDist);
            GameObject madeSpawn =  Instantiate(spawnSpot, spawnPointPos, Quaternion.identity);
            madeSpawn.transform.parent = spawnLocationParent.transform;

            //eachWall.GetComponent<WallManager>().UpdateWallCollider();
        }
    }
}
