using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamManager : MonoBehaviour
{
    float shakeTime = 0;
    float maxShakeTime = 0.5f;
    Vector3 shakeMovement;
    Vector3 defaultPos;
    public static CamManager current;
    public Vector3 camPos;
    Vector3 modCamPos;
    public Vector3 currShake = Vector3.zero;

    Vector3 camStartMove;
    Vector3 camDestination;
    float moveTime;
    float currMoveTime;
    bool movingToDest = false;
    bool mapOver = false;
    public Vector3 camFinalSpot = Vector3.zero;
    // Start is called before the first frame update
    public bool canScroll = false;
    float scrollMin = -100;
    float scrollMax = 0;
    float MOVE_SCROLL_RATE = 10;
    float WHEEL_SCROLL_RATE = 3;
    void OnEnable()
    {
        current = this;
        camPos = gameObject.transform.position;
    }

    // Update is called once per frame

    
    void LateUpdate()
    {
        if (canScroll)
        {
            if (Input.GetKey(KeyCode.W))
            {
                camPos += new Vector3(0, MOVE_SCROLL_RATE * Time.deltaTime);

            }
            else if (Input.GetKey(KeyCode.S))
            {
                camPos -= new Vector3(0, MOVE_SCROLL_RATE * Time.deltaTime);

            }
            camPos += new Vector3(0, Input.mouseScrollDelta.y * WHEEL_SCROLL_RATE);
            camPos = new Vector3(camPos.x, Mathf.Clamp(camPos.y, scrollMin, scrollMax), camPos.z);
        }

        if (movingToDest)
        {
            currMoveTime += Time.deltaTime;
            modCamPos = Vector3.Lerp(camStartMove, camDestination, currMoveTime / moveTime);
        } else if(mapOver == false)
        {
            modCamPos = camPos;
        }
        
        if (shakeTime > 0 && mapOver == false)
        {
            //Starts shake at 'full power' for the first half of the shaking, then dials it back.
            
            currShake = Vector3.Lerp(Vector3.zero, shakeMovement, shakeTime * 2 / maxShakeTime);
            currShake = new Vector3(Random.Range(currShake.x * -1, currShake.x),
                Random.Range(currShake.y * -1, currShake.y));
            modCamPos = camPos + currShake;
            shakeTime -= Time.deltaTime;
        } else
        {
            currShake = Vector3.zero;
        }
        
        
        Camera.main.transform.position = modCamPos;
    }

    public void SetScrollBounds(float min, float max = 0)
    {
        scrollMin = min;
        scrollMax = max;
    }
    public void ShakeCamera(Vector3 moveAmount, float durration)
    {
        if (moveAmount.magnitude < shakeMovement.magnitude * Mathf.Clamp01(shakeTime * 2 / maxShakeTime))
        {
            return;
        }
        maxShakeTime = durration;
        shakeTime = maxShakeTime;

        shakeMovement = moveAmount;
    }

    public void MoveToPoint(Vector3 destination, float moveTime = 2f)
    {
        camStartMove = Camera.main.transform.position;
        camDestination = new Vector3(destination.x,destination.y, Camera.main.transform.position.z);
        this.moveTime = moveTime;
        currMoveTime = 0;
        movingToDest = true;
    }

    public void SetPoint(Vector3 pos)
    {
        camPos = pos;
    }
    public void EndMapCam(Vector3 destination, float moveTime = 2f)
    {
        MoveToPoint(destination, moveTime = 2f);
        camFinalSpot = destination;
        mapOver = true;
    }
}
