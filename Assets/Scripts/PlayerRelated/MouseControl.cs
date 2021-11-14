using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour
{
    public GameObject mainChara;
    float ANGLE_OFFSET = -90f;
    Vector3 mousePos = new Vector3();
    float OFFSET_MODIFIER = 0.2f;
    float xCamOffset = 0;
    float yCamOffset = 0;
    Rigidbody2D rb;
    Vector2 baseCamPos;
    float spiralRotationSpeed = 200f;
    
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
        if (MapManager.current.goingToNextLevel)
        {
            rb.MoveRotation(rb.rotation + spiralRotationSpeed*Time.deltaTime);
            return;
        }
        //new idea: have the camera subtly change as the mouse 
        //get mouseposition
        //rotate character towards mouse
        if (mainChara != null)
        {
            Vector3 charPos = mainChara.transform.position;
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - CamManager.current.currShake;


            //change camera position based on mouse
            /*
            if(dist > 1)
            {
                dist = Mathf.Pow(dist, 1 / 3);
                Camera.main.transform.position = baseCamPos;
            }
            */
            Vector3 camPos = new Vector3(charPos.x, charPos.y, Camera.main.transform.position.z);
            xCamOffset = (mousePos.x - charPos.x) * OFFSET_MODIFIER;
            yCamOffset = (mousePos.y - charPos.y) * OFFSET_MODIFIER;

            camPos += new Vector3(xCamOffset, yCamOffset);
            CamManager.current.camPos = camPos;
            
            float baseAngle = Mathf.Atan2(mousePos.y - charPos.y, mousePos.x - charPos.x) * Mathf.Rad2Deg;
            float rotationalAngle = baseAngle + ANGLE_OFFSET;
            //float dist = Mathf.Sqrt(Mathf.Pow(mousePos.x - charPos.x, 2) + Mathf.Pow(mousePos.y - charPos.y, 2));
            rb.MoveRotation(rotationalAngle);
        }
        
    }
}
