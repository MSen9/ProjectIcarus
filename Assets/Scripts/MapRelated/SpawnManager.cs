using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    public float spawnSize;
    float spawnWait = 1f;
    float currGrowth;
    float spawnGrowthSpeed = 8f;
    public GameObject spawnObject;
    bool hasFullyGrown;
    bool isWaiting = false;
    bool isShrinking = false;
    void Start()
    {
        transform.localScale = new Vector3(0, 0, 1);
        hasFullyGrown = false;
        currGrowth = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasFullyGrown == false)
        {
            currGrowth += spawnGrowthSpeed * Time.deltaTime;
            if (currGrowth > spawnSize)
            {
                currGrowth = spawnSize;
                isWaiting = true;
                hasFullyGrown = true;
            }
            transform.localScale = new Vector3(currGrowth, currGrowth, 1);
        }
        else if (isWaiting)
        {
            spawnWait -= Time.deltaTime;
            if (spawnWait <= 0)
            {
                isWaiting = false;
                if(MapManager.current.spawnsAllowed == true)
                {
                    GameObject madeEnemy = Instantiate(spawnObject,transform.position,Quaternion.identity);
                    madeEnemy.transform.parent = GameObject.FindGameObjectWithTag("EnemyList").transform;
                }
                
                isShrinking = true;
            }
        }
        else if (isShrinking)
        {
            currGrowth -= spawnGrowthSpeed;
            if (currGrowth <= 0)
            {
                currGrowth = 0;
                Destroy(gameObject);
            }
            transform.localScale = new Vector3(currGrowth, currGrowth, 1);
        }
    }
    
}
