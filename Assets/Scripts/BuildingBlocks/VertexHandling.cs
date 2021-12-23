using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexHandling : MonoBehaviour
{
    //Defines the start and end points of the 
    GameObject startPoint;
    public GameObject endPoint;
    public bool autoGetEndPoint = false;
    AllPointManager apm;
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
    bool canLoad = false;
    //used for things like bullets which to not spawn in at the start
    //public bool startFullyGrown = false;
    float xDist;
    float yDist;
    public float totalPointDist;

    public bool setUpComplete = false;
    void Start()
    {
        apm = transform.parent.parent.GetComponent<AllPointManager>();
        if (apm.startFullyGrown || MapManager.current == null)
        {
            InstantGrowVertexes();
            return;
        }
        growing = apm.growNormally;
        GetVertexBaseInfo();
        GetPointDist();
        SetBaseVertexAngle();
        //set scale to 0 so it grows out
        transform.localScale = new Vector3(transform.localScale.x, 0,1f);
        //set the extend rate
        extendTime = apm.extendTime;
        extendRate = new Vector3(0f, ((totalPointDist / extendTime) / startPoint.transform.localScale.y) / apm.unitsPerScale);


    }
    

    public void InstantGrowVertexes()
    {
        /*
        if (setUpComplete && Application.isEditor == false)
        {
            return;
        }
        */
        growing = false;
        if (apm == null)
        {
            apm = transform.parent.parent.GetComponent<AllPointManager>();
        }

        if (GetVertexBaseInfo() == false)
        {
            return;
        }
        if(endPoint == null)
        {
            //no end point, setting vector scale to 0
            transform.localScale = new Vector3(transform.localScale.x,0f, 1f);
            return;
        }
        GetPointDist();
        SetBaseVertexAngle();
        SetToFullVertexLength();
        setUpComplete = true;
        
    }

    public void SetVertexLength(float growthPercent)
    {
        float objScale = startPoint.transform.parent.lossyScale.y;
        transform.localScale = new Vector3(transform.localScale.x, ((totalPointDist / apm.unitsPerScale) / objScale)*(growthPercent/100), 1f);
    }

    bool GetVertexBaseInfo()
    {

        startPoint = transform.parent.gameObject;

        if(apm == null || apm.points == null)
        {
           
            Debug.Log("No AllPointManager for: " + transform.parent.parent.name);
            return false;
        }
        //autoGetEndPoint = true;
        if (autoGetEndPoint)
        {
            //will get the next point in the list of point gameobjects
            int startSibIndex = transform.parent.GetSiblingIndex();
            Transform pointOwner = transform.parent.parent;
            int totalPoints = apm.points.Count;
            //get the next point on the list down

            if (apm.points.ContainsKey(startSibIndex) == false)
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
                if (apm.points.ContainsKey(currIndex))
                {
                    endPoint = pointOwner.GetChild(apm.points[currIndex]).gameObject;
                    break;
                }
            }      
            //autoGetEndPoint = false;
        }
        return true;
    }

    void GetPointDist()
    {
        xDist = startPoint.transform.position.x - endPoint.transform.position.x;
        yDist = startPoint.transform.position.y - endPoint.transform.position.y;
        totalPointDist = Mathf.Sqrt(Mathf.Pow(xDist, 2) +
            Mathf.Pow(yDist, 2));
    }

    void SetBaseVertexAngle()
    {
        //assume Point positions can't change while it is growing out
        //Get the distance in units with which the vector has to grow out
        
        float startAngle = Mathf.Atan2(yDist, xDist) * Mathf.Rad2Deg + ANGLE_OFFSET;
        //startPoint is parent so you only need to set that rotation
        startPoint.transform.rotation = Quaternion.Euler(0f, 0f, startAngle);
    }

    public void SetToFullVertexLength()
    {
        SetVertexLength(100f);
    }

    // Update is called once per frame
    void Update()
    {
        //stop the update early
        if (canLoad == false)
        {
            if (MapManager.current != null && MapManager.current.vectorLoad)
            {
                canLoad = true;
            }
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
            if (totalPointDist < transform.localScale.y * apm.unitsPerScale)
            {
                SetToFullVertexLength();
                growing = false;
                //also record for positional changes
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
        extendRate = new Vector3(0f, -1 * totalPointDist / extendTime / startPoint.transform.localScale.y / apm.unitsPerScale/ shrinkTime);
        totalDeathTime = shrinkTime;
    }
}
