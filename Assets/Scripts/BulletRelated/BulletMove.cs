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
    manaGen
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
    public AudioClip powerUpHit;
    List<Vector3> nList;
    //keeps a list of entities already hit by the bullet, stops 'double hits'
    //resets on a bounce
    public List<GameObject> alreadyHit;
    // Start is called before the first frame update

    void Start()
    {
        if (bulletType != BulletType.powerUp)
        {
            nList = new List<Vector3>();
            ignoreWalls = new List<Collider2D>();
        }
        moveVals = (transform.rotation * Vector2.up) * frameSpeed * moveSpeed;
        rb = GetComponent<Rigidbody2D>();
        alreadyHit = new List<GameObject>();
        sp = GetComponent<SoundPlayer>();
        if(sp != null && shotSound != null)
        {

            sp.PlaySound(shotSound, 0.25f);
        }
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        if (hitWall)
        {
            ChangeMovement(nList);
            hitWallFramePassed = true;
            nList.Clear();
        }
        //move the BIG SHOT (or regular shot)
        rb.MovePosition(rb.position + moveVals);
        //transform.position += moveVals;
        hitWall = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Main bullet has hit trigger");
        if(collision.tag == "Wall")
        {
            
            if(bulletType == BulletType.powerUp)
            {
                //TODO: add an animation to the bullet falling apart
                Destroy(gameObject);
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
        moveVals = (w - u);
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
