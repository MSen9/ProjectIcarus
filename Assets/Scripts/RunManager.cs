using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct EnemyInfo
{
    public string fileName;
    public EnemyIds id;
    public int difficulty;
    public float spawnSize;
    public bool spawnNormally;
    public int minMapSpawn;
    public int maxMapSpawn;
    public EnemyInfo(string fileName,EnemyIds id, int difficulty,float spawnSize, bool spawnNormally = true, int minMapSpawn = 0, int maxMapSpawn = 99)
    {
        this.fileName = fileName;
        this.id = id;
        this.difficulty = difficulty;
        this.spawnSize = spawnSize;
        this.spawnNormally = spawnNormally;
        this.minMapSpawn = minMapSpawn;
        this.maxMapSpawn = maxMapSpawn;
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
}
public class RunManager : MonoBehaviour
{
    
    public int runDifficulty = 10;
    //int level = 0;
    public List<EnemyInfo> enemies;

    public static RunManager current;
    // Start is called before the first frame update
    void OnEnable()
    {
        if (current != null)
        {
            Destroy(this.gameObject);
            return;
        }
        current = this;
        DontDestroyOnLoad(this.gameObject);
        enemies = new List<EnemyInfo>();
        enemies.Add(new EnemyInfo("FireRateEnemy", EnemyIds.BasicFireRate, 2, 1.5f,true,0, 3));
        
        enemies.Add(new EnemyInfo("ManaGenEnemy", EnemyIds.BasicManaGen, 3, 1.8f,true,0, 3));
        enemies.Add(new EnemyInfo("ShotSizeEnemy", EnemyIds.BasicShotSize, 6, 3f,true, 0, 3));
        enemies.Add(new EnemyInfo("ShotPenEnemy", EnemyIds.BasicShotSize, 4, 2f,true, 0,3));
        enemies.Add(new EnemyInfo("AdvFireRateEnemy", EnemyIds.AdvFireRate, 7, 2.5f, true, 2, 10));
        enemies.Add(new EnemyInfo("AdvManaGenEnemy", EnemyIds.AdvManaGen, 8, 3f, true, 2, 10));
        enemies.Add(new EnemyInfo("AdvShotPenEnemy", EnemyIds.AdvShotPen, 6, 3f, true, 2, 10));
        enemies.Add(new EnemyInfo("AdvShotSizeEnemy", EnemyIds.AdvShotSize, 12, 6f, true, 2, 10));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
