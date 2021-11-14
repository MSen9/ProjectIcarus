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
    public EnemyInfo(string fileName,EnemyIds id, int difficulty,float spawnSize, bool spawnNormally = true)
    {
        this.fileName = fileName;
        this.id = id;
        this.difficulty = difficulty;
        this.spawnSize = spawnSize;
        this.spawnNormally = spawnNormally;
    }
}

public enum EnemyIds
{
    BasicFireRate,
    BasicShotSize,
    BasicManaGen
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
            return;
        }
        current = this;
        DontDestroyOnLoad(this.gameObject);
        enemies = new List<EnemyInfo>();
        enemies.Add(new EnemyInfo("FireRateEnemy", EnemyIds.BasicFireRate, 2, 1.5f));
        enemies.Add(new EnemyInfo("ManaGenEnemy", EnemyIds.BasicManaGen, 3, 1.8f));
        enemies.Add(new EnemyInfo("ShotSizeEnemy", EnemyIds.BasicShotSize, 6, 3f));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
