using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaGrowth : MonoBehaviour
{
    public GameObject manaSource;
    PlayerShooting ps;
    public GameObject[] points;
    VertexHandling[] vertexes;
    // Start is called before the first frame update

    public void SetManaProgress(float manaPercent)
    {
        float manaSection = 100 / points.Length;
        int pointCount = 0;
        
        while (manaPercent > 0)
        {
            if(pointCount >= points.Length)
            {
                //Debug.LogError("Error: too many points cycled over when setting mana progress");
                break;
            }
            points[pointCount].SetActive(true);
            if (manaPercent > manaSection)
            {
                vertexes[pointCount].SetToFullVertexLength();
            } else
            {
                vertexes[pointCount].SetVertexLength((manaPercent / manaSection) * 100);
            }

            manaPercent -= manaSection;
            pointCount++;
        }

        for (int i = pointCount; i < points.Length; i++)
        {
            points[i].SetActive(false);
        }
    }
    void Start()
    {
        
        UpdateVertexes();
        if (manaSource == null)
        {
            manaSource = GameObject.FindGameObjectWithTag("Player");
        }
        ps = manaSource.GetComponent<PlayerShooting>();
    }
    private void Update()
    {
        SetManaProgress(Mathf.Clamp(ps.GetManaRatio()*100,0,100));
    }
    void UpdateVertexes()
    {
        vertexes = new VertexHandling[points.Length];
        for (int i = 0; i < points.Length; i++)
        {
            vertexes[i] = points[i].transform.GetChild(0).GetComponent<VertexHandling>();
        }
    }
}
