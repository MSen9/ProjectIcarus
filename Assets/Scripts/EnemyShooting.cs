using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum FireType {
    normal,
    wavey //shoots bullets in a ossiclating wave, life an over-the-top machinegun
}

public class EnemyShooting : MonoBehaviour
{
    public GameObject bullet;
    public float reloadTime = 1f;
    float currReloadTime = 0;
    public bool canShoot = false;
    public float shootDistOffset = .5f;
    // Start is called before the first frame update
    void Start()
    {
        currReloadTime = reloadTime;
    }

    void Update()
    {
        if (canShoot)
        {
            if(currReloadTime < 0)
            {
                currReloadTime += reloadTime;
                GameObject madeBullet = Instantiate(bullet, transform.position + transform.rotation * new Vector2(0,shootDistOffset), transform.rotation);
            }
            currReloadTime -= Time.deltaTime;
            
        } 
    }
}
