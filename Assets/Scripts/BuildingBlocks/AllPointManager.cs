using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllPointManager : MonoBehaviour
{

    //This class handles things like animation with the vertexes as well as them wiggling from damage
    //Also can size all the vertexes
    // Start is called before the first frame update

    //maps the sibling indexes to child indexes for pointts
    public Dictionary<int, int> points;
    public List<GameObject> pointObjs;
    public List<GameObject> vertexObjs;
    public List<VertexHandling> vHandlers;
    public bool startFullyGrown = false;
    public float pointScale = 0.6f;
    public bool growNormally = true;
 
    public bool ignoreLoading = false;
    public bool firstWaveLoad = false;

    public float extendTime = 1;
    float extendTimeRemaining;
    public bool doneExtending = false;
    int VERT_PIXELS = 64;
    int PIXELS_PER_UNIT = 200;
    public float unitsPerScale; 
    Color firstPointColor;
    void OnEnable()
    {
        GetPoints();
        GetVertexInfo();
        ScalePoints();
        extendTimeRemaining = extendTime;
        
        unitsPerScale = (VERT_PIXELS / (float)PIXELS_PER_UNIT) * pointScale;
    }

    void DisablePointsSprites()
    {
        foreach(GameObject p in pointObjs)
        {
            p.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    void MinimizeVectors()
    {
        foreach(VertexHandling vh in vHandlers)
        {
            vh.SetVertexLength(0);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(doneExtending == false)
        {
            extendTimeRemaining -= Time.deltaTime;
            if(extendTimeRemaining <= 0)
            {
                doneExtending = true;
            }
        }
        
    }

    void GetPoints()
    {
        points = new Dictionary<int, int>();
        pointObjs = new List<GameObject>();
        //gets all the points and their sibling indexes
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform currChild = transform.GetChild(i);
            if (currChild.gameObject.CompareTag("Point"))
            {
                if (firstPointColor != null)
                {
                    firstPointColor = currChild.gameObject.GetComponent<SpriteRenderer>().color;
                }
                points.Add(currChild.GetSiblingIndex(), i);
                pointObjs.Add(currChild.gameObject);
            }
        }
    }

    void GetVertexInfo()
    {
        vertexObjs = new List<GameObject>();
        vHandlers = new List<VertexHandling>();
        foreach (int index in points.Values)
        {
            Transform currPoint = transform.GetChild(index);
            for (int i = 0; i < currPoint.childCount; i++)
            {
                Transform currVertex = currPoint.GetChild(i);
                VertexHandling vHandling = currVertex.GetComponent<VertexHandling>();
                if (vHandling != null)
                {
                    vertexObjs.Add(currVertex.gameObject);
                    vHandlers.Add(vHandling);
                }
            }
        }
    }
    public void InstantGrowAllVertexes()
    {
        unitsPerScale = (VERT_PIXELS / (float)PIXELS_PER_UNIT) * pointScale;
        doneExtending = true;
        GetPoints();
        GetVertexInfo();
        ScalePoints();
        foreach (VertexHandling vh in vHandlers)
        {
            
             vh.InstantGrowVertexes();
            
        }
        
    }

    void ScalePoints()
    {
        foreach(int pIndex in points.Values)
        {
            Transform currPoint = transform.GetChild(pIndex);
            currPoint.localScale = new Vector3(pointScale, pointScale, currPoint.localScale.z);
        }
    }

    public void DamageAnimation(Vector3 damageMove, float damageRotate, float damageTime, int damagePoints)
    {
        //moves and rotates the points slightly to signal damage, can also change the tint of the sprite
        if(damagePoints > points.Count)
        {
            //Debug.LogError("Error, too many damage points assign to: " + gameObject.name);
            damagePoints = points.Count;
        }
        
        int[] damageIndexes = new int[damagePoints];
        int[] randomIndexes = new int[points.Count];
        for (int i = 0; i < randomIndexes.Length; i++)
        {
            randomIndexes[i] = i;
        }
        //shuffle the indexes and take the first x where x is the number of damagePoints

        int n = randomIndexes.Length;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            int temp = randomIndexes[k];
            randomIndexes[k] = randomIndexes[n];
            randomIndexes[n] = temp;
        }

        for (int i = 0; i < damagePoints; i++)
        {
            int currInd = randomIndexes[i];
            GameObject point = pointObjs[currInd];
            Vector3 trueMove = damageMove;
            float trueRotate = damageRotate;
            if (point.transform.position.x < transform.position.x)
            {
                trueMove.x *= -1;
            }
            if (point.transform.position.y < transform.position.y)
            {
                trueMove.y *= -1;
            }
            //calculates the angle towards the point
            float angleToPoint = Mathf.Atan2(point.transform.localPosition.y, point.transform.localPosition.x);
            angleToPoint = HelperFunctions.NormalizeAngle(angleToPoint);
            float pointRotation = HelperFunctions.NormalizeAngle(point.transform.localRotation.eulerAngles.z);
            //checks if the rotation angle would be brought closer to the center if were added to
            trueRotate *= CompareRotateAwayDirection(pointRotation, angleToPoint);


            point.GetComponent<PointHandling>().BecomeDamaged(trueMove, trueRotate, damageTime);
        }
    }
    //This function returns the direction a1 should rotate in order to rotate further away form a2;
    float CompareRotateAwayDirection(float a1, float a2)
    {
        float rotationDirection = 1;
        if (a1 <= a2)
        {
            if (a1 + 180 > a2)
            {
                rotationDirection *= -1;
            }
        }
        else if (a1 >= a2)
        {
            if (a1 - 180 < a2)
            {
                rotationDirection *= -1;
            }
        }
        return rotationDirection;
    }

    public void BreakBullet()
    {
        float destroyVelocity = 0.5f;
        float rotationRate = 30f;
        float destroyTime = 2f;
        
        PolygonCollider2D[] pCols = gameObject.GetComponents<PolygonCollider2D>();
        for (int i = 0; i < pCols.Length; i++)
        {
            pCols[i].enabled = false;
        }
        BoxCollider2D[] bCols = gameObject.GetComponents<BoxCollider2D>();
        for (int i = 0; i < bCols.Length; i++)
        {
            bCols[i].enabled = false;
        }
        gameObject.GetComponent<BulletMove>().enabled = false;
        Destroy(gameObject, destroyTime);
        DeathAnimation(destroyVelocity, rotationRate, destroyTime);
    }

    public void DeathAnimation(float destroyVelocity = 0.5f,float rotationRate = 30,float destroyTime = 2f)
    {
        Vector3 myPos = transform.position;
        foreach (GameObject point in pointObjs)
        {
            float trueRotate = rotationRate;
            Vector3 pointPos = point.transform.position - myPos;
            float totalDist = Mathf.Abs(pointPos.x) + Mathf.Abs(pointPos.y);
            Vector2 deathMove;
            if (totalDist != 0)
            {
                deathMove = new Vector2(pointPos.x / totalDist, pointPos.y / totalDist) * destroyVelocity;
            } else
            {
                deathMove = Vector2.zero;
            }
            
            //calculates the angle towards the point
            float angleToPoint = Mathf.Atan2(point.transform.localPosition.y, point.transform.localPosition.x);
            angleToPoint = HelperFunctions.NormalizeAngle(angleToPoint);
            float pointRotation = HelperFunctions.NormalizeAngle(point.transform.localRotation.eulerAngles.z);
            trueRotate *= CompareRotateAwayDirection(pointRotation, angleToPoint);
            PointHandling ph = point.GetComponent<PointHandling>();
            if(ph != null)
            {
                try
                {
                    ph.BecomeDead(deathMove, trueRotate, destroyTime);
                } catch
                {
                    Debug.LogError("Error trying to do point death animations. deathMove: " + deathMove.ToString() +
                        " trueRotate: " + trueRotate.ToString() + 
                        " destroyTime: " + destroyTime.ToString());
                }
                
            } else
            {
                Debug.LogError("Error: Point failed to be destroyed, no point handling object. Point obj is: " + gameObject.name);
            }
           
        }
    }

    public void SetAllColors(Color c)
    {
        foreach(GameObject point in pointObjs)
        {
            point.GetComponent<SpriteRenderer>().color = c;
        }

        foreach (GameObject vertex in vertexObjs)
        {
            vertex.GetComponent<SpriteRenderer>().color = c;
        }
    }

    public Color GetPointColor(int pointIndex = 0)
    {
        SpriteRenderer sr = pointObjs[pointIndex].GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("Error no color found for: " + gameObject.name + " At point index: " + pointIndex.ToString());
            return Color.white;
        }
        return sr.color;
    }
}
