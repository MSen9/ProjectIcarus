using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandling : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isPlayer = false;
    public int health = 1;
    public float invincibilityTime = 0f;
    float currInvincibilityTime = 0f;
    Dictionary<Collider2D,float> bulletIgnoreTimes;
    //the time with which individual bullets are ignored 
    //float indivBulletIgnoreTime = 1f;


    public Vector3 damageMove = new Vector3(0.1f, 0.1f, 0);
    public float damageRotate = 20f;
    public int baseDamageTime = 10;
    //Collider2D[] colliders;
    AllPointManager pm;
    void Start()
    {
        //colliders = GetComponents<BoxCollider2D>();
        pm = GetComponent<AllPointManager>();
        bulletIgnoreTimes = new Dictionary<Collider2D,float>();

    }

    // Update is called once per frame
    void Update()
    {
        currInvincibilityTime -= Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Main bullet has hit trigger");
        BulletMove bulletInfo = collision.gameObject.GetComponent<BulletMove>();
        if (bulletInfo != null)
        {
            //Got hit by a bullet, figure out what kind
            if(bulletInfo.bulletType == BulletType.damage)
            {
                //check if the gameObject has invincibility at all
                if(invincibilityTime > 0)
                {
                    

                    if (currInvincibilityTime > 0)
                    {
                        return;
                    }

                    //has invincibility time, likely a player
                    PlayerShooting pShooting = gameObject.GetComponent<PlayerShooting>();
                    if (pShooting != null)
                    {
                        pShooting.currPowerUpImmunity = pShooting.powerUpImmunityTime;
                    }

                    currInvincibilityTime = invincibilityTime;
                    
                }
                if (bulletInfo.alreadyHit.Contains(gameObject))
                {
                    //have already been hit by the bullet, needs to bounce first
                    return;
                }

                
                Debug.Log("Hit");
                health--;
                
                bulletInfo.alreadyHit.Add(gameObject);
                pm.DamageAnimation(damageMove,damageRotate,baseDamageTime);
            } else if(bulletInfo.bulletType == BulletType.powerUp)
            {
                if (isPlayer)
                {
                    PlayerShooting pShooting = gameObject.GetComponent<PlayerShooting>();
                    if(pShooting.currPowerUpImmunity > 0f)
                    {
                        //immune, ignore powerUp
                        return;
                        
                    }
                    //TODO: Assign tint on pShooting so they change to that color slightly while immune
                    //process the powerup
                    switch(bulletInfo.powerType){
                        case PowerUpType.fireRate:
                            pShooting.FireRateBuffs++;
                            break;
                        case PowerUpType.shotSize:
                            pShooting.ShotSizeBuffs++;
                            break;

                    }

                    pShooting.currPowerUpImmunity = pShooting.powerUpImmunityTime;
                    //powerUpIsNoLongerNeeded
                    Destroy(collision.gameObject);
                }
            }
        }
    }

   
}
