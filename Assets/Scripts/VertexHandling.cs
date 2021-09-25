using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexHandling : MonoBehaviour
{
    //Defines the start and end points of the 
    GameObject startPoint;
    public GameObject endPoint;
    public bool autoGetEndPoint = false;
    PointManager pm;
    Vector3 lastStartPointPos;
    Vector3 lastEndPointPos;
    float ANGLE_OFFSET = 90f;
    //the number of frames needed for the extension to complete
    public int ExtendFrames = 60;
    Vector3 extendRate;
    bool growing = true;
    //used for things like bullets which to not spawn in at the start
    public bool startFullyGrown = false;
    float totalPointDist;
    int VERT_PIXELS = 64;
    int PIXELS_PER_UNIT = 100;
    //the amount of units crossed for each 'scale' it has. For instance at 2 scale 
    float UNITS_PER_SCALE = 0;
    void Start()
    {

        GetVertexBaseInfo();
        SetVertexAngle();
        //set scale to 0 so it grows out
        transform.localScale = new Vector3(transform.localScale.x, 0,1f);     
        //set the extend rate
        extendRate = new Vector3(0f, totalPointDist / ExtendFrames);

    }

    public void InstantGrowVertexes()
    {
        GetVertexBaseInfo();
        if(endPoint == null)
        {
            //no end point, setting vector scale to 0
            transform.localScale = new Vector3(transform.localScale.x,0f, 1f);
            return;
        }
        SetVertexAngle();
        transform.localScale = new Vector3(transform.localScale.x, totalPointDist / UNITS_PER_SCALE, 1f);
    }

    bool GetVertexBaseInfo()
    {

        startPoint = transform.parent.gameObject;
        pm = startPoint.transform.parent.GetComponent<PointManager>();
        if(pm == null)
        {
           
            Debug.LogError("No pointManager for object");
            return false;
        }
        //autoGetEndPoint = true;
        if (autoGetEndPoint)
        {
            //will get the next point in the list of point gameobjects
            int startSibIndex = transform.parent.GetSiblingIndex();
            Transform pointOwner = transform.parent.parent;
            int totalPoints = pm.points.Count;
            //get the next point on the list down

            if (pm.points.ContainsKey(startSibIndex) == false)
            {
                Debug.LogError("Starting point is not within the points list");
                return false;
            }
            for (int i = startSibIndex+1; i < totalPoints+startSibIndex+1; i++)
            {
                int currIndex;
                if(i >= totalPoints)
                {
                    currIndex = i - totalPoints;
                } else
                {
                    currIndex = i;
                }
                if (pm.points.ContainsKey(currIndex))
                {
                    endPoint = pointOwner.GetChild(pm.points[currIndex]).gameObject;
                    break;
                }
            }      
            //autoGetEndPoint = false;
        }
        UNITS_PER_SCALE = (VERT_PIXELS / (float)PIXELS_PER_UNIT) * startPoint.transform.localScale.y;
        return true;
    }

    void SetVertexAngle()
    {
        //assume Point positions can't change while it is growing out
        //Get the distance in units with which the vector has to grow out
        float xDist = startPoint.transform.position.x - endPoint.transform.position.x;
        float yDist = startPoint.transform.position.y - endPoint.transform.position.y;
        totalPointDist = Mathf.Sqrt(Mathf.Pow(xDist, 2) +
            Mathf.Pow(yDist, 2));
        float startAngle = Mathf.Atan2(yDist, xDist) * Mathf.Rad2Deg + ANGLE_OFFSET;
        //startPoint is parent so you only need to set that rotation
        startPoint.transform.rotation = Quaternion.Euler(0f, 0f, startAngle);
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
