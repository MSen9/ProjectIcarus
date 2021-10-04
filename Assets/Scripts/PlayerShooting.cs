using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public float powerUpImmunityTime = 3f;
    public float currPowerUpImmunity = 0f;
    int _fireRateBuffs = 0;
    int _shotSizeBuffs = 0;
    float RELOAD_SPEED_SCALING = 0.9f;
    float SHOT_SIZE_SCALING = 1.1f;
    public int FireRateBuffs
    {
        get
        {
            return _fireRateBuffs;
        }
        set
        {
            if(_fireRateBuffs != value)
            {
                _fireRateBuffs = value;
                UpdateBuffs(PowerUpType.fireRate);

            }
        }
    }
    public int ShotSizeBuffs
    {
        get
        {
            return _shotSizeBuffs;
        }
        set
        {
            if (_shotSizeBuffs != value)
            {
                _shotSizeBuffs = value;
                UpdateBuffs(PowerUpType.shotSize);

            }
        }
    }
    float shotScale = 1;
    float reloadSpeed = 1f;
    float nextShotTime = 0;
    public GameObject bullet;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (nextShotTime > 0)
        {
            nextShotTime -= Time.deltaTime;
        }
        if (Input.GetMouseButton(0))
        {
            //SHOOT!
            if(nextShotTime <= 0)
            {
                //TODO: lower "mana" bar
                //start reload
                nextShotTime += reloadSpeed;
                GameObject madeBullet = Instantiate(bullet, transform.position + transform.rotation*Vector2.up, transform.rotation);
                madeBullet.transform.localScale = new Vector3(shotScale, shotScale, 1f);
                //make bullet
            }
        }

        currPowerUpImmunity -= Time.deltaTime;

        //TODO: Change the tint of the main character based on the kind of powerUp just absorbed
    }

    void UpdateBuffs(PowerUpType pType)
    {
        //current balance idea: make it so that the power raises exponentially
        //more motivation to keep things "balanced" also helps accelerate the game
        switch (pType)
        {
            case PowerUpType.fireRate:
                reloadSpeed = 1 * Mathf.Pow(RELOAD_SPEED_SCALING, FireRateBuffs);
                break;
            case PowerUpType.shotSize:
                shotScale = 1 * Mathf.Pow(SHOT_SIZE_SCALING, ShotSizeBuffs);
                break;
        }
    }
}
