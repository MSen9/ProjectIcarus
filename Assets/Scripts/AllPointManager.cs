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
    public List<VertexHandling> vHandlers;
    public bool startFullyGrown = false;
    public float pointScale = 0.6f;
    public bool growNormally = true;

    public bool ignoreLoading = false;
    public bool firstWaveLoad = false;

    public float extendTime = 1;
    void OnEnable()
    {
        GetPoints();
        GetVertexHandlers();
        ScalePoints();
        if (MapManager.current.normalMapLoad)
        {
            DisablePointsSprites();
            MinimizeVectors();
        }
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
                points.Add(currChild.GetSiblingIndex(), i);
                pointObjs.Add(currChild.gameObject);
            }
        }
    }

    void GetVertexHandlers()
    {
        foreach (int index in points.Values)
        {
            Transform currPoint = transform.GetChild(index);
            for (int i = 0; i < currPoint.childCount; i++)
            {
                VertexHandling vHandling = currPoint.GetChild(i).GetComponent<VertexHandling>();
                if (vHandling != null)
                {
                    vHandlers.Add(vHandling);
                }
            }
        }
    }
    public void InstantGrowAllVertexes()
    {
        GetPoints();
        GetVertexHandlers();
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
            angleToPoint = EnemyMovement.NormalizeAngle(angleToPoint);
            float pointRotation = EnemyMovement.NormalizeAngle(point.transform.localRotation.eulerAngles.z);
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
    public void DeathAnimation(float destroyVelocity,float rotationRate,float destroyTime)
    {
        Vector3 myPos = transform.position;
        foreach (GameObject point in pointObjs)
        {
            float trueRotate = rotationRate;
            Vector3 pointPos = point.transform.localPosition;
            float totalDist = Mathf.Abs(pointPos.x) + Mathf.Abs(pointPos.y);
            Vector2 deathMove = new Vector2(pointPos.x/totalDist,pointPos.y/totalDist)*destroyVelocity;
            //calculates the angle towards the point
            float angleToPoint = Mathf.Atan2(point.transform.localPosition.y, point.transform.localPosition.x);
            angleToPoint = EnemyMovement.NormalizeAngle(angleToPoint);
            float pointRotation = EnemyMovement.NormalizeAngle(point.transform.localRotation.eulerAngles.z);
             trueRotate *= CompareRotateAwayDirection(pointRotation, angleToPoint);

            point.GetComponent<PointHandling>().BecomeDead(deathMove, trueRotate, destroyTime);
        }
    }
}
