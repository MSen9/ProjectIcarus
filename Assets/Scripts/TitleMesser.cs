using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMesser : MonoBehaviour
{
    // Start is called before the first frame update
    bool titleExploded = false;
    public Vector3 damageMove = new Vector3(0.5f,0.5f,0);
    public float damageRotate = 45;
    public float damageTime = .2f;
    public int damageVectors = 10;
    public float titleCorrupt = 1 / 200f;
    AllPointManager apm;
    public float deathVelocity = -3f;
    public float deathRotate = 45;
    public float deathTime = 1.5f;
    void Start() 
    { 
        apm = GetComponent<AllPointManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(titleExploded == false)
        {
            if (Input.GetMouseButton(0))
            {
                apm.DamageAnimation(damageMove, damageRotate, damageTime, damageVectors, titleCorrupt);
            } 
        }
       
        
    }

    public void TitleExplode()
    {
        titleExploded = true;
        apm.DeathAnimation(deathVelocity, deathRotate, deathTime);
    }
}
