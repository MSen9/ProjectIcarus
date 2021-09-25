using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexHandling : MonoBehaviour
{
    //Defines the start and end points of the 
    GameObject startPoint;
    public GameObject endPoint;
    public bool autoGetEndPoint = true;
    Vector3 lastStartPointPos;
    Vector3 lastEndPointPos;
    float ANGLE_OFFSET = 90f;
    //the number of frames needed for the extension to complete
    public int ExtendFrames = 60;
    Vector3 extendRate;
    bool growing = true;
    float totalPointDist;
    int VERT_PIXELS = 64;
    int PIXELS_PER_UNIT = 100;
    //the amount of units crossed for each 'scale' it has. For instance at 2 scale 
    float UNITS_PER_SCALE = 0;
    void Start()
    {
        
        startPoint = transform.parent.gameObject;

        if (autoGetEndPoint)
        {
            //will get the next point in the list of point gameobjects
            int parentIndex = transform.parent.GetSiblingIndex();
            Transform pointOwner = transform.parent.parent;
            int totalChildren = pointOwner.childCount;
            //get the next point on the list down
            int endPointIndex = parentIndex + 1;
            if(endPointIndex == totalChildren)
            {
                //last point, get the first one
                endPointIndex = 0;
            }
           
           
            for (int i = 0; i < pointOwner.childCount; i++)
            {
                if(pointOwner.GetChild(i).GetSiblingIndex() == endPointIndex)
                {
                    //the next point is the end point
                    endPoint = pointOwner.GetChild(i).gameObject;
                } 
            }
        }
        UNITS_PER_SCALE = (VERT_PIXELS / (float)PIXELS_PER_UNIT) * startPoint.transform.localScale.y;
        //set scale to 0
        transform.localScale = new Vector3(transform.localScale.x, 0,1f);
        //assume Point positions can't change while it is growing out
        //Get the distane in units with which the vector has to grow out

        float xDist = startPoint.transform.position.x - endPoint.transform.position.x;
        float yDist = startPoint.transform.position.y - endPoint.transform.position.y;
        totalPointDist = Mathf.Sqrt(Mathf.Pow(xDist,2) +
            Mathf.Pow(yDist, 2));

        float startAngle = Mathf.Atan2(yDist, xDist) * Mathf.Rad2Deg + ANGLE_OFFSET;
        //transform.rotation = Quaternion.Euler(0f, 0f, startAngle);
        //startPoint is parent so you only need to set that rotation
        startPoint.transform.rotation = Quaternion.Euler(0f, 0f, startAngle);

        extendRate = new Vector3(0f, totalPointDist / ExtendFrames);

    }

    // Update is called once per frame
    void Update()
    {
        //when valid change the rotation to face towards end point and grow until you reach it.
        //use the scale and distance between points to measure if it is done growing
        if (growing)
        {
            transform.localScale += extendRate;
            if (totalPointDist < transform.localScale.y * UNITS_PER_SCALE)
            {
                transform.localScale = new Vector3(transform.localScale.x, totalPointDist / UNITS_PER_SCALE,1f);
                growing = false;
                //also record for positional changes
                lastStartPointPos = startPoint.transform.position;
                lastEndPointPos = startPoint.transform.position;
            }
        } else
        {

        }
    }
}
