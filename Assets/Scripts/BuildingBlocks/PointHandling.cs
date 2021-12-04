using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointHandling : MonoBehaviour
{
    bool damaged = false;
    int damageInstances = 0;
    bool dying = false;
    Vector3 dyingMove;
    Vector3 basePos;
    float baseRotate;
    float dyingRotation;
    float deathTime = 0;
    float totalDeathTime;
    bool backToNormalState = false;
    float damageRotation = 0f;
    Vector3 damageMove;

    float currDamageTime;
    float maxDamageTime;
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
        GetComponent<SpriteRenderer>().color = GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        if (dying)
        {
            transform.position += dyingMove;
            transform.rotation = Quaternion.Euler(0f, 0f, (transform.rotation.eulerAngles.z) + dyingRotation);
            deathTime += Time.deltaTime;
            Color c = GetComponent<SpriteRenderer>().color;
            c.a = 1 - deathTime / totalDeathTime;
            GetComponent<SpriteRenderer>().color = c;
        } else if (damaged)
        {
            currDamageTime += Time.deltaTime;
            if (backToNormalState == false)
            {            
                transform.localPosition = Vector3.Lerp(basePos, basePos + damageMove, currDamageTime / maxDamageTime);
                transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(baseRotate,baseRotate+damageRotation,currDamageTime/maxDamageTime));
            } else
            {
                transform.localPosition = Vector3.Lerp(basePos + damageMove, basePos, currDamageTime / maxDamageTime);
                transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(baseRotate + damageRotation, baseRotate, currDamageTime / maxDamageTime));
            }
            
            

            if(currDamageTime >= maxDamageTime)
            {
                if (backToNormalState)
                {
                    damaged = false;
                    damageInstances = 0;
                    transform.localPosition = basePos;
                    transform.localRotation = Quaternion.Euler(0,0, baseRotate);
                    foreach (GameObject vertex in vertexes)
                    {
                        vertex.GetComponent<VertexHandling>().fixVertexPositions = true;
                    }
                } else
                {
                    backToNormalState = true;
                    currDamageTime = 0;
                }
                
            } 
        }
    }

    
    public void BecomeDamaged(Vector3 move,float rotateZ, float damageTime, float transformCorruptMod = 1/20f)
    {
        
        if (damaged)
        {
            //increase the effect of the current damage (with diminishing returns)
            damageInstances++;
            damageMove += move / damageInstances;
            damageRotation += rotateZ / damageInstances;

            if (backToNormalState)
            {
                backToNormalState = false;
                currDamageTime = maxDamageTime - currDamageTime;
            }
            maxDamageTime += damageTime / damageInstances;
        } else
        {
            damaged = true;
            damageInstances = 1;
            backToNormalState = false;
            maxDamageTime = damageTime;
            currDamageTime = 0;
            basePos = transform.localPosition;
            baseRotate = transform.localRotation.eulerAngles.z;
            damageMove = move;
            damageRotation = rotateZ;
            foreach(GameObject vertex in vertexes)
            {
                VertexHandling vh = vertex.GetComponent<VertexHandling>();
                //vh.RefreshLastPositions();
                vh.fixVertexPositions = false;
            }
        }
        basePos += transformCorruptMod * move;
        baseRotate += transformCorruptMod * rotateZ;
    }

    public void BecomeDead(Vector2 move, float rotateRate, float deathTime)
    {
        dying = true;
        dyingMove = move*Time.fixedDeltaTime;
        dyingRotation = rotateRate*Time.fixedDeltaTime;
        totalDeathTime = deathTime;
        foreach(GameObject vertex in vertexes)
        {
            vertex.GetComponent<VertexHandling>().StartShrinking(deathTime);
        }
    }
}
