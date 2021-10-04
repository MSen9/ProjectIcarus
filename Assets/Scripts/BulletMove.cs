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
    shotSize
}

public class BulletMove : MonoBehaviour
{
    //BulletType bulletType = BulletType.damage;
    float frameSpeed = 1/50f;
    float moveSpeed = 4f;
    //the rotation on the z axis with which the bullet is moving
    Vector2 moveVals;
    bool hitWall = false;
    Collider2D ignoreWall = null;
    public BulletType bulletType = BulletType.powerUp;
    public PowerUpType powerType = PowerUpType.none;
    Rigidbody2D rb;

    //keeps a list of entities already hit by the bullet, stops 'double hits'
    //resets on a bounce
    public List<GameObject> alreadyHit;
    // Start is called before the first frame update

    void Start()
    {
        moveVals = (transform.rotation * Vector2.up) * frameSpeed * moveSpeed;
        rb = GetComponent<Rigidbody2D>();
        alreadyHit = new List<GameObject>();
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        //move the BIG SHOT (or regular shot)
        rb.MovePosition(rb.position + moveVals);
        //transform.position += moveVals;
        hitWall = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Main bullet has hit trigger");
        if(collision.tag == "Wall" && hitWall == false && ignoreWall != collision)
        {
            //rotate the bullet and move it based on excess movement and 
            hitWall = true;
            ignoreWall = collision;
            //calculate the bounce
            
            //works with any angle
            int xMult = 1;
            int yMult = -1;
            float wallAngle = collision.transform.rotation.z;
            Vector3 n = new Vector3(Mathf.Sin(wallAngle)*xMult,Mathf.Cos(wallAngle)*yMult);
            Vector3 v = moveVals;
            Vector3 u = (v.x * n.x + v.y * n.y) * n;
            Vector3 w = v - u;
            moveVals = (w - u);
            rb.MoveRotation(Mathf.Atan2(moveVals.y, moveVals.x) * Mathf.Rad2Deg - 90f);
            alreadyHit.Clear();
            //transform.rotation = Quaternion.Euler(0f,0f, Mathf.Atan2(moveVals.y,moveVals.x)*Mathf.Rad2Deg - 90f);
            
        }
    }
}
