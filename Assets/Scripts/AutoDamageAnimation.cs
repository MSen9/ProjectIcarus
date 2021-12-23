using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDamageAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    AllPointManager apm;
    public float damageRate = 0.5f;
    public Vector3 damageMove = new Vector3(0.5f,0.5f);
    public float damageRotate = 30f;
    public float damageTime = 1f;
    public int damagePoints = 5;
    float intervalTime = 0;
    void Start()
    {
        apm = GetComponent<AllPointManager>();   
    }

    // Update is called once per frame
    void Update()
    {
        intervalTime += Time.deltaTime;
        if (intervalTime >= damageRate)
        {
            intervalTime -= damageRate;
            apm.DamageAnimation(damageMove, damageRotate, damageTime, damagePoints,0);
        }
    }
}
