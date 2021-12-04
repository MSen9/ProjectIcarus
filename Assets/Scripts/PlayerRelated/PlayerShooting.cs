using UnityEngine;
using UnityEngine.UI;
public class PlayerShooting : MonoBehaviour
{
    public float powerUpImmunityTime = 2f;
    public float currPowerUpImmunity = 0f;

    float RELOAD_SPEED_SCALING = 0.85f;
    float SHOT_SIZE_SCALING = 1.1f;
    float MANA_COST_SCALING = 0.9f;
    float MANA_GEN_SCALING = 1.1f;
    float defaultShotSize = 1.2f;
    float SHOT_SPREAD_FIRERATE_NERF = 3 / 2f;
    float SHOT_SPREAD_MANA_SPEND_NERF = 3 / 5f;
    //These two variables increase the angle at which the bullets come out
    float SHOT_SPREAD_ANGLE_BOOST = 30f;
    float SHOT_SPREAD_ANGLE_NERF = 4 / 5f;
    float SHOT_SPLiT_FIRERATE_NERF = 3 / 2f;
    float SHOT_SPLIT_MANA_SPEND_BUFF = 3 / 2f;
    float BASE_FIRERATE = 1;

    public GameObject sliderObj;
    AllPointManager apm;
    bool properColor = true;
    public Color powerUpCol;
    Slider slider;
    bool gainingMana = true;
    public float manaValue = 75;
    public float manaGenRate = 5; //the rate at which mana charges every second
    float baseManaGenRate = 5;
    float bulletManaCost = 10;
    float baseBulletManaCost = 10;
    float maxMana = 100;

    CamManager sShaker;
    //the penalty for the extra bullets during a burst
    float burstPenaltyMod = 0.25f;
    float burstAimOffset = 30f;
    float SHOOT_DIST_MOD = 0.8f;


    public int fireRateBuffs = 0;
    public int shotSizeBuffs = 0;
    public int manaGenBuffs = 0;
    public int shotPenBuffs = 0;
    public int shotSpreadBuffs = 0;
    public int shotExplodeBuffs = 0;
    public int shotSplitBuffs = 0;

    int BASE_SHOT_PEN = 2;
    float shotScale = 1;
    float reloadSpeed = 1f;
    float nextShotTime = 0;
    public GameObject bullet;
    bool canShoot = true;


    HealthHandling hh;
    SoundPlayer sPlayer;
    // Start is called before the first frame update
    void Start()
    {
        if(BetweenMapInfo.current != null && BetweenMapInfo.current.hasInfoSaved)
        {
            PlayerSaveInfo currInfo = BetweenMapInfo.current.savedInfo;
            for (int i = 0; i < currInfo.fireRatePowerUps; i++)
            {
                HelperFunctions.GainPowerUp(PowerUpType.fireRate);
            }
            for (int i = 0; i < currInfo.manaGenPowerUps; i++)
            {
                HelperFunctions.GainPowerUp(PowerUpType.manaGen);
            }
            for (int i = 0; i < currInfo.shotSizePowerUps; i++)
            {
                HelperFunctions.GainPowerUp(PowerUpType.shotSize);
            }
            for (int i = 0; i < currInfo.shotPenBuffs; i++)
            {
                HelperFunctions.GainPowerUp(PowerUpType.shotPen);
            }
            for (int i = 0; i < currInfo.shotSpreadBuffs; i++)
            {
                HelperFunctions.GainPowerUp(PowerUpType.shotSpread);
            }
            for (int i = 0; i < currInfo.shotExplodeBuffs; i++)
            {
                HelperFunctions.GainPowerUp(PowerUpType.shotExplode);
            }
            for (int i = 0; i < currInfo.shotSplitBuffs; i++)
            {
                HelperFunctions.GainPowerUp(PowerUpType.shotSplit);
            }
        }
        if(sliderObj != null)
        {
            slider = sliderObj.GetComponent<Slider>();
        }
        
        UpdateBuffs();
        apm = GetComponent<AllPointManager>();
        sShaker = Camera.main.GetComponent<CamManager>();
        hh = GetComponent<HealthHandling>();
        sPlayer = GetComponent<SoundPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (MapManager.current == null || MapManager.current.doneLoading == false || MapManager.current.mapOver)
        {
            return;
        }
        if (nextShotTime > 0)
        {
            nextShotTime -= Time.deltaTime;
        }
        if (Input.GetMouseButton(0) && canShoot)
        {
            //SHOOT!
            //checks if the player has enough mana
            if(nextShotTime <= 0)
            {
                FireBullet();
            }
        }

        if(manaValue < 0)
        {
            manaValue = 0;
        }
        currPowerUpImmunity -= Time.deltaTime;

        //Changes the tint of the main character based on the kind of powerUp just absorbed
        if(currPowerUpImmunity > 0)
        {
            properColor = false;
            apm.SetAllColors(Color.Lerp(Color.white, powerUpCol,(currPowerUpImmunity / powerUpImmunityTime)*2));
        } else if(properColor == false)
        {
            properColor = true;
            apm.SetAllColors(Color.white);
        }
        

        //update mana
        //checks for overflow
        //it is checked this way so that users have an extra frame to shoot a bullet before overflow
        //could probably be exploited if you were superhuman
        

        if (gainingMana)
        {
            manaValue += manaGenRate * Time.deltaTime;
        } 

        if (manaValue >= maxMana)
        {
            FireBullet(true);
        }

        //update the slider
        manaValue = Mathf.Clamp(manaValue,0,100);
        //float oldVal = slider.value;
        if(slider != null)
        {
            slider.value = GetManaRatio();
        }
        
    }

    public float GetManaRatio()
    {
        return manaValue / maxMana;
    }

    public void EndShooting()
    {
        canShoot = false;
        gainingMana = false;
    }

    

    GameObject[] FireBullet(bool ignoreReload = false, float aimOffset = 0)
    {
        //start reload
        int shotsFired = 1 + shotSpreadBuffs * 2;
        GameObject[] bulletList = new GameObject[shotsFired];
        if(ignoreReload == false)
        {
            nextShotTime += reloadSpeed;
        }
        sShaker.ShakeCamera(new Vector3(0.05f, 0.05f),0.1f);
       
        if(shotsFired == 1)
        {
            bulletList[0] = CreateBullet(aimOffset);
        } else
        {
            float leftmostOffset = (SHOT_SPREAD_ANGLE_BOOST * shotSpreadBuffs) * (SHOT_SPREAD_ANGLE_NERF * shotSpreadBuffs);
            float offsetShift = leftmostOffset / (shotSpreadBuffs * 2);
            for (int i = 0; i < shotsFired; i++)
            {
                bulletList[i] = CreateBullet(aimOffset+leftmostOffset - offsetShift*i);
            }
        }
        

        //lower Mana
        manaValue -= bulletManaCost;
        //make bullet
        return bulletList;
    }
    GameObject CreateBullet(float aimOffset = 0)
    {
        Quaternion trueRotation;
        if (aimOffset != 0)
        {
            trueRotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + aimOffset);
        } else
        {
            trueRotation = transform.rotation;
        }
        GameObject madeBullet = Instantiate(bullet, transform.position + trueRotation * Vector2.up*shotScale*SHOOT_DIST_MOD, trueRotation);
        madeBullet.transform.parent = GameObject.FindGameObjectWithTag("BulletList").transform;
        madeBullet.transform.localScale = new Vector3(shotScale, shotScale, 1f);
        BulletMove bm = madeBullet.GetComponent<BulletMove>();
        bm.shotPen = BASE_SHOT_PEN + shotPenBuffs;
        bm.baseShotPen = BASE_SHOT_PEN + shotPenBuffs;
        bm.shotSplits = shotSplitBuffs;
        bm.shotExplodes = shotExplodeBuffs;
        bm.PlayShotSound();
        //madeBullet.GetComponent<AllPointManager>().InstantGrowAllVertexes(); // corrects the size to fit the new scale
        return madeBullet;
    }
    void BurstShot()
    {
        //function in order to decide how many bullets to shoot:
        //the amount of bullets fired with the max mana '100'
        //+ the number of bullets lost with one second of cooldown (manaRate)
        //+ a penaltyMod
        int burstBullets = Mathf.RoundToInt((maxMana/bulletManaCost + manaGenRate/bulletManaCost)*(1+burstPenaltyMod));
        for (int i = 0; i < burstBullets; i++)
        {
            CreateBullet(burstAimOffset);
        }
    }

    public void UpdateBuffs()
    {
        //current balance idea: make it so that the power raises exponentially
        //more motivation to keep things "balanced" also helps accelerate the game
        reloadSpeed = BASE_FIRERATE * Mathf.Pow(RELOAD_SPEED_SCALING, fireRateBuffs) * Mathf.Pow(SHOT_SPREAD_FIRERATE_NERF,shotSpreadBuffs) * Mathf.Pow(SHOT_SPLiT_FIRERATE_NERF,shotSplitBuffs);
        bulletManaCost = baseBulletManaCost * Mathf.Pow(MANA_COST_SCALING, fireRateBuffs) * Mathf.Pow(SHOT_SPREAD_MANA_SPEND_NERF, shotSpreadBuffs) * Mathf.Pow(SHOT_SPLIT_MANA_SPEND_BUFF, shotSplitBuffs); 
        shotScale = defaultShotSize * Mathf.Pow(SHOT_SIZE_SCALING, shotSizeBuffs);
        manaGenRate = baseManaGenRate * Mathf.Pow(MANA_GEN_SCALING, manaGenBuffs);
        
    }

    public void PowerUpSound(AudioClip ac)
    {
        sPlayer.PlaySound(ac, 0.25f);
    }
}
