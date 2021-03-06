using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PlayerSaveInfo
{
    public int fireRatePowerUps;
    public int manaGenPowerUps;
    public int shotSizePowerUps;
    public int shotPenBuffs;
    public int shotSpreadBuffs;
    public int shotExplodeBuffs;
    public int shotSplitBuffs;
    public int health;
    public int clears;
    public int currency;

    public PlayerSaveInfo(int fireRatePowerUps, int manaGenPowerUps, int shotSizePowerUps,int shotPenBuffs, int shotSpreadBuffs,
        int shotExplodeBuffs, int shotSplitBuffs, int health, int clears, int currency)
    {
        this.fireRatePowerUps = fireRatePowerUps;
        this.manaGenPowerUps = manaGenPowerUps;
        this.shotSizePowerUps = shotSizePowerUps;
        this.shotPenBuffs = shotPenBuffs;
        this.shotSpreadBuffs = shotSpreadBuffs;
        this.shotExplodeBuffs = shotExplodeBuffs;
        this.shotSplitBuffs = shotSplitBuffs;
        this.health = health;
        this.clears = clears;
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
            Destroy(this.gameObject);
            return;
        } 
        if (SaveAndLoad.current.loadingRun)
        {
            //load from saved player stats
            savedInfo = SaveAndLoad.current.runSInfo.psInfo;
            hasInfoSaved = true;
        } else
        {
            //Set default values
            
            savedInfo = new PlayerSaveInfo(0, 0, 0, 0, 0, 0, 0, 3,3, 0);
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
        BulletClears bc = player.GetComponent<BulletClears>();
        savedInfo = new PlayerSaveInfo(ps.fireRateBuffs, ps.manaGenBuffs, ps.shotSizeBuffs, ps.shotPenBuffs, ps.shotSpreadBuffs,ps.shotExplodeBuffs,ps.shotSplitBuffs, hh.health, bc.clears, cm.currency);
    }
}
