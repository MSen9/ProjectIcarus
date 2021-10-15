using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float baseMoveSpeed = 2f;
    public float rotateRate = 120f;
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
    AllPointManager apm;
    // Start is called before the first frame update
    void Start()
    {

        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        eShoot = GetComponent<EnemyShooting>();
        rotateGoal = GetRotationGoal();

        transform.rotation = Quaternion.Euler(0, 0, rotateGoal);
        //rb.MoveRotation(rotateGoal);
        apm = GetComponent<AllPointManager>();

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
        if (MapManager.current.doneLoading == false || apm.doneExtending == false)
        {
            return;
        }
        if (player == null)
        {
            return;
        }
        UpdatePlayerDist();
        rotateGoal = GetRotationGoal();
        float currAngle = rb.rotation;

        currAngle = HelperFunctions.RotateTowards(currAngle, rotateGoal, rotateRate, Time.fixedDeltaTime);
        rb.MoveRotation(currAngle);
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

        moveVals = (transform.rotation * Vector2.up) * baseMoveSpeed * Time.fixedDeltaTime * currMoveMod;
        rb.MovePosition(rb.position + moveVals);

        
        
    }

    
}
