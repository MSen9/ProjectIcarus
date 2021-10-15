using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterWallDetector : MonoBehaviour
{
    public GameObject detectingFor;
    public string tagType = "Wall";
    PlayerMovement cm;
    CircleCollider2D cd2D;
    // Start is called before the first frame update
    void Start()
    {
        cm = detectingFor.GetComponent<PlayerMovement>();
        cd2D = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Main bullet has hit trigger");
        if (collision.tag == tagType)
        {
            //calculate a vector parallel and perpendicular to wall
            Vector3 pPosition = detectingFor.transform.position;
            Vector3 wPosition = collision.gameObject.transform.position;
            Vector2 hypotenuse = new Vector2(pPosition.x - wPosition.x, pPosition.y-wPosition.y);


            float wallAngle = collision.transform.rotation.eulerAngles.z * Mathf.Deg2Rad;
            Vector3 closestPoint = collision.ClosestPoint(pPosition);
            Vector3 closestPointDist = pPosition - closestPoint;
            //int xMult = 1;
            //int yMult = 1;
            Vector3 n = new Vector3(Mathf.Sin(wallAngle), Mathf.Cos(wallAngle));
            Vector2 mults = BulletMove.CheckReversals(closestPointDist, n);
            n.x *= mults.x;
            n.y *= mults.y;

            detectingFor.transform.position = closestPoint + n * cd2D.radius * 1.2f;
            /*
            if (collision.bounds.Contains(detectingFor.transform.position))
            {
                detectingFor.transform.position = closestPoint + n * cd2D.radius * -1.2f;
            }
            */
            /*
            float oldHypotenuseMagnitute = hypotenuse.magnitude;
            cm.transform.position += n * MOVE_INCREMENT;
            int moveAttempts = 0;
            if(hypotenuse.magnitude < oldHypotenuseMagnitute)
            {
                //moved closer to the center of the collider, going out of bounds
                //turn around vector
                n = n * -1;
            }
            
            //an unefficient means up pushing out the player from a wall
            //but it is also relatively not that complex and works with multiple walls
            while (collision.bounds.Contains(cm.transform.position + n*cd2D.radius*-2))
            {
                cm.transform.position += n * MOVE_INCREMENT;
                moveAttempts++;
                if(moveAttempts > MAX_MOVE_ATTEMPTS)
                {
                    Debug.LogError("Infinite Loop when trying to move out of colliders, stopping attempt");
                    break;
                }
            }
            */
        }
    }
}
