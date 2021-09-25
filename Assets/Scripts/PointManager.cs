using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{

    //This class handles things like animation with the vertexes as well as them wiggling from damage
    //Also can size all the vertexes
    // Start is called before the first frame update

    //maps the sibling indexes to child indexes for pointts
    public Dictionary<int, int> points;
    void OnEnable()
    {
        GetPoints();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetPoints()
    {
        points = new Dictionary<int, int>();
        //gets all the points and their sibling indexes
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform currChild = transform.GetChild(i);
            if (currChild.gameObject.CompareTag("Point"))
            {
                points.Add(currChild.GetSiblingIndex(), i);
            }
        }
    }
    public void InstantGrowAllVertexes()
    {
        GetPoints();
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
}
