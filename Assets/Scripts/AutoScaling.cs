using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoScaling : MonoBehaviour
{
    // Start is called before the first frame update
    public float scaleRate = 0.2f;
    Vector3 baseScale = new Vector3(1, 1, 0);
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale += baseScale* scaleRate * Time.deltaTime;
    }
}
