using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public struct ShopItemInfo
{
    public string fileName;
    public int cost;
    public int costVariance;
    public bool spawnNormally;
    public int startSpawnDepth;

    public ShopItemInfo(string fileName, int cost, int costVariance = 0, bool spawnNormally = true, int startSpawnDepth = 0)
    {
        this.fileName = fileName;
        this.cost = cost;
        this.costVariance = costVariance;
        this.spawnNormally = spawnNormally;
        this.startSpawnDepth = startSpawnDepth;
    }
    
}
public enum BuyEffects
{
    none,
    gainHp,  
    gainPowerUp,
    gainRandomBasicPowerUp,
    losePowerUp,
    loseRandomBasicPowerUp,
    gainClear,
}
public class ShopManager : MonoBehaviour
{
    GameObject player;
    HealthPowerUpTracker healthPupTracker;
    public static ShopManager current;
    GameObject currDesc;
    public List<ShopItemInfo> possibleItems;
    Vector3 startPos;
    public float yEnd = 0;
    float moveTime = 0;
    float maxMoveTime = 2;
    bool yEndReached = false;
    public int mapDepth;
    // Start is called before the first frame update
    float shopItems = 9;
    void Start()
    {
        int level = RunRouteManager.current.GetCurrentRunNode().heightPos;
        current = this;
        player = GameObject.FindGameObjectWithTag("Player");
        healthPupTracker = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HealthPowerUpTracker>();
        possibleItems = new List<ShopItemInfo>();
        possibleItems.Add(new ShopItemInfo("HeartShopItem", 10*(level+1),0,false));
        possibleItems.Add(new ShopItemInfo("LoseRandomBasicPUP", 5 * (level + 1), 0, false));
        possibleItems.Add(new ShopItemInfo("AntiVectorShopItem", 5 * (level + 1), 0, false));
        possibleItems.Add(new ShopItemInfo("BasicFireRatePUP", -10, 5));
        possibleItems.Add(new ShopItemInfo("BasicPowerGenPUP", -10, 5));
        possibleItems.Add(new ShopItemInfo("BasicShotSizePUP", -10, 5));
        possibleItems.Add(new ShopItemInfo("BasicShotPenPUP", -10, 5));
        possibleItems.Add(new ShopItemInfo("RandomBasicPUP", -15, 5));
        possibleItems.Add(new ShopItemInfo("ShotSpreadPUP", -100, 50,true,5));
        possibleItems.Add(new ShopItemInfo("ShotSplitPUP", -100, 50,true,6));
        possibleItems.Add(new ShopItemInfo("ShotExplodePUP", -100, 50, true, 7));

        //make the shop items
        MakeShopItems();
        startPos = transform.position;
    }

    public void MakeShopItems()
    {
        List<ShopItemInfo> spawnNormallyList = new List<ShopItemInfo>();
        //randomly makes a set of 5 shop items
        foreach(ShopItemInfo sio in possibleItems)
        {
            if (sio.spawnNormally && sio.startSpawnDepth <= mapDepth)
            {
                spawnNormallyList.Add(sio);
            }
        }

        SpawnShopItem(possibleItems[0], transform.position);
        float spawnXOffset = 0;
        float spawnYOffset = 0;
        float spawnXOffsetChange = 6;
        float spawnYOffsetChange = -4;
        int newRowCount = 5;
        
        for (int i = 1; i < shopItems; i++)
        {

            
            //Handles spawn Position
            if (i == newRowCount)
            {
                spawnYOffset += spawnYOffsetChange;
                spawnXOffset = spawnXOffsetChange / 2f;
            }
            else if(i%2 == 1)
            {
                spawnXOffset *= -1;
                spawnXOffset += spawnXOffsetChange;
            } else
            {
                spawnXOffset *= -1;
            }

            //Handles what to spawn
            ShopItemInfo toSpawn;
            int itemSpawnIndex;
            if (i <= 2)
            {
                itemSpawnIndex = i;
                toSpawn = possibleItems[itemSpawnIndex];
            }
            else
            {
                itemSpawnIndex = UnityEngine.Random.Range(0, spawnNormallyList.Count);
                toSpawn = spawnNormallyList[itemSpawnIndex];
            }

            SpawnShopItem(toSpawn, transform.position + new Vector3(spawnXOffset, spawnYOffset));
            

        }
    }

    public GameObject SpawnShopItem(ShopItemInfo sio, Vector3 spawnPos)
    {
        string ePath = Path.Combine("ShopItems", sio.fileName);
        GameObject spawnItem = Resources.Load(ePath) as GameObject;
        if (spawnItem == null)
        {
            Debug.LogError("No valid shop item file for " + ePath);
            return null;
        }
        GameObject madeItem = Instantiate(spawnItem, spawnPos, Quaternion.identity);
        ShopItem sItem = madeItem.GetComponent<ShopItem>();
        sItem.cost = sio.cost + UnityEngine.Random.Range(-1 * sio.costVariance, sio.costVariance+1);
        sItem.sio = sio;
        madeItem.transform.parent = this.transform;
        return madeItem;
    }

    // Update is called once per frame
    void Update()
    {
        if(yEndReached == false)
        {
            moveTime += Time.deltaTime;
            transform.position = Vector3.Slerp(startPos, new Vector3(startPos.x, yEnd), moveTime / maxMoveTime);
            if(moveTime >= maxMoveTime)
            {
                yEndReached = true;
            }
        }
    }

    public void GainHealth()
    {
        HelperFunctions.GainHealth();
    }

    public void GainClear()
    {
        HelperFunctions.GainClear();
    }

    public void GainPowerUp(PowerUpType pType)
    {
        HelperFunctions.GainPowerUp(pType);
    }

    public bool LosePowerUp(PowerUpType pType)
    {
        return HelperFunctions.LosePowerUp(pType);
    }

    public bool LoseRandomPowerUp(PowerUpType pType)
    {
        int basicPupCount = 4;
        int maxIndex = (int)PowerUpType.shotPen;
        int pIndex = (int)pType;
        for (int i = pIndex; i < pIndex+basicPupCount; i++)
        {
            int modI = i;
            if(i > maxIndex)
            {
                modI = i%basicPupCount;
            }
            if (LosePowerUp((PowerUpType)modI))
            {
                return true;
            }
        }
        return false;
    }

    public void MakeItemDesc(string desc)
    {
        RemoveItemDesc();
        currDesc = StringToVectorManager.current.StringToVectors(desc,1,StringAlignment.center);
        currDesc.transform.position = CamManager.current.camFinalSpot +  new Vector3(0, -10);
        currDesc.transform.parent = this.transform;
    }

    public void RemoveItemDesc()
    {
        if(currDesc == null)
        {
            //Debug.LogError("Trying to destroy non-existant description");
            return;
        }
        StringToVectorManager.current.DestroyString(currDesc);
    }

}
