using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnd : MonoBehaviour
{
    // Start is called before the first frame update
    public float yEnd;
    public Vector3 posStart;
    float yMoveTime = 3f;
    float currMoveTime = 0;
    string endString = "Wow, Your Winner!\nYou've reached the test ending\nI wonder What's the most\n";
    string[] bonusStrings = { "Fire rate Power-Ups", "Shot Size Power-Ups","Power gen power-ups","Power-ups","Health","Clears","Currency"};
    string endEndString  = "\nYou can win with.";
    GameObject endText;
    void Start()
    {
        string totalString = endString + bonusStrings[Random.Range(0, bonusStrings.Length)] + endEndString;
        endText = StringToVectorManager.current.StringToVectors(totalString, 1.2f, StringAlignment.center);
        endText.transform.position = this.transform.position;
        endText.transform.parent = this.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (currMoveTime > yMoveTime)
        {
            return;
        }
        currMoveTime += Time.deltaTime;
        transform.position = Vector3.Lerp(posStart, new Vector3(posStart.x, yEnd), currMoveTime / yMoveTime);
        
    }
}
