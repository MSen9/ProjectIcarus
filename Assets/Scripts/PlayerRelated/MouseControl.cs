using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour
{
    public GameObject mainChara;
    float ANGLE_OFFSET = -90f;
    Vector3 mousePos = new Vector3();
    /*
    float OFFSET_MODIFIER = 0.2f;
    float xCamOffset = 0;
    float yCamOffset = 0;
    */
    Rigidbody2D rb;
    Vector2 baseCamPos;
    // Start is called before the first frame update
    void Start()
    {
        if(mainChara == null)
        {
            mainChara = GameObject.FindGameObjectWithTag("Player");
        }
        rb = mainChara.GetComponent<Rigidbody2D>();
        baseCamPos = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //new idea: have the camera subtly change as the mouse 
        //get mouseposition
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //rotate character towards mouse
        if (mainChara != null)
        {
            Vector3 charPos = mainChara.transform.position;
            float baseAngle = Mathf.Atan2(mousePos.y - charPos.y, mousePos.x - charPos.x) * Mathf.Rad2Deg;
            float rotationalAngle = baseAngle + ANGLE_OFFSET;
            //float dist = Mathf.Sqrt(Mathf.Pow(mousePos.x - charPos.x, 2) + Mathf.Pow(mousePos.y - charPos.y, 2));
            //mainChara.transform.rotation = Quaternion.Euler(0f, 0f, angle);
            rb.MoveRotation(rotationalAngle);

            //change camera position based on mouse
            /*
            if(dist > 1)
            {
                dist = Mathf.Pow(dist, 1 / 3) - 1;
                Camera.main.transform.position = baseCamPos;
            }
            Camera.main.transform.position = new Vector3(charPos.x, charPos.y, Camera.main.transform.position.z);
            
            Camera.main.transform.position += new Vector3(xCamOffset, yCamOffset, 0);
            */
        }
        
    }
}
