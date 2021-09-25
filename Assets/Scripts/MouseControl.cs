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
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //get mouseposition
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //rotate character towards mouse
        if (mainChara != null)
        {
            Vector3 charPos = mainChara.transform.position;
            float angle = Mathf.Atan2(mousePos.y - charPos.y, mousePos.x - charPos.x) * Mathf.Rad2Deg + ANGLE_OFFSET;
            mainChara.transform.rotation = Quaternion.Euler(0f, 0f, angle);


            //change camera position based on mouse
            Camera.main.transform.position = new Vector3(charPos.x, charPos.y, Camera.main.transform.position.z);
            xCamOffset = (mousePos.x - charPos.x) * OFFSET_MODIFIER;
            yCamOffset = (mousePos.y - charPos.y) * OFFSET_MODIFIER;
            Camera.main.transform.position += new Vector3(xCamOffset, yCamOffset, 0);

        }
        
    }
}
