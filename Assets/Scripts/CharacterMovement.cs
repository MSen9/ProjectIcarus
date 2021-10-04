using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    Vector2 currSpeed;
    float maxSpeed = 3f;
    float accelRate = 1/2f;
    float frameMult = 1 / 50f;
    KeyCode upKey;
    KeyCode downKey;
    KeyCode leftKey;
    KeyCode rightKey;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        upKey = KeyCode.W;
        downKey = KeyCode.S;
        leftKey = KeyCode.A;
        rightKey = KeyCode.D;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (Input.GetKey(rightKey))
        {
            if(currSpeed.x < 0)
            {
                currSpeed.x += accelRate;
            }
            currSpeed.x += accelRate;
        } else if(Input.GetKey(leftKey)){
            if (currSpeed.x > 0)
            {
                currSpeed.x -= accelRate;
            }
            currSpeed.x -= accelRate;
        } else
        {
            if (currSpeed.x > 0)
            {
                currSpeed.x -= accelRate;
                if(currSpeed.x < 0)
                {
                    currSpeed.x = 0;
                }
            }
            else if (currSpeed.x < 0)
            {
                currSpeed.x += accelRate;
                if (currSpeed.x > 0)
                {
                    currSpeed.x = 0;
                }
            }
        }

        if (Input.GetKey(upKey))
        {
            if (currSpeed.y < 0)
            {
                currSpeed.y += accelRate;
            }
            currSpeed.y += accelRate;
        }
        else if (Input.GetKey(downKey))
        {
            if (currSpeed.y > 0)
            {
                currSpeed.y -= accelRate;
            }
            currSpeed.y -= accelRate;
        }
        else
        {
            if (currSpeed.y > 0)
            {
                currSpeed.y -= accelRate;
                if (currSpeed.y < 0)
                {
                    currSpeed.y = 0;
                }
            }
            else if (currSpeed.y < 0)
            {
                currSpeed.y += accelRate;
                if (currSpeed.y > 0)
                {
                    currSpeed.y = 0;
                }
            }
        }
        currSpeed.x = Mathf.Clamp(currSpeed.x, maxSpeed * -1, maxSpeed);
        currSpeed.y = Mathf.Clamp(currSpeed.y, maxSpeed * -1, maxSpeed);

        rb.MovePosition(new Vector2(transform.position.x + currSpeed.x * frameMult, 
            transform.position.y + currSpeed.y * frameMult));
       
    }
}
