using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ShopItem : MonoBehaviour
{
    
    public BuyEffects buyEffect;
    public PowerUpType pType1 = PowerUpType.none;
    public PowerUpType pType2 = PowerUpType.none;
    public int cost = 0;
    public float textYGap = 5.2f;
    bool canBuy = true;
    public bool repeater = false;
    public float repeatScaling = 2f;
    public bool canRepeatPurchase = true;
    GameObject priceVectorText;
    public ShopItemInfo sio;
    

    public string itemDescription;
    // Start is called before the first frame update
    void Start()
    {
        string costString = (cost).ToString();
        
        priceVectorText = StringToVectorManager.current.StringToVectors(costString, 1,StringAlignment.center, CurrencyManager.moneyCols);
        priceVectorText.transform.position = transform.position - new Vector3(0, textYGap);
        priceVectorText.transform.parent = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool PurchaseItem()
    {
        if (CurrencyManager.current.currency < cost || canBuy == false || (repeater && canRepeatPurchase == false))
        {
            return false;
        }
        
        if(ShopEffect() == false)
        {
            return false;
        }
        CurrencyManager.current.currency -= cost;
        float destroyTime = 2;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            
            AllPointManager apm = gameObject.transform.GetChild(i).GetComponent<AllPointManager>();
            if(apm != null)
            {
                apm.DeathAnimation(0.5f, 30f, destroyTime);
            } else
            {
                if(gameObject.transform.GetChild(i).tag == "SVectorHolder")
                {
                    StringToVectorManager.current.StringExplode(gameObject.transform.GetChild(i).gameObject, destroyTime);
                }
            }
            
        }
        if (repeater)
        {
            GameObject repeatedItem = ShopManager.current.SpawnShopItem(sio,transform.position);
            ShopItem si = repeatedItem.GetComponent<ShopItem>();
            si.cost = Mathf.FloorToInt(cost * repeatScaling);
            si.canRepeatPurchase = false;
        }
        Destroy(gameObject, destroyTime);
        canBuy = false;
        return true;
    }
    public bool ShopEffect()
    {
        bool effectSucceeded = true;
        switch (buyEffect)
        {
            case BuyEffects.gainHp:
                ShopManager.current.GainHealth();
                break;
            case BuyEffects.gainClear:
                ShopManager.current.GainClear();
                break;
            case BuyEffects.gainPowerUp:
                ShopManager.current.GainPowerUp(pType1);
                break;

            case BuyEffects.gainRandomBasicPowerUp:
                PowerUpType randomPType = (PowerUpType)UnityEngine.Random.Range(1, 5);
                ShopManager.current.GainPowerUp(randomPType);
                break;
            case BuyEffects.losePowerUp:
                ShopManager.current.LosePowerUp(pType1);
                break;
            case BuyEffects.loseRandomBasicPowerUp:
                PowerUpType randomLossPType = (PowerUpType)UnityEngine.Random.Range(1, 5);
                effectSucceeded = ShopManager.current.LoseRandomPowerUp(randomLossPType);
                break;
            default:
                Debug.LogError("No buy effect attached to touched shop item");
                break;
        }
        return effectSucceeded;
    }
}
