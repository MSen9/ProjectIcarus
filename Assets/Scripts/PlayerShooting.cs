using UnityEngine;
using UnityEngine.UI;
public class PlayerShooting : MonoBehaviour
{
    public float powerUpImmunityTime = 3f;
    public float currPowerUpImmunity = 0f;

    float RELOAD_SPEED_SCALING = 0.85f;
    float SHOT_SIZE_SCALING = 1.1f;
    float MANA_COST_SCALING = 0.9f;
    float MANA_GEN_SCALING = 1.1f;
    public GameObject sliderObj;
    Slider slider;
    bool gainingMana = true;
    bool overflow = false;
    public float manaValue = 75;
    public float manaGenRate = 5; //the rate at which mana charges every second
    float baseManaGenRate = 5;
    float bulletManaCost = 10;
    float baseBulletManaCost = 10;
    float overFlowEmptyRate = 100;
    float maxMana = 100;

    //the penalty for the extra bullets during a burst
    float burstPenaltyMod = 0.25f;
    float burstAimOffset = 30f;


    public int fireRateBuffs = 0;
    public int shotSizeBuffs = 0;
    public int manaGenBuffs = 0;
   
    float shotScale = 1;
    float reloadSpeed = 1f;
    float nextShotTime = 0;
    public GameObject bullet;
    // Start is called before the first frame update
    void Start()
    {
        slider = sliderObj.GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (MapManager.current.doneLoading == false)
        {
            return;
        }
        if (nextShotTime > 0)
        {
            nextShotTime -= Time.deltaTime;
        }
        if (Input.GetMouseButton(0))
        {
            //SHOOT!
            //checks if the player has enough mana
            if(nextShotTime <= 0 && manaValue >= bulletManaCost)
            {
                //TODO: lower "mana" bar
                //start reload
                nextShotTime += reloadSpeed;
                //make bullet
                FireBullet();

                //lowerMana
                manaValue -= bulletManaCost;
            }
        }

        currPowerUpImmunity -= Time.deltaTime;

        //TODO: Change the tint of the main character based on the kind of powerUp just absorbed


        //update mana
        //checks for overflow
        //it is checked this way so that users have an extra frame to shoot a bullet before overflow
        //could probably be exploited if you were superhuman
        if (manaValue >= maxMana)
        {
            overflow = true;
            gainingMana = false;
            //do the bullet burst
            BurstShot();
        }

        if (gainingMana)
        {
            manaValue += manaGenRate * Time.deltaTime;
        } else if (overflow)
        {
            manaValue -= overFlowEmptyRate * Time.deltaTime;
            if(manaValue <= 0)
            {
                overflow = false;
                gainingMana = true;
            }
        }


        //update the slider
        manaValue = Mathf.Clamp(manaValue,0,100);
        //float oldVal = slider.value;
        slider.value = GetManaRatio();
    }

    public float GetManaRatio()
    {
        return manaValue / maxMana;
    }
    GameObject FireBullet(float aimOffset = 0)
    {
        Quaternion trueRotation;
        if (aimOffset != 0)
        {
            float trueOffset = Random.Range(aimOffset * -1, aimOffset);
            trueRotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + trueOffset);
        } else
        {
            trueRotation = transform.rotation;
        }
        GameObject madeBullet = Instantiate(bullet, transform.position + trueRotation * Vector2.up, trueRotation);
        madeBullet.transform.localScale = new Vector3(shotScale, shotScale, 1f);
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
            FireBullet(burstAimOffset);
        }
    }

    public void UpdateBuffs(PowerUpType pType)
    {
        //current balance idea: make it so that the power raises exponentially
        //more motivation to keep things "balanced" also helps accelerate the game
        switch (pType)
        {
            case PowerUpType.fireRate:
                reloadSpeed = 1 * Mathf.Pow(RELOAD_SPEED_SCALING, fireRateBuffs);
                bulletManaCost = baseBulletManaCost * Mathf.Pow(MANA_COST_SCALING, fireRateBuffs); 
                break;
            case PowerUpType.shotSize:
                shotScale = 1 * Mathf.Pow(SHOT_SIZE_SCALING, shotSizeBuffs);
                break;
            case PowerUpType.manaGen:
                manaGenRate = baseManaGenRate * Mathf.Pow(MANA_GEN_SCALING, manaGenBuffs);
                break;
            default:
                Debug.LogError("Cannot update buff: NO POWERUP TYPE");
                break;
        }
    }
}
