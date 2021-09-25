using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    float reloadSpeed = 1f;
    float nextShotTime = 0;
    public GameObject bullet;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (nextShotTime > 0)
        {
            nextShotTime -= Time.deltaTime;
        }
        if (Input.GetMouseButton(0))
        {
            //SHOOT!
            if(nextShotTime <= 0)
            {
                //lower "mana" bar
                //start reload
                nextShotTime += reloadSpeed;
                GameObject madeBullet = Instantiate(bullet, transform.position + transform.rotation*Vector2.up, transform.rotation);
                //make bullet
            }
        }
        
        
    }
}
