using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ShopItem : MonoBehaviour
{
    
    public BuyEffects buyEffect;
    public PowerUpType pType1 = PowerUpType.none;
    public PowerUpType pType2 = PowerUpType.none;
    public float cost = 0;
    public float textYGap = 5.2f;
    GameObject priceVectorText;
    

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

    public void PurchaseItem()
    {
        if(CurrencyManager.current.currencyThing < cost)
        {
            return;
        }

        CurrencyManager.current.currencyThing -= cost;
        ShopEffect();
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
        Destroy(gameObject, destroyTime);
    }
    public void ShopEffect()
    {
        switch (buyEffect)
        {
            case BuyEffects.gainHp:
                ShopManager.current.GainHealth();
                break;
            case BuyEffects.gainPowerUp:
                ShopManager.current.GainPowerUp(pType1);
                break;
            case BuyEffects.losePowerUp:
                ShopManager.current.LosePowerUp(pType1);
                break;
            default:
                Debug.LogError("No buy effect attached to touched shop item");
                break;
        }
    }
}
