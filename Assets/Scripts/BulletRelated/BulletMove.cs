using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType{
    damage,
    powerUp
}
public enum PowerUpType
{
    none,
    fireRate,
    shotSize,
    manaGen,
    shotPen,
    shotSpread,
    shotExplode,
    shotSplit,
    OMEGA
}

public class BulletMove : MonoBehaviour
{
    //BulletType bulletType = BulletType.damage;
    float frameSpeed = 1/50f;
    public float moveSpeed = 4f;
    //the rotation on the z axis with which the bullet is moving
    Vector2 moveVals;
    bool hitWall = false;
    bool hitWallFramePassed = false;
    List<Collider2D> ignoreWalls = null;
    public BulletType bulletType = BulletType.powerUp;
    public PowerUpType powerType = PowerUpType.none;
    Rigidbody2D rb;
    SoundPlayer sp;
    public AudioClip shotSound;
    public float powerUpVolume = 0.5f;
    public AudioClip powerUpHit;
    List<Vector3> nList;
    //keeps a list of entities already hit by the bullet, stops 'double hits'
    //resets on a bounce
    public List<GameObject> alreadyHit;
    public int shotPen = 2;
    public int baseShotPen = 0;
    public int shotSplits = 0;
    public int shotExplodes = 0;
    float SHOT_SPLIT_ANGLE_BOOST = 20f;
    float SHOT_SPLIT_ANGLE_NERF = 4 / 5f;
    float SPLIT_SHOT_SECONDS_BACK = 1 / 10f;
    public float accelerationRate = 0;
    public bool canBounce = false;
    public bool wallImmune = false;
    public bool bounceOnce = false;
    public bool explodedShot = false;
    // Start is called before the first frame update
    public int powerUpCount = 1;
    void Start()
    {
        if (bulletType != BulletType.powerUp || canBounce || bounceOnce)
        {
            nList = new List<Vector3>();
            
            if (explodedShot == false)
            {
                alreadyHit = new List<GameObject>();
                ignoreWalls = new List<Collider2D>();
            }
        }
        UpdateMovement();
        rb = GetComponent<Rigidbody2D>();
        
        sp = GetComponent<SoundPlayer>();

    }
    
    public void PlayShotSound(float volume = 0.25f)
    {
        sp = GetComponent<SoundPlayer>();
        if (sp != null && shotSound != null)
        {

            sp.PlaySound(shotSound, 0.25f);
        }
    }

    // Update is called once per frame
    public void UpdateMovement()
    {
        moveVals = (transform.rotation * Vector2.up) * frameSpeed * moveSpeed;
    }
    void FixedUpdate()
    {
        if (hitWall)
        {
            ChangeMovement(nList);
            hitWallFramePassed = true;
            nList.Clear();
            bounceOnce = false;
        }
        //move the BIG SHOT (or regular shot)
        rb.MovePosition(rb.position + moveVals);
        //transform.position += moveVals;
        hitWall = false;
        if(accelerationRate != 0)
        {
            moveSpeed += accelerationRate * Time.fixedDeltaTime;
            UpdateMovement();
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Main bullet has hit trigger");
        if(collision.tag == "Wall")
        {
            if (wallImmune)
            {
                return;
            }

            if (bulletType == BulletType.powerUp && canBounce == false && bounceOnce == false)
            {
                //TODO: add an animation to the bullet falling apart
                GetComponent<AllPointManager>().BreakBullet(0.1f,30f,0.3f);
                return;
            }
            if (ignoreWalls.Contains(collision))
            {
                
                return;
            }
            if (hitWallFramePassed)
            {
                ignoreWalls.Clear();
                hitWallFramePassed = false;
            }
            
           
            //rotate the bullet and move it based on excess movement and 
            ignoreWalls.Add(collision);
            //calculate the bounce

            //works with any angle
            float wallAngle = collision.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            
            Vector3 n = new Vector3(Mathf.Sin(wallAngle),Mathf.Cos(wallAngle));
            //gets the center wall position by taking its starting positions (at the start point) and end point position then taking the average
            Vector3 wallCenter = (collision.transform.position + collision.gameObject.GetComponent<AllPointManager>().pointObjs[1].transform.position) / 2;
            n = CheckTowardsCenter(wallCenter, n);
            
           


            nList.Add(n);
            hitWall = true;



        }
    }

    void SplitShot()
    {
        if (shotSplits > 0)
        {
            if (bulletType == BulletType.powerUp)
            {
                transform.localScale -= new Vector3(0.5f, 0.5f);
            }
            else
            {
                transform.localScale /= Mathf.Pow(PlayerShooting.SPLIT_SHOT_MAIN_SCALING, shotSplits);
            }
            float leftmostOffset = (SHOT_SPLIT_ANGLE_BOOST * shotSplits) * (SHOT_SPLIT_ANGLE_NERF * shotSplits);
            float offsetShift = leftmostOffset / ((shotSplits * 2) - 1);
            Vector3 moveValsBack = moveVals / (frameSpeed / SPLIT_SHOT_SECONDS_BACK);
            for (int i = 0; i < shotSplits * 2; i++)
            {
                GameObject splitBullet = Instantiate(this.gameObject, transform.position - moveValsBack,
                    Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + (leftmostOffset / 2) - offsetShift * i));
                splitBullet.transform.parent = GameObject.FindGameObjectWithTag("BulletList").transform;
                BulletMove bm = splitBullet.GetComponent<BulletMove>();
                //splitBullet.transform.localScale = transform.localScale;
                bm.moveVals = (transform.rotation * Vector2.up) * frameSpeed * moveSpeed;
                bm.shotPen = shotPen;
                splitBullet.transform.localScale /= Mathf.Pow(PlayerShooting.EXPLODE_SHOT_MAIN_SCALING, shotExplodes);
                bm.shotExplodes = 0;
                bm.shotSplits = 0;

            }
            shotSplits = 0;
        }
    }
    public void ExplodeShot(GameObject hitTarget)
    {
        if(shotExplodes == 0)
        {
            return;
        }
        float baseExplodeShots = 2;
        transform.localScale /= Mathf.Pow(PlayerShooting.EXPLODE_SHOT_MAIN_SCALING, shotExplodes) ;
        for (int i = 0; i < shotExplodes; i++)
        {
            for (int j = 0; j < baseExplodeShots; j++)
            {
                GameObject splitBullet = Instantiate(this.gameObject, transform.position,
                        Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z - (0.5f+(i*shotExplodes)+j)/(float)(shotExplodes* baseExplodeShots) * 360f));
                splitBullet.transform.parent = GameObject.FindGameObjectWithTag("BulletList").transform;
                splitBullet.transform.localScale /= Mathf.Pow(PlayerShooting.SPLIT_SHOT_MAIN_SCALING, shotSplits);
                BulletMove bm = splitBullet.GetComponent<BulletMove>();
                //splitBullet.transform.localScale = transform.localScale;
                bm.moveVals = (transform.rotation * Vector2.up) * frameSpeed * moveSpeed;
                bm.shotPen = baseShotPen;
                bm.baseShotPen = baseShotPen;
                bm.shotExplodes = 0;
                
                bm.shotSplits = 0;
                bm.explodedShot = true;
                bm.alreadyHit = new List<GameObject>();
                bm.alreadyHit.Add(hitTarget);
                bm.ignoreWalls = new List<Collider2D>();
            }
        }
        shotExplodes = 0;
    }
    Vector3 GetAverageVector(List<Vector3> vectors)
    {
        Vector3 compVector = Vector3.zero;
        foreach (Vector3 singleN in vectors)
        {
            compVector += singleN;
        }
        Vector3 trueVector = compVector / vectors.Count;
        return trueVector;
    }

    void ChangeMovement(List<Vector3> nVals)
    {
        Vector3 n;
        if(nVals.Count == 1)
        {
            n = nVals[0];
        } else
        {
            n = GetAverageVector(nVals);
            n = Vector3.Normalize(n);
        }
        
        
        Vector3 v = moveVals;
        Vector3 u = (v.x * n.x + v.y * n.y) * n;
        Vector3 w = v - u;
        Vector3 potentialMoveVals = w - u;
        
        if (Vector3.Magnitude((transform.position + new Vector3(moveVals.x,moveVals.y))) < Vector3.Magnitude(transform.position + potentialMoveVals))
        {
            Debug.Log("Stopped bounce that would cause the bullet to move closer to being out of bounds");
            return;
        }
        SplitShot();
        moveVals = potentialMoveVals;
        rb.MoveRotation(Mathf.Atan2(moveVals.y, moveVals.x) * Mathf.Rad2Deg - 90f);
        alreadyHit.Clear();
    }

    

    Vector2 CheckTowardsCenter(Vector3 wallPos, Vector2 n)
    {
        if (wallPos.x < 0)
        {
            if (n.x > 0)
            {
                n.x *= -1;
            }
        }
        else if (wallPos.x > 0)
        {
            if (n.x < 0)
            {
                n.x *= -1;

            }
        }

        if (wallPos.y < 0)
        {
            if (n.y > 0)
            {
                n.y *= -1;
            }
        }
        else if (wallPos.y > 0)
        {
            if (n.y < 0)
            {
                n.y *= -1;

            }
        }
        return n;
    }
    public static Vector2 CheckReversals(Vector3 baseVector, Vector3 beingChecked)
    {
        float xMult = 1;
        float yMult = 1;
        if (baseVector.x > 0)
        {
            if (beingChecked.x < 0)
            {
                xMult *= -1;
            }
        }
        else if (baseVector.x < 0)
        {
            if (beingChecked.x > 0)
            {
                xMult *= -1;
            }
        }

        if (baseVector.y > 0)
        {
            if (beingChecked.y < 0)
            {
                yMult *= -1;
            }
        }
        else if (baseVector.y < 0)
        {
            if (beingChecked.y > 0)
            {
                yMult *= -1;
            }
        }
        return new Vector2(xMult, yMult);
    }
    
}
