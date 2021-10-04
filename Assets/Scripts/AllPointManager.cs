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
    public bool startFullyGrown = false;
    public float pointScale = 0.6f;
    

    
    void OnEnable()
    {
        GetPoints();
        ScalePoints();
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
    public void InstantGrowAllVertexes()
    {
        GetPoints();
        ScalePoints();
        foreach (int index in points.Values)
        {
            Transform currPoint = transform.GetChild(index);
            for (int i = 0; i < currPoint.childCount; i++)
            {
                VertexHandling vHandling = currPoint.GetChild(i).GetComponent<VertexHandling>();
                if(vHandling != null)
                {
                    vHandling.InstantGrowVertexes();
                }
            }
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

    public void DamageAnimation(Vector3 damageMove, float damageRotate, int damageTime)
    {
        //moves and rotates the points slightly to signal damage, can also change the tint of the sprite

        foreach (GameObject point in pointObjs)
        {
            Vector3 trueMove = damageMove;
            float trueRotate = damageRotate;
            if(point.transform.position.x < transform.position.x)
            {
                trueMove.x *= -1;
            }
            if (point.transform.position.y < transform.position.y)
            {
                trueMove.y *= -1;
            }
            //calculates the angle towards the point
            float angleToPoint = Mathf.Atan2(point.transform.localPosition.y, point.transform.localPosition.x);
            //checks if the rotation angle would be brought closer to the center if were added to
            if(point.transform.localRotation.eulerAngles.z > angleToPoint)
            {
                //reverses the rotation change so that the point moves further from the center
                trueRotate *= -1;
            }

            point.GetComponent<PointHandling>().BecomeDamaged(trueMove,trueRotate,damageTime);
        }
    }
}
