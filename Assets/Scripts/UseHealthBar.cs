using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseHealthBar : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject healthBar;
    VertexHandling healthVertex;
    HealthPowerUpTracker healthPupTracker;
    HealthHandling hh;
    bool dead = false;
    void Start()
    {
        healthPupTracker = GameObject.FindGameObjectWithTag("Canvas").GetComponent<HealthPowerUpTracker>();
        healthBar = healthPupTracker.bossHPBar;
        healthBar.SetActive(true);
        hh = GetComponent<HealthHandling>();
        SetUpHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
        {
            return;
        }
        float healthPercent = hh.GetHealthFraction() * 100;
        if (healthPercent <= 0)
        {
            dead = true;
            healthBar.SetActive(false);
        } else
        {
            healthVertex.SetVertexLength(healthPercent);
        }

    
    }

    void SetUpHealthBar()
    {
        healthVertex = healthBar.GetComponent<AllPointManager>().vertexObjs[0].GetComponent<VertexHandling>();
        GameObject endPoint = healthBar.GetComponent<AllPointManager>().pointObjs[1];
        endPoint.transform.localScale = new Vector3(endPoint.transform.localScale.x, 1, 1);
    }

    
}
