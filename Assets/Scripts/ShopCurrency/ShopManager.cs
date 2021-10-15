using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ShopManager : MonoBehaviour
{

    GameObject player;
    HealthPowerUpTracker healthPupTracker;
    public static ShopManager current;
    GameObject currDesc;
    // Start is called before the first frame update
    void Start()
    {
        current = this;
        player = GameObject.FindGameObjectWithTag("Player");
        healthPupTracker = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HealthPowerUpTracker>();

        //make the shop items
    }

    public void MakeShopItems()
    {
        //randomly makes a set of 5 shop items
    }

    // Update is called once per frame
    void Update()
    {
        
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
