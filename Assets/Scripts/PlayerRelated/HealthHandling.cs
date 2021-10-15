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


    public Vector2 damageMove = new Vector2(0.2f, 0.2f);
    public float damageRotate = 40f;

    //the time spent moving out for the 'damage animation' the same amount of time is then spent moving back to the normal state
    public float baseDamageTime = 0.5f;
    public int damagePoints = 3;

    public float deathMove = 0.5f;
    public float deathRotate = 60f;
    float deathTime = 2f;

    HealthPowerUpTracker healthPupTracker;
    //Collider2D[] colliders;
    AllPointManager pm;
    MapManager mapManager;
    void Start()
    {
        //colliders = GetComponents<BoxCollider2D>();
        pm = GetComponent<AllPointManager>();
        bulletIgnoreTimes = new Dictionary<Collider2D,float>();
        if (isPlayer)
        {
            healthPupTracker = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HealthPowerUpTracker>();
            for (int i = 0; i < health; i++)
            {
                healthPupTracker.MakeUIObj(trackedTypes.health);
            }
        }

        if (!isPlayer)
        {
            mapManager = GameObject.FindGameObjectWithTag("MapManager").GetComponent<MapManager>();
            mapManager.EnemySpawn(gameObject);
        }

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
                        healthPupTracker.RemoveUIObj(trackedTypes.health);
                        pShooting.currPowerUpImmunity = pShooting.powerUpImmunityTime;
                    }

                    currInvincibilityTime = invincibilityTime;
                    
                }
                if (bulletInfo.alreadyHit.Contains(gameObject))
                {
                    //have already been hit by the bullet, needs to bounce first
                    return;
                }
                health--;
                
                bulletInfo.alreadyHit.Add(gameObject);
                if(health > 0)
                {
                    pm.DamageAnimation(damageMove, damageRotate, baseDamageTime,damagePoints);
                } else
                {
                    //destruction effect

                    BoxCollider2D[] bCols = GetComponents<BoxCollider2D>();
                    for (int i = 0; i < bCols.Length; i++)
                    {
                        bCols[i].enabled = false;
                    }
                    CircleCollider2D[] cCols = GetComponents<CircleCollider2D>();
                    for (int i = 0; i < cCols.Length; i++)
                    {
                        cCols[i].enabled = false;
                    }
                    if (isPlayer)
                    {
                        GetComponent<PlayerShooting>().enabled = false;
                        GetComponent<PlayerMovement>().enabled = false;
                    } else
                    {
                        GetComponent<EnemyMovement>().enabled = false;
                        GetComponent<EnemyShooting>().enabled = false;
                        mapManager.EnemyDeath(gameObject);
                    }

                    pm.DeathAnimation(deathMove, deathRotate, deathTime);
                    Destroy(gameObject, deathTime);
                    this.enabled = false;
                }
                
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
                    HelperFunctions.GainPowerUp(bulletInfo.powerType);

                    pShooting.currPowerUpImmunity = pShooting.powerUpImmunityTime;
                    //powerUpIsNoLongerNeeded
                    Destroy(collision.gameObject);
                }
            }
        }
    }

   
}
