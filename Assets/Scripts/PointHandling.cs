using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointHandling : MonoBehaviour
{
    bool damaged = false;
    bool backToNormalState = false;
    float damageRotation = 0f;
    Vector3 damageMove;
    //the percent with which 
    float damageChangeRate = 0f;
    int damageMoveFrames = 0;
    int maxFrames = 20;
    //tracks the effects of the move and rotation at the frames of movement
    List<GameObject> vertexes;
    // Start is called before the first frame update
    void Start()
    {
        vertexes = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform currChild = transform.GetChild(i);
            if(currChild.GetComponent<VertexHandling>() != null)
            {
                vertexes.Add(currChild.gameObject);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (damaged)
        {
            if(backToNormalState == false)
            {
                damageMoveFrames++;
                transform.localPosition += damageMove * damageChangeRate;
                transform.rotation = Quaternion.Euler(0f, 0f, (transform.rotation.eulerAngles.z) + (damageChangeRate * damageRotation));
            } else
            {
                damageMoveFrames--;
                transform.localPosition -= damageMove * damageChangeRate;
                transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z - (damageChangeRate * damageRotation));
            }
            
            

            if(damageMoveFrames >= maxFrames)
            {
                backToNormalState = true;
            } else if(damageMoveFrames <= 0)
            {
                //done moving
                damaged = false;
                foreach (GameObject vertex in vertexes)
                {
                    vertex.GetComponent<VertexHandling>().fixVertexPositions = true;
                }
            }
        }
    }

    public void BecomeDamaged(Vector3 move,float rotateZ, int damageFrames)
    {
        if (damaged)
        {
            //increase the effect of the current damage (with diminishing returns)
        } else
        {
            damaged = true;
            backToNormalState = false;
            maxFrames = damageFrames;
            damageChangeRate = 1f / maxFrames;
            damageMove = move;
            damageRotation = rotateZ;
            damageMoveFrames = 0;
            foreach(GameObject vertex in vertexes)
            {
                VertexHandling vh = vertex.GetComponent<VertexHandling>();
                vh.RefreshLastPositions();
                vh.fixVertexPositions = false;
            }
        }
    }
}
