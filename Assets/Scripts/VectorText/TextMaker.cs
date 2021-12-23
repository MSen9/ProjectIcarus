using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMaker : MonoBehaviour
{
    // Start is called before the first frame update
    public string inputString;
    public float stringScale = 1f;
    public StringAlignment stringAllignment = StringAlignment.left;
    public float[] textCols = null;
    GameObject madeString;
    void Start()
    {
        madeString = StringToVectorManager.current.StringToVectors(inputString, stringScale, stringAllignment, textCols);
        madeString.transform.position = this.transform.position;
        madeString.transform.parent = this.transform;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
