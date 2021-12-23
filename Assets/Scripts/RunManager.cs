using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct EnemyInfo
{
    public string fileName;
    public EnemyIds id;
    public int difficulty;
    public float spawnSize;
    public bool spawnNormally;
    public int minMapSpawn;
    public int maxMapSpawn;
    public int startSpawnRate;
    public int endSpawnRate;
    public EnemyInfo(string fileName,EnemyIds id, int difficulty,float spawnSize, bool spawnNormally = true, int minMapSpawn = 0, int maxMapSpawn = 99, 
        int startSpawnRate = 0, int endSpawnRate =0)
    {
        this.fileName = fileName;
        this.id = id;
        this.difficulty = difficulty;
        this.spawnSize = spawnSize;
        this.spawnNormally = spawnNormally;
        this.minMapSpawn = minMapSpawn;
        this.maxMapSpawn = maxMapSpawn;
        this.startSpawnRate = startSpawnRate;
        this.endSpawnRate = endSpawnRate;
    }
}

public enum EnemyIds
{
    BasicFireRate,
    BasicShotSize,
    BasicManaGen,
    BasicShotPen,
    AdvFireRate,
    AdvManaGen,
    AdvShotPen,
    AdvShotSize,
    EliteFireRate,
    EliteManaGen,
    EliteShotPen,
    EliteShotSize,
    TripleShot,
    SplitShot,
    ExplodeShot,
    OMEGA
}
public class RunManager : MonoBehaviour
{
    
    public int runDifficulty = 8;
    //int level = 0;
    public List<EnemyInfo> enemies;

    public static RunManager current;
    EnemyInfo finalEnemy;
    bool backToMenu = false;
    // Start is called before the first frame update
    void OnEnable()
    {
        Application.targetFrameRate = 60;
        if (current != null)
        {
            Destroy(this.gameObject);
            return;
        }
        current = this;
        DontDestroyOnLoad(this.gameObject);
        enemies = new List<EnemyInfo>();
        enemies.Add(new EnemyInfo("FireRateEnemy", EnemyIds.BasicFireRate, 2, 1.5f,true,1, 8,300,50));      
        enemies.Add(new EnemyInfo("ManaGenEnemy", EnemyIds.BasicManaGen, 3, 1.8f,true,1, 8,300,50));
        enemies.Add(new EnemyInfo("ShotSizeEnemy", EnemyIds.BasicShotSize, 6, 3f,true, 1, 8,200,25));
        enemies.Add(new EnemyInfo("ShotPenEnemy", EnemyIds.BasicShotPen, 4, 2f,true, 1, 8,250,40));
        enemies.Add(new EnemyInfo("AdvFireRateEnemy", EnemyIds.AdvFireRate, 7, 2.5f, true, 2, 9,10,75));
        enemies.Add(new EnemyInfo("AdvManaGenEnemy", EnemyIds.AdvManaGen, 8, 3f, true, 3, 9,10,75));
        enemies.Add(new EnemyInfo("AdvShotPenEnemy", EnemyIds.AdvShotPen, 10, 3f, true, 4, 9,10,100));
        enemies.Add(new EnemyInfo("AdvShotSizeEnemy", EnemyIds.AdvShotSize, 12, 6f, true, 4, 9,10,100));
        enemies.Add(new EnemyInfo("EliteFireRateEnemy", EnemyIds.EliteFireRate, 20, 3.5f, true, 5, 9,5,100));
        enemies.Add(new EnemyInfo("EliteManaGenEnemy", EnemyIds.EliteManaGen, 25, 4f, true, 6, 9, 10, 100));
        enemies.Add(new EnemyInfo("EliteShotPenEnemy", EnemyIds.EliteShotPen, 25, 5f, true, 6, 9, 10, 75));
        enemies.Add(new EnemyInfo("EliteShotSizeEnemy", EnemyIds.EliteShotSize, 30, 6.5f, true, 7, 9, 10, 50));
        enemies.Add(new EnemyInfo("TripleShotEnemy", EnemyIds.TripleShot, 20, 4f, true, 2, 9, 1, 20));
        enemies.Add(new EnemyInfo("SplitShotEnemy", EnemyIds.SplitShot, 25, 4f, true, 2, 9, 1, 20));
        enemies.Add(new EnemyInfo("ExplodeShotEnemy", EnemyIds.ExplodeShot, 30, 5f, true,3, 9, 1, 15));
        finalEnemy = new EnemyInfo("FinalEnemy", EnemyIds.OMEGA, 1000, 12f, false, 0, 0, 0,0);
        enemies.Add(finalEnemy);

    }

    public EnemyInfo GetFinalEnemy()
    {
        return finalEnemy;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(LightTransition.current != null && LightTransition.current.fadingOut == false)
            {
                backToMenu = true;
                LightTransition.current.StartFadeOut(1);
            }
            
           
        }
        if (backToMenu && LightTransition.current != null && LightTransition.current.fadedOut)
        {
            SceneTransition.current.GoToScene("Menu");
        }

        
    }
}
