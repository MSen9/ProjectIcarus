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

    public ShopItemInfo(string fileName, int cost, int costVariance = 0, bool spawnNormally = true)
    {
        this.fileName = fileName;
        this.cost = cost;
        this.costVariance = costVariance;
        this.spawnNormally = spawnNormally;
    }
    
}
public enum BuyEffects
{
    none,
    gainHp,
    gainPowerUp,
    losePowerUp
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
    // Start is called before the first frame update
    float shopItems = 5;
    void Start()
    {
        current = this;
        player = GameObject.FindGameObjectWithTag("Player");
        healthPupTracker = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HealthPowerUpTracker>();
        possibleItems = new List<ShopItemInfo>();
        possibleItems.Add(new ShopItemInfo("HeartShopItem", 20,0,false));
        possibleItems.Add(new ShopItemInfo("BasicFireRatePUP", -10, 5));
        possibleItems.Add(new ShopItemInfo("BasicPowerGenPUP", -10, 5));
        possibleItems.Add(new ShopItemInfo("BasicShotSizePUP", -10, 5));
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
            if (sio.spawnNormally)
            {
                spawnNormallyList.Add(sio);
            }
        }

        SpawnShopItem(possibleItems[0], transform.position);
        float spawnXOffset = 0;
        float spawnXOffsetChange = 6;
        for (int i = 1; i < shopItems; i++)
        {
            if(i%2 == 1)
            {
                spawnXOffset *= -1;
                spawnXOffset += spawnXOffsetChange;
            } else
            {
                spawnXOffset *= -1;
            }
            int itemSpawnIndex = UnityEngine.Random.Range(0, spawnNormallyList.Count);
            SpawnShopItem(spawnNormallyList[itemSpawnIndex], transform.position + new Vector3(spawnXOffset, 0));
            

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
        madeItem.transform.parent = this.transform;
        return madeItem;
    }

    // Update is called once per frame
    void Update()
    {
        if(yEndReached == false)
        {
            moveTime += Time.deltaTime;
            transform.position = Vector3.Slerp(startPos, new Vector3(transform.position.x, yEnd), moveTime / maxMoveTime);
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

    public void GainPowerUp(PowerUpType pType)
    {
        HelperFunctions.GainPowerUp(pType);
    }

    public void LosePowerUp(PowerUpType pType)
    {
        HelperFunctions.LosePowerUp(pType);
    }

    public void MakeItemDesc(string desc)
    {
        currDesc = StringToVectorManager.current.StringToVectors(desc,1,StringAlignment.center);
        currDesc.transform.position = new Vector3(0, -10);
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
