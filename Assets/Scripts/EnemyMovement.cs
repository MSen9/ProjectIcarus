using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float baseMoveSpeed = 2f;
    public float rotateRate = 120f;
    int HALF_CIRCLE_DEGREES = 180;
    int CIRLCE_DEGREES = 360;
    float updateTimeGap = 1/50f;
    float rotateGoal = 0;
    public float fireRange = 30;
    public float fireMovementMod = 0.8f;
    float currMoveMod = 1f;
    public float fireRangeGraceTime = 1f;
    float currFireRangeGraceTime = 1f;
    Rigidbody2D rb;
    GameObject player;
    Vector2 moveVals;
    float playerDist = 0;
    EnemyShooting eShoot;
    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        eShoot = GetComponent<EnemyShooting>();
        rotateGoal = GetRotationGoal();
        
        rb.MoveRotation(rotateGoal);

    }

    float GetRotationGoal()
    {
        return Mathf.Atan2(transform.position.y - player.transform.position.y, transform.position.x - player.transform.position.x) * Mathf.Rad2Deg + 90f;
    }

    void UpdatePlayerDist()
    {
        playerDist = Mathf.Sqrt(Mathf.Pow(transform.position.x - player.transform.position.x,2) + Mathf.Pow(transform.position.y- player.transform.position.y, 2));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdatePlayerDist();
        rotateGoal = GetRotationGoal();
        rotateGoal = NormalizeAngle(rotateGoal);
        float currAngle = rb.rotation;

        //rb.MoveRotation(Quaternion.RotateTowards(Quaternion.Euler(0,0,currAngle), Quaternion.Euler(0,0,rotateGoal), rotateRate).z*Mathf.Rad2Deg);
        currAngle = NormalizeAngle(currAngle);
        int negativeMod = 1;
        float angleDist = currAngle - rotateGoal;
        //the left and right distance to the 0 degrees angle for the current and 
        if(angleDist > HALF_CIRCLE_DEGREES || (angleDist < 0 && angleDist >= -1*HALF_CIRCLE_DEGREES))
        {
            currAngle += rotateRate * updateTimeGap;
            /*
            if (currAngle > rotateGoal)
            {
                //currAngle = rotateGoal;
            }
            */
            rb.MoveRotation(currAngle);
        } else if ((angleDist <= HALF_CIRCLE_DEGREES && angleDist > 0) || angleDist < -1 * HALF_CIRCLE_DEGREES)
        {
            currAngle -= rotateRate * updateTimeGap;
            /*
            if (currAngle < rotateGoal)
            {
                //currAngle = rotateGoal;
            }
            */
            rb.MoveRotation(currAngle);
        }

        if (eShoot == null)
        {
            Debug.LogError("No shooting script attached to this object: " + gameObject.name);
        }
        //check player dist
        if (playerDist < fireRange && eShoot)
        {
            currFireRangeGraceTime = fireRangeGraceTime;
            eShoot.canShoot = true;
            currMoveMod = fireMovementMod;

        }
        else
        {
            if (currFireRangeGraceTime < 0)
            {
                eShoot.canShoot = false;
                currMoveMod = 1f;
            }
            currFireRangeGraceTime -= Time.fixedDeltaTime;
           
        }

        moveVals = (transform.rotation * Vector2.up) * baseMoveSpeed * updateTimeGap * currMoveMod;
        rb.MovePosition(rb.position + moveVals);

        
        
    }

    float NormalizeAngle(float angle)
    {
        while(angle > CIRLCE_DEGREES)
        {
            angle -= CIRLCE_DEGREES;
        }
        while(angle < 0)
        {
            angle += CIRLCE_DEGREES;
        }

        return angle;
    }
}
