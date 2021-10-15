using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexHandling : MonoBehaviour
{
    //Defines the start and end points of the 
    GameObject startPoint;
    public GameObject endPoint;
    public bool autoGetEndPoint = false;
    AllPointManager pm;
    Vector3 lastStartPointPos;
    Vector3 lastEndPointPos;
    float ANGLE_OFFSET = 90f;
    //the amount of time needed to fully extend the vertex
    public float extendTime = 1;
    Vector3 extendRate;
    bool shrinking = false;
    float deathTime = 0;
    float totalDeathTime;
    bool growing = true;
    //used for things like bullets which to not spawn in at the start
    //public bool startFullyGrown = false;
    float totalPointDist;
    int VERT_PIXELS = 64;
    int PIXELS_PER_UNIT = 200;
    //the amount of units crossed for each 'scale' it has. For instance at 2 scale 
    float unitsPerScale = 0;

    float MIN_MOVE_CHECK = 0.001f;
    public bool fixVertexPositions = true;
    void Start()
    {
        pm = transform.parent.parent.GetComponent<AllPointManager>();
        if (pm.startFullyGrown)
        {
            InstantGrowVertexes();
            return;
        }
        growing = pm.growNormally;
        GetVertexBaseInfo();
        SetBaseVertexAngle();
        //set scale to 0 so it grows out    
        transform.localScale = new Vector3(transform.localScale.x, 0,1f);
        //set the extend rate
        extendTime = pm.extendTime;
        extendRate = new Vector3(0f, ((totalPointDist / extendTime) / startPoint.transform.localScale.y) / unitsPerScale);


    }
    

    public void InstantGrowVertexes()
    {
        if(pm == null)
        {
            pm = transform.parent.parent.GetComponent<AllPointManager>();
        }

        GetVertexBaseInfo();
        if(endPoint == null)
        {
            //no end point, setting vector scale to 0
            transform.localScale = new Vector3(transform.localScale.x,0f, 1f);
            return;
        }
        SetBaseVertexAngle();
        SetToFullVertexLength();
    }

    public void SetVertexLength(float growthPercent)
    {
        transform.localScale = new Vector3(transform.localScale.x, ((totalPointDist / unitsPerScale) / startPoint.transform.parent.localScale.y)*(growthPercent/100), 1f);
    }

    bool GetVertexBaseInfo()
    {

        startPoint = transform.parent.gameObject;

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
        unitsPerScale = (VERT_PIXELS / (float)PIXELS_PER_UNIT) * startPoint.transform.localScale.y;
        return true;
    }

    void SetBaseVertexAngle()
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

    public void SetToFullVertexLength()
    {
        transform.localScale = new Vector3(transform.localScale.x, (totalPointDist / unitsPerScale)/startPoint.transform.parent.localScale.y, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        //stop the update early
        if(MapManager.current.vectorLoad == false)
        {
            return;
        }

        //when valid change the rotation to face towards end point and grow until you reach it.
        //use the scale and distance between points to measure if it is done growing
        if (shrinking)
        {
            transform.localScale += extendRate*Time.deltaTime;
            if(transform.localScale.y < 0)
            {
                transform.localScale = new Vector2(transform.localScale.x, 0);
            }
            deathTime += Time.deltaTime;
            Color c = GetComponent<SpriteRenderer>().color;
            c.a = 1 - deathTime / totalDeathTime;
            GetComponent<SpriteRenderer>().color = c;
        }
        else if (growing)
        {
            transform.localScale += extendRate*Time.deltaTime;
            if (totalPointDist < transform.localScale.y * unitsPerScale)
            {
                transform.localScale = new Vector3(transform.localScale.x, totalPointDist / unitsPerScale,1f);
                growing = false;
                //also record for positional changes
                RefreshLastPositions();
            }
        } else
        {
            //do code to check for point moving and update the vetex locations
            if (fixVertexPositions && endPoint != null)
            {
                if((lastStartPointPos - startPoint.transform.localPosition).magnitude > MIN_MOVE_CHECK ||
                    (lastEndPointPos - endPoint.transform.localPosition).magnitude > MIN_MOVE_CHECK)
                {
                    SetBaseVertexAngle();
                    SetToFullVertexLength();
                }
                RefreshLastPositions();
            }


            
        }
    }

    public void RefreshLastPositions()
    {
        lastStartPointPos = startPoint.transform.localPosition;
        if(endPoint != null)
        {
            lastEndPointPos = endPoint.transform.localPosition;
        }
        
    }

    public void StartShrinking(float shrinkTime)
    {
        shrinking = true;
        totalDeathTime = shrinkTime;
        extendRate *= -1*extendTime;
        extendRate /= totalDeathTime;
        fixVertexPositions = false;
    }
}
