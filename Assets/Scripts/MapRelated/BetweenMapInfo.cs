using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct PlayerSaveInfo
{
    public int fireRatePowerUps;
    public int manaGenPowerUps;
    public int shotSizePowerUps;
    public int health;
    public int currency;

    public PlayerSaveInfo(int fireRatePowerUps, int manaGenPowerUps, int shotSizePowerUps, int health, int currency)
    {
        this.fireRatePowerUps = fireRatePowerUps;
        this.manaGenPowerUps = manaGenPowerUps;
        this.shotSizePowerUps = shotSizePowerUps;
        this.health = health;
        this.currency = currency;
    }
}
public class BetweenMapInfo : MonoBehaviour
{
    public static BetweenMapInfo current;
    public PlayerSaveInfo savedInfo;
    GameObject player;
    public bool hasInfoSaved = false;
    // Start is called before the first frame update
    void OnEnable()
    {
        if(current != null)
        {
            return;
        } else
        {
            //Set default values
            savedInfo = new PlayerSaveInfo(0, 0, 0, 3, 0);
        }
        current = this;
        DontDestroyOnLoad(gameObject);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveMapInfo()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        hasInfoSaved = true;
        //save the info
        //need to save:
        //Player health
        //Player power-ups
        //Currency
        HealthHandling hh = player.GetComponent<HealthHandling>();
        PlayerShooting ps = player.GetComponent<PlayerShooting>();
        CurrencyManager cm = player.GetComponent<CurrencyManager>();
        savedInfo = new PlayerSaveInfo(ps.fireRateBuffs, ps.manaGenBuffs, ps.shotSizeBuffs, hh.health, cm.currency);
    }
}
